using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Library
{
    ////start
    // use LIB_HeuristicStateFullBase
    class LIB_BeamSearchFullState
    {
        class BayesianBeamWidthSuggester
        {
            double meanSec;
            double varianceSec;
            double varianceStateSec;
            double varianceObserveSec;
            double timeLimitSec;
            int currentTurn;
            int maxTurn;
            int warmupTurn;
            int minBeamWidth;
            int maxBeamWidth;
            int currentBeamWidth;
            DateTime startTime;
            DateTime lastTime;

            public BayesianBeamWidthSuggester(int maxTurn, int warmupTurn, double timeLimitSec, int standardBeamWidth, int minBeamWidth, int maxBeamWidth)
            {
                this.meanSec = timeLimitSec / (maxTurn * standardBeamWidth);
                var stddevSec = meanSec * 0.1;
                this.varianceSec = stddevSec * stddevSec;
                var stddevStateSec = 0.01 * meanSec;
                this.varianceStateSec = stddevStateSec * stddevStateSec;
                var stddevObserveSec = 0.05 * meanSec;
                this.varianceObserveSec = stddevObserveSec * stddevObserveSec;
                this.timeLimitSec = timeLimitSec;
                this.currentTurn = 0;
                this.maxTurn = maxTurn;
                this.warmupTurn = warmupTurn;
                this.minBeamWidth = minBeamWidth;
                this.maxBeamWidth = maxBeamWidth;
                this.currentBeamWidth = 0;
                this.startTime = DateTime.Now;
                this.lastTime = this.startTime;
            }
            void UpdateState()
            {
                varianceSec += varianceStateSec;
            }
            void UpdateDistribution(double durationSec)
            {
                var oldMean = meanSec;
                var oldVariance = varianceSec;
                var noiseVariance = varianceObserveSec;

                meanSec = (oldMean * noiseVariance + oldVariance * durationSec) / (noiseVariance + oldVariance);
                varianceSec = oldVariance * noiseVariance / (oldVariance + noiseVariance);
            }
            int CalcSafeBeamWidth()
            {
                var remainingTurn = maxTurn - currentTurn;
                var elapsedTime = (DateTime.Now - startTime).TotalSeconds;
                var remainingTime = timeLimitSec - elapsedTime;

                var varianceTotal = varianceSec * varianceObserveSec;

                var mean = remainingTurn * meanSec;
                var variance = remainingTurn * varianceTotal;
                var stddev = Sqrt(variance);

                const double SIGMA_COEF = 3.0;
                var neededTimePerWidth = mean + SIGMA_COEF * stddev;
                var beamWidth = Max(minBeamWidth, Min(maxBeamWidth, (int)(remainingTime / neededTimePerWidth)));

                return beamWidth;
            }
            public int Suggest()
            {
                if (currentTurn >= warmupTurn)
                {
                    var elapsed = (DateTime.Now - lastTime).TotalSeconds;
                    var elapsedPerBeam = elapsed / currentBeamWidth;
                    UpdateState();
                    UpdateDistribution(elapsedPerBeam);
                }

                lastTime = DateTime.Now;
                var beamWidth = CalcSafeBeamWidth();
                currentBeamWidth = beamWidth;
                ++currentTurn;
                return beamWidth;
            }
        }

        public string[] CalcMax(HeuristicStateFullInternal state, int width, int maxTurn)
        {
            return CalcMax(state, width, width, -1, maxTurn);
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public string[] CalcMax(HeuristicStateFullInternal state, int initialWidth, int maxWidth, int totalMillis, int maxTurn)
        {
            var q = new LIB_IntervalQueue<(long, int, HeuristicStateFullInternal)>[maxTurn + 1];
            var queueAliveCounter = new int[maxTurn + 1];
            var queueIdCounter = new int[maxTurn + 1];
            var usedHash = new LIB_Dictionary<long, (long, int)>[maxTurn + 1];
            var deletedId = new LIB_HashSet<int>[maxTurn + 1];
            for (var i = 0; i <= maxTurn; ++i)
            {
                q[i] = new LIB_IntervalQueue<(long, int, HeuristicStateFullInternal)>();
                usedHash[i] = new LIB_Dictionary<long, (long, int)>();
                deletedId[i] = new LIB_HashSet<int>();
            }

            state.Initialize((s, str) =>
            {
                var ope = new LIB_OperatorInitialFull();
                ope.operateString = str;
                s.thisOperator = ope;
                q[0].Push((0, ++queueIdCounter[0], s));
                ++queueAliveCounter[0];
            });

            var beamWidthSuggester = new BayesianBeamWidthSuggester(maxTurn, (int)(maxTurn * 0.05) + 1, totalMillis / 1000.0, initialWidth, 1, maxWidth);

            for (var i = 0; i < maxTurn; ++i)
            {
                var width = Min(beamWidthSuggester.Suggest(), queueAliveCounter[i]);
                Console.Error.WriteLine($"Turn {i} : {width}");

                for (var j = 0; j < width; ++j)
                {
                    var poped = q[i].PopMax();
                    while (deletedId[i].Contains(poped.Item2))
                    {
                        poped.Item3.thisOperator.Unuse();
                        poped.Item3.Unuse();
                        poped = q[i].PopMax();
                    }
                    --queueAliveCounter[i];
                    var thisState = poped.Item3;
                    var actions = thisState.ListupActions(i);

                    thisState.thisOperator.deleteBlock = true;
                    foreach (var item in actions)
                    {
                        item.SetParent(thisState.thisOperator);

                        var nextState = thisState.Clone();
                        nextState.DoAction(item, i, out var nextTurn, out var score, out var hash);

                        if (usedHash[nextTurn].TryGetValue(hash, out var oldVal))
                        {
                            if (oldVal.Item1 >= score)
                            {
                                item.Unuse();
                                nextState.Unuse();
                                continue;
                            }

                            var newId = ++queueIdCounter[nextTurn];
                            usedHash[nextTurn][hash] = (score, newId);
                            q[nextTurn].Push((score, newId, nextState));
                            deletedId[nextTurn].Add(oldVal.Item2);
                        }
                        else
                        {
                            var newId = ++queueIdCounter[nextTurn];
                            usedHash[nextTurn].Add(hash, (score, newId));
                            q[nextTurn].Push((score, newId, nextState));
                            ++queueAliveCounter[nextTurn];
                        }

                        if (queueAliveCounter[nextTurn] > maxWidth)
                        {
                            var minPoped = q[nextTurn].PopMin();
                            while (deletedId[nextTurn].Contains(minPoped.Item2))
                            {
                                minPoped.Item3.thisOperator.Unuse();
                                minPoped.Item3.Unuse();
                                minPoped = q[nextTurn].PopMin();
                            }
                            --queueAliveCounter[nextTurn];
                            var delState = minPoped.Item3;
                            delState.thisOperator.Unuse();
                            delState.Unuse();
                        }
                    }
                    thisState.thisOperator.deleteBlock = false;
                    if (thisState.thisOperator.refCount == 0) thisState.thisOperator.Unuse();
                    thisState.Unuse();
                }
            }

            if (queueAliveCounter[maxTurn] == 0) return new string[0];

            var maxPoped = q[maxTurn].PopMax();
            while (deletedId[maxTurn].Contains(maxPoped.Item2)) maxPoped = q[maxTurn].PopMax();
            var maxOperation = maxPoped.Item3.thisOperator;

            var answer = new List<string>();
            while (maxOperation != null)
            {
                answer.Add(maxOperation.GetOperateString());
                maxOperation = maxOperation.parent;
            }

            answer.Reverse();

            return answer.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BeamSearchFullState()
        {
        }
    }
    ////end
}