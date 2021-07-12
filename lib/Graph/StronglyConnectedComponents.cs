
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
    class LIB_StronglyConnectedComponents
    {
        int n;
        long[] edges;
        int edgeCnt;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_StronglyConnectedComponents(long n)
        {
            this.n = (int)n;
            edges = new long[8];
            edgeCnt = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long from, long to)
        {
            ref long edgesref = ref edges[0];
            Unsafe.Add(ref edgesref, edgeCnt) = (from << 30) | to;
            if (++edgeCnt == edges.Length)
            {
                var tmp = new long[edgeCnt << 1];
                ref long tmpref = ref tmp[0];
                Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmpref), ref Unsafe.As<long, byte>(ref edgesref), (uint)(8 * edgeCnt));
                edges = tmp;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected (int, int[]) SCCIDs()
        {
            if (edgeCnt == 0) return (0, new int[0]);
            Span<int> tmp = stackalloc int[(n << 2) + 1 + edgeCnt];
            var startoff = 0;
            var visitedoff = startoff + n + 1;
            var lowoff = visitedoff + n;
            var ordoff = lowoff + n;
            var elistoff = ordoff + n;
            ref int tmpref = ref tmp[0];
            ref long edgesref = ref edges[0];
            for (var i = 0; i < edgeCnt; ++i)
            {
                ++Unsafe.Add(ref tmpref, startoff + (int)((Unsafe.Add(ref edgesref, i) >> 30) + 1));
            }
            var beforeStart = tmpref;
            for (var i = 1; i <= n; ++i)
            {
                ref int thisStartRef = ref Unsafe.Add(ref tmpref, startoff + i);
                thisStartRef += beforeStart;
                beforeStart = thisStartRef;
            }
            var counter = new int[n + 1];
            ref int counterref = ref counter[0];
            Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref counterref), ref Unsafe.As<int, byte>(ref Unsafe.Add(ref tmpref, startoff)), (uint)(4 * (n + 1)));
            for (var i = 0; i < edgeCnt; ++i)
            {
                var e = Unsafe.Add(ref edgesref, i);
                Unsafe.Add(ref tmpref, elistoff + Unsafe.Add(ref counterref, (int)(e >> 30))++) = (int)(e & 1073741823);
            }
            var nowOrd = -1;
            var groupNum = 0;
            var visitedIdx = -1;
            var ids = new int[n];
            ref int idsref = ref ids[0];
            Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref tmpref, ordoff)), 255, (uint)(4 * n));
            void dfs(int v, ref int tmpref, ref int idsref)
            {
                ref int lowv = ref Unsafe.Add(ref tmpref, lowoff + v);
                lowv = Unsafe.Add(ref tmpref, ordoff + v) = ++nowOrd;
                Unsafe.Add(ref tmpref, visitedoff + ++visitedIdx) = v;
                var endv = Unsafe.Add(ref tmpref, startoff + v + 1);
                for (var i = Unsafe.Add(ref tmpref, startoff + v); i < endv; ++i)
                {
                    var to = Unsafe.Add(ref tmpref, elistoff + i);
                    ref int ordto = ref Unsafe.Add(ref tmpref, ordoff + to);
                    if (ordto == -1)
                    {
                        dfs(to, ref tmpref, ref idsref);
                        lowv = Math.Min(lowv, Unsafe.Add(ref tmpref, lowoff + to));
                    }
                    else lowv = Math.Min(lowv, ordto);
                }
                if (lowv == Unsafe.Add(ref tmpref, ordoff + v))
                {
                    while (true)
                    {
                        var u = Unsafe.Add(ref tmpref, visitedoff + visitedIdx);
                        --visitedIdx;
                        Unsafe.Add(ref tmpref, ordoff + u) = n;
                        Unsafe.Add(ref idsref, u) = groupNum;
                        if (u == v) break;
                    }
                    ++groupNum;
                }
            }
            for (var i = 0; i < n; ++i)
            {
                if (Unsafe.Add(ref tmpref, ordoff + i) == -1) dfs(i, ref tmpref, ref idsref);
            }
            for (var i = 0; i < n; i++)
            {
                ref int idsi = ref Unsafe.Add(ref idsref, i);
                idsi = groupNum - 1 - idsi;
            }
            return (groupNum, ids);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[][] SCC()
        {
            var ids = SCCIDs();
            var idsItem1 = ids.Item1;
            if (idsItem1 == 0) return new int[0][];
            Span<int> counts = stackalloc int[idsItem1];
            ref int countsref = ref counts[0];
            var groups = new int[idsItem1][];
            ref int[] groupsref = ref groups[0];
            var sz = Unsafe.SizeOf<int[]>();
            ref int idsItem2Ref = ref ids.Item2[0];
            var idsItem2Cnt = ids.Item2.Length;
            for (var i = 0; i < idsItem2Cnt; ++i) ++Unsafe.Add(ref countsref, Unsafe.Add(ref idsItem2Ref, i));
            for (var i = 0; i < idsItem1; ++i) Unsafe.Add(ref groupsref, i) = new int[Unsafe.Add(ref countsref, i)];
            for (var i = 0; i < idsItem2Cnt; ++i)
            {
                var v = Unsafe.Add(ref idsItem2Ref, i);
                Unsafe.Add(ref groupsref, v)[--Unsafe.Add(ref countsref, v)] = i;
            }
            return groups;
        }
    }
    ////end
}