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
    class LIB_ParallelNibunTansaku
    {
        long maxForwardCount;
        long queryCount;
        Func<long, bool> checker;
        Action initialize;
        Action forward;

        public LIB_ParallelNibunTansaku()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ParallelNibunTansaku SetMaxForwardCount(long maxForwardCount)
        {
            this.maxForwardCount = maxForwardCount;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ParallelNibunTansaku SetChecker(Func<long, bool> checker)
        {
            this.checker = checker;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ParallelNibunTansaku SetInitialize(Action initialize)
        {
            this.initialize = initialize;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ParallelNibunTansaku SetForward(Action forward)
        {
            this.forward = forward;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ParallelNibunTansaku SetQueryCount(long queryCount)
        {
            this.queryCount = queryCount;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long[] Calc()
        {
            var leftList = new long[queryCount];
            var rightList = new long[queryCount];
            var posToQuery = Enumerable.Repeat(0, (int)maxForwardCount + 1).Select(_ => new List<long>()).ToArray();
            var posToQueryNext = Enumerable.Repeat(0, (int)maxForwardCount + 1).Select(_ => new List<long>()).ToArray();
            leftList.Fill(-1);
            rightList.Fill(maxForwardCount + 1);
            for (var i = 0; i < queryCount; ++i) posToQuery[(rightList[i] + leftList[i]) / 2].Add(i);

            while (true)
            {
                initialize();

                var finished = true;

                Action<long> checkAll = (long idx) =>
                {
                    foreach (var item in posToQuery[idx])
                    {
                        if (checker(item)) rightList[item] = idx;
                        else leftList[item] = idx;
                        if (rightList[item] - leftList[item] > 1)
                        {
                            finished = false;
                            posToQueryNext[(rightList[item] + leftList[item]) / 2].Add(item);
                        }
                    }
                    posToQuery[idx].Clear();
                };

                for (var i = 0; i < maxForwardCount; ++i)
                {
                    checkAll(i);
                    forward();
                }
                checkAll(maxForwardCount);

                (posToQuery, posToQueryNext) = (posToQueryNext, posToQuery);

                if (finished) break;
            }

            return rightList.ToArray();
        }
    }
    ////end
}