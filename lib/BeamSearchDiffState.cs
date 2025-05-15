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
    // use LIB_HeuristicStateDiffBase
    class LIB_BeamSearchDiffState
    {
        struct Node
        {
            public int parent;
            public int child;
            public int prev;
            public int next;
            public (int l, int r) patch;
            public LIB_OperatorBase ope;
            public bool deleted;
            public int depth;
        }

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

        LIB_Deque<int> waitingReUse = new LIB_Deque<int>();
        Node[] nodeList = new Node[1000000];
        int nodeCount = 0;
        int root;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int NewNode()
        {
            // waitingReUse に削除されたノードが格納されており、これを使い回す
            // nodeList のできるだけ近い位置のノードを使うことでキャッシュヒット率を上げる意図があります（上がるのかは不明）
            if (waitingReUse.Count == 0) return ++nodeCount;
            return waitingReUse.PopBack();
        }

        void Remove(int nodeIdx)
        {
            // ノードを削除します
            // ノードの削除によって親ノードの子が無くなった場合は、親ノードも削除します（再帰的に）
            waitingReUse.PushBack(nodeIdx);
            ref var node = ref nodeList[nodeIdx];
            node.ope.Unuse();
            node.deleted = true;
            HeuristicStateDiffInternal.DeleteHistory(node.patch);
            if (node.prev == 0 && node.next == 0)
            {
                Remove(node.parent);
            }
            else if (node.prev == 0)
            {
                nodeList[node.parent].child = node.next;
                nodeList[node.next].prev = 0;
            }
            else if (node.next == 0)
            {
                nodeList[node.prev].next = 0;
            }
            else
            {
                nodeList[node.prev].next = node.next;
                nodeList[node.next].prev = node.prev;
            }
        }

        public string[] Run(HeuristicStateDiffInternal state, int width, int maxTurn)
        {
            return Run(state, width, -1, maxTurn, true);
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public string[] Run(HeuristicStateDiffInternal state, int initialWidth, int totalMillis, int maxTurn, bool fixHaba = false, int[] widthList = null)
        {
            // 状態を木構造で持ちます
            // Node 構造体で1つの要素を表現
            // Node の実体はリストで管理して、親や子などの情報はインデックス番号で保持します
            // 0番目の要素は番兵です（nullチェック回避のため）
            //
            // Node は以下の情報を持ちます
            // parent - 親の Node のインデックス
            // child - 一番左の子のインデックス
            // next - 右の兄弟のインデックス
            // prev - 左の兄弟のインデックス
            // patch - この Node で行った操作の履歴
            // action - この Node で行った操作（最終的に出力される文字列）
            //
            // Node は操作の履歴と親兄弟の情報のみ保持して、状態自体は継承先のクラスが保持する方針

            // 確定済みの操作を保存するためのリスト
            var answer = new List<string>();

            // 状態の初期化
            // 継承先のクラスで実装する
            state.Initialize();

            // 初期状態を表す Node を作成
            root = NewNode();
            nodeList[root].deleted = false;
            HeuristicStateDiffInternal.Batch();

            var width = initialWidth;
            ref var nodeListRef = ref nodeList[0];
            var startTime = DateTime.Now;
            var beforeTurnTime = startTime;
            var weightedAverageWidthTurnTime = 0.0;
            var tenPercentOfTurn = maxTurn / 10;
            var usedHash = new Dictionary<long, (int que, long score)>();
            var deleteNodeList = new HashSet<int>();
            var trueQueueCount = 0;
            var nextQueue = new LIB_PriorityQueue(); // このキューは最小要素を取り出す＝スコアの最大化になる（途中要らないものを Pop すると最小のものから削除されるので）
            var maxScore = long.MinValue;
            var maxAns = new List<string>();
            Func<int, bool, string[]> calcAnswer = (maxNode, doOperate) =>
            {
                var ret = answer.ToList();
                var backwardNodeList = new List<int>();
                var tmpmaxNode = maxNode;
                while (tmpmaxNode != root)
                {
                    backwardNodeList.Add(tmpmaxNode);
                    tmpmaxNode = nodeList[tmpmaxNode].parent;
                }
                backwardNodeList.Reverse();
                foreach (var item in backwardNodeList)
                {
                    if (doOperate) HeuristicStateDiffInternal.Apply(nodeList[item].patch);
                    //Console.Error.WriteLine($"kakutei depth:{nodeList[item].depth}");
                    ret.Add(nodeList[item].ope.GetOperateString());
                }
                return ret.ToArray();
            };
            var beamWidthSuggester = new BayesianBeamWidthSuggester(maxTurn, (int)(maxTurn * 0.05) + 1, totalMillis / 1000.0, initialWidth, 1, initialWidth * 3);
            for (var i = 0; i < maxTurn; ++i)
            {
                if (widthList != null) width = widthList[i];

                if ((i & 1) != 0) Console.Error.WriteLine($"turn: {i} width:{width} score: {nextQueue.Peek.Key} hashCount:{usedHash.Count}");

                usedHash.Clear();
                trueQueueCount = 0;

                var turnStartTime = DateTime.Now;
                // elapsed - 経過時間
                // lastTime - 残り時間
                var elapsed = (turnStartTime - startTime).TotalMilliseconds;
                var lastTime = totalMillis - elapsed;
                //if (totalMillis >= 0 && lastTime < 0) break;
                if (!fixHaba)
                {
                    // 残り時間に応じて幅を調整します
                    //var widthTurnTime = (turnStartTime - beforeTurnTime).TotalMilliseconds / width;
                    //if (i == 1) weightedAverageWidthTurnTime = widthTurnTime;
                    //else weightedAverageWidthTurnTime = weightedAverageWidthTurnTime * 0.9 + widthTurnTime * 0.1;
                    //if (i >= tenPercentOfTurn) width = Min(initialWidth * 2, Max(1, (int)(lastTime / (maxTurn - i) / weightedAverageWidthTurnTime)));
                    //beforeTurnTime = turnStartTime;
                    width = beamWidthSuggester.Suggest();
                }

                // キューを空にします
                // このキューは、1ターンの処理の途中で幅を超える要素を削除するために使います
                // 木には生きているノードしかおらず、毎ターン木の root から走査するので、
                // キューは問答無用で空にする
                nextQueue.Reset();

                var nodeIdx = root; // 今見ているノードのインデックス
                while (true)
                {
                    // 一番左の子ノードに移動できるだけ移動する
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).child != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).child;
                        HeuristicStateDiffInternal.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }
                    // ここで nodeIdx は一番左の葉ノードになっている

                    // needRemoveIdx は、子が一つも無くなったらノードを削除するために使われます
                    // 以下 foreach の中で、子の要素が全て usedHash に含まれている場合に子が一つも無くなる
                    var needRemoveIdx = nodeIdx;

                    // beforeNodeIdx は兄弟ノードをつなぐために使われます
                    var beforeNodeIdx = 0;
                    //Debug();

                    if (nodeList[nodeIdx].depth == i)
                    {
                        // ListupActions で可能な操作を列挙し、操作ごとに子を生やします
                        // ハッシュが usedHash に含まれている（過去と同一の盤面）なら、その操作はスキップします
                        foreach (var ope in state.ListupActions(i))
                        {
                            // DoAction で操作（順遷移）を行う
                            var score = state.DoAction(ope, i);
                            (int que, long score) oldValue;
                            if (usedHash.TryGetValue(score.hash, out oldValue))
                            {
                                if (!nodeList[oldValue.que].deleted)
                                {
                                    if (oldValue.score >= score.score)
                                    {
                                        ope.Unuse();
                                        var noUseHistory = HeuristicStateDiffInternal.Batch();
                                        HeuristicStateDiffInternal.Rollback(noUseHistory);
                                        HeuristicStateDiffInternal.DeleteHistory(noUseHistory);
                                        continue;
                                    }
                                    --trueQueueCount;
                                    deleteNodeList.Add(oldValue.que);
                                }
                            }
                            var newNodeIdx = NewNode();
                            nodeList[newNodeIdx].deleted = false;
                            usedHash[score.hash] = (newNodeIdx, score.score);
                            needRemoveIdx = 0; // 有効な子がいたので、親を削除しないようにする
                            ref var node = ref Unsafe.Add(ref nodeListRef, newNodeIdx);
                            node.child = 0;
                            node.prev = 0;
                            node.next = 0;
                            node.parent = nodeIdx;
                            node.depth = nodeList[nodeIdx].depth + 1;
                            node.patch = HeuristicStateDiffInternal.Batch();
                            node.ope = ope;
                            if (beforeNodeIdx != 0)
                            {
                                // 兄ノードがある場合
                                node.prev = beforeNodeIdx;
                                Unsafe.Add(ref nodeListRef, beforeNodeIdx).next = newNodeIdx;
                            }
                            else
                            {
                                // 兄ノードがいない（newNodeIdx が一番左の子）なら、親とつなぐ
                                Unsafe.Add(ref nodeListRef, nodeIdx).child = newNodeIdx;
                            }

                            beforeNodeIdx = newNodeIdx;
                            nextQueue.Push(score.score, newNodeIdx);
                            ++trueQueueCount;

                            HeuristicStateDiffInternal.Rollback(node.patch);

                            if (false)//maxScore < score.score)
                            {
                                maxScore = score.score;
                                maxAns = calcAnswer(newNodeIdx, false).ToList();
                            }
                        }
                    }

                    // なんかのケースで 0 番が汚染されていたことがあったので、ここでリセット
                    Unsafe.Add(ref nodeListRef, 0).prev = 0;
                    Unsafe.Add(ref nodeListRef, 0).child = 0;

                    // 木上を移動します
                    HeuristicStateDiffInternal.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    // next == 0、つまり右の兄弟がいない場合は親に移動します
                    while (Unsafe.Add(ref nodeListRef, nodeIdx).next == 0 && Unsafe.Add(ref nodeListRef, nodeIdx).parent != 0)
                    {
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).parent;
                        HeuristicStateDiffInternal.Rollback(Unsafe.Add(ref nodeListRef, nodeIdx).patch);
                    }

                    // 右の兄弟がいない（root に戻った）場合は終了
                    if (Unsafe.Add(ref nodeListRef, nodeIdx).next == 0)
                    {
                        // 有効な要素数を width に制限する
                        foreach (var item in deleteNodeList) Remove(item);
                        deleteNodeList.Clear();
                        while (trueQueueCount > width)
                        {
                            var ni = nextQueue.Pop().Value;
                            if (!nodeList[ni].deleted)
                            {
                                Remove(ni);
                                --trueQueueCount;
                            }
                        }
                        break;
                    }
                    else
                    {
                        // 右の兄弟に移動します
                        nodeIdx = Unsafe.Add(ref nodeListRef, nodeIdx).next;
                        HeuristicStateDiffInternal.Apply(Unsafe.Add(ref nodeListRef, nodeIdx).patch);

                        // 有効な要素数を width に制限する
                        foreach (var item in deleteNodeList) Remove(item);
                        deleteNodeList.Clear();
                        while (trueQueueCount > width)
                        {
                            var ni = nextQueue.Pop().Value;
                            if (!nodeList[ni].deleted)
                            {
                                Remove(ni);
                                --trueQueueCount;
                            }
                        }
                    }

                    // needRemoveIdx > 0 なら、それは子が一つもないノードなので、ここで削除
                    if (needRemoveIdx > 0) Remove(needRemoveIdx);
                }

                // root の子がただ一つなら、それを root に昇格させる
                // 旧 root は確定操作として answer に追加
                while (Unsafe.Add(ref nodeListRef, root).child != 0 && Unsafe.Add(ref nodeListRef, Unsafe.Add(ref nodeListRef, root).child).next == 0)
                {
                    root = Unsafe.Add(ref nodeListRef, root).child;
                    //Console.Error.WriteLine($"kakutei depth:{nodeList[root].depth}");
                    answer.Add(Unsafe.Add(ref nodeListRef, root).ope.GetOperateString());
                    HeuristicStateDiffInternal.Apply(Unsafe.Add(ref nodeListRef, root).patch);
                    HeuristicStateDiffInternal.DeleteHistory(Unsafe.Add(ref nodeListRef, root).patch);
                    Unsafe.Add(ref nodeListRef, root).patch = (0, 0);
                    Unsafe.Add(ref nodeListRef, root).parent = 0;
                }
            }

            // nextQueue から最後に取れる要素＝最大スコアの要素を取り出します
            var maxNode = 0;
            var maxv = 0L;
            while (trueQueueCount > 0)
            {
                var pop = nextQueue.Pop();
                if (!nodeList[pop.Value].deleted)
                {
                    maxNode = pop.Value;
                    maxv = pop.Key;
                    --trueQueueCount;
                }
            }
            //Console.Error.WriteLine($"lastScore: {maxv}");

            // 最大スコアの要素から親を辿っていき、操作の履歴を answer に追加します
            if (maxScore < maxv)
            {
                maxAns = calcAnswer(maxNode, true).ToList();
            }
            /*
            var backwardNodeList = new List<int>();
            while (maxNode != root)
            {
                backwardNodeList.Add(maxNode);
                maxNode = nodeList[maxNode].parent;
            }
            backwardNodeList.Reverse();
            foreach (var item in backwardNodeList)
            {
                answer.Add(nodeList[item].ope.GetOperateString());
            }

            return answer.ToArray();
            */

            return maxAns.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BeamSearchDiffState()
        {
        }
    }
    ////end
}