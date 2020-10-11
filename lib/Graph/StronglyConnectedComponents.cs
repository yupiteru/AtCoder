
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
            Span<int> start = stackalloc int[n + 1];
            ref int startref = ref start[0];
            Span<int> elist = stackalloc int[edgeCnt];
            ref int elistref = ref elist[0];
            ref long edgesref = ref edges[0];
            for (var i = 0; i < edgeCnt; ++i)
            {
                ++Unsafe.Add(ref startref, (int)((Unsafe.Add(ref edgesref, i) >> 30) + 1));
            }
            var beforeStart = startref;
            for (var i = 1; i <= n; ++i)
            {
                ref int thisStartRef = ref Unsafe.Add(ref startref, i);
                thisStartRef += beforeStart;
                beforeStart = thisStartRef;
            }
            var counter = new int[n + 1];
            ref int counterref = ref counter[0];
            Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref counterref), ref Unsafe.As<int, byte>(ref startref), (uint)(4 * (n + 1)));
            for (var i = 0; i < edgeCnt; ++i)
            {
                var e = Unsafe.Add(ref edgesref, i);
                Unsafe.Add(ref elistref, Unsafe.Add(ref counterref, (int)(e >> 30))++) = (int)(e & 1073741823);
            }
            var nowOrd = -1;
            var groupNum = 0;
            var visitedIdx = -1;
            Span<int> visited = stackalloc int[n];
            ref int visitedref = ref visited[0];
            Span<int> low = stackalloc int[n];
            ref int lowref = ref low[0];
            Span<int> ord = stackalloc int[n];
            ref int ordref = ref ord[0];
            var ids = new int[n];
            ref int idsref = ref ids[0];
            Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref ordref), 255, (uint)(4 * n));
            void dfs(int v, ref int lowref, ref int ordref, ref int visitedref, ref int startref, ref int elistref, ref int idsref)
            {
                ref int lowv = ref Unsafe.Add(ref lowref, v);
                lowv = Unsafe.Add(ref ordref, v) = ++nowOrd;
                Unsafe.Add(ref visitedref, ++visitedIdx) = v;
                var endv = Unsafe.Add(ref startref, v + 1);
                for (var i = Unsafe.Add(ref startref, v); i < endv; ++i)
                {
                    var to = Unsafe.Add(ref elistref, i);
                    ref int ordto = ref Unsafe.Add(ref ordref, to);
                    if (ordto == -1)
                    {
                        dfs(to, ref lowref, ref ordref, ref visitedref, ref startref, ref elistref, ref idsref);
                        lowv = Math.Min(lowv, Unsafe.Add(ref lowref, to));
                    }
                    else lowv = Math.Min(lowv, ordto);
                }
                if (lowv == Unsafe.Add(ref ordref, v))
                {
                    while (true)
                    {
                        var u = Unsafe.Add(ref visitedref, visitedIdx);
                        --visitedIdx;
                        Unsafe.Add(ref ordref, u) = n;
                        Unsafe.Add(ref idsref, u) = groupNum;
                        if (u == v) break;
                    }
                    ++groupNum;
                }
            }
            for (var i = 0; i < n; ++i)
            {
                if (Unsafe.Add(ref ordref, i) == -1) dfs(i, ref lowref, ref ordref, ref visitedref, ref startref, ref elistref, ref idsref);
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