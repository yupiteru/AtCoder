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
    class LIB_MinCostFlow
    {
        public struct Edge
        {
            public int from;
            public int to;
            public long cap;
            public long flow;
            public long cost;
        }
        public struct CapCost
        {
            public long cap;
            public long cost;
        }
        const int SHIFT_SIZE = 30;
        const int MASK = 1073741823;
        List<ulong> pos;
        int[][] gTo;
        int[][] gRev;
        long[][] gCap;
        long[][] gCost;
        int[] gTolen;
        int[] gRevlen;
        int[] gCaplen;
        int[] gCostlen;
        int N;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_MinCostFlow(long n)
        {
            N = (int)n;
            pos = new List<ulong>();
            gTo = new int[n][];
            gRev = new int[n][];
            gCap = new long[n][];
            gCost = new long[n][];
            gTolen = new int[n];
            gRevlen = new int[n];
            gCaplen = new int[n];
            gCostlen = new int[n];
            for (var i = 0; i < n; i++)
            {
                gTo[i] = new int[8];
                gRev[i] = new int[8];
                gCap[i] = new long[8];
                gCost[i] = new long[8];
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddEdge(long from, long to, long cap, long cost)
        {
            var ret = pos.Count;
            pos.Add(((ulong)from << SHIFT_SIZE) | (uint)gTolen[from]);
            if (gTo[from].Length == gTolen[from])
            {
                var tmp = new int[gTolen[from] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gTo[from][0]), (uint)(gTolen[from] << 2));
                gTo[from] = tmp;
                tmp = new int[gRevlen[from] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gRev[from][0]), (uint)(gRevlen[from] << 2));
                gRev[from] = tmp;
                var tmp2 = new long[gCaplen[from] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp2[0]), ref Unsafe.As<long, byte>(ref gCap[from][0]), (uint)(gCaplen[from] << 3));
                gCap[from] = tmp2;
                tmp2 = new long[gCostlen[from] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp2[0]), ref Unsafe.As<long, byte>(ref gCost[from][0]), (uint)(gCostlen[from] << 3));
                gCost[from] = tmp2;
            }
            if (gTo[to].Length == gTolen[to])
            {
                var tmp = new int[gTolen[to] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gTo[to][0]), (uint)(gTolen[to] << 2));
                gTo[to] = tmp;
                tmp = new int[gRevlen[to] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gRev[to][0]), (uint)(gRevlen[to] << 2));
                gRev[to] = tmp;
                var tmp2 = new long[gCaplen[to] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp2[0]), ref Unsafe.As<long, byte>(ref gCap[to][0]), (uint)(gCaplen[to] << 3));
                gCap[to] = tmp2;
                tmp2 = new long[gCostlen[to] << 1];
                Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp2[0]), ref Unsafe.As<long, byte>(ref gCost[to][0]), (uint)(gCostlen[to] << 3));
                gCost[to] = tmp2;
            }
            gTo[from][gTolen[from]++] = (int)to;
            gRev[from][gRevlen[from]++] = gRevlen[to];
            gCap[from][gCaplen[from]++] = cap;
            gCost[from][gCostlen[from]++] = cost;
            gTo[to][gTolen[to]++] = (int)from;
            gRev[to][gRevlen[to]++] = gRevlen[from] - 1;
            gCap[to][gCaplen[to]++] = 0;
            gCost[to][gCostlen[to]++] = -cost;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Edge GetEdge(long i)
        {
            var posidx = pos[(int)i];
            var idxf = posidx >> SHIFT_SIZE;
            var idxt = (int)(posidx & MASK);
            var eTo = gTo[idxf][idxt];
            var eRev = gRev[idxf][idxt];
            var reCap = gCap[eTo][eRev];
            return new Edge { from = (int)(idxf), to = eTo, cap = gCap[idxf][idxt] + reCap, flow = reCap, cost = gCost[idxf][idxt] };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Edge[] GetAllEdge()
        {
            var res = new Edge[pos.Count];
            for (var i = 0; i < res.Length; ++i) res[i] = GetEdge(i);
            return res;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CapCost Flow(long s, long t) => Flow(s, t, long.MaxValue >> 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CapCost Flow(long s, long t, long flowLimit) => Slope(s, t, flowLimit).Last();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<CapCost> Slope(long s, long t) => Slope(s, t, long.MaxValue >> 2);
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public List<CapCost> Slope(long s, long t, long flowLimit)
        {
            var tmp1 = new long[N << 1];
            var tmp2 = new int[N];
            var tmp3 = new int[N];
            ref var dualref = ref tmp1[0];
            ref var distref = ref tmp1[N];
            ref var pvref = ref tmp2[0];
            ref var peref = ref tmp3[0];
            var flow = 0L;
            var cost = 0L;
            var prevCost = -1L;
            var result = new List<CapCost>();
            result.Add(new CapCost { cap = flow, cost = cost });
            var vis = new bool[N];
            ref var visref = ref vis[0];
            while (flow < flowLimit)
            {
                Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref pvref), 0xFF, (uint)(N << 2));
                Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref peref), 0xFF, (uint)(N << 2));
                Unsafe.InitBlock(ref Unsafe.As<long, byte>(ref distref), 0x7F, (uint)(N << 3));
                Unsafe.InitBlock(ref Unsafe.As<bool, byte>(ref visref), 0, (uint)N);
                var que = new LIB_PriorityQueue();
                Unsafe.Add(ref distref, (int)s) = 0;
                que.Push(0, (int)s);
                while (que.Count > 0)
                {
                    var v = que.Pop().Value;
                    if (Unsafe.Add(ref visref, v)) continue;
                    Unsafe.Add(ref visref, v) = true;
                    if (v == t) break;
                    var gToV = gTo[v];
                    var gToVlen = gTolen[v];
                    var gCapV = gCap[v];
                    var gCostV = gCost[v];
                    var dualv = Unsafe.Add(ref dualref, v);
                    var distv = Unsafe.Add(ref distref, v);
                    for (var i = 0; i < gToVlen; ++i)
                    {
                        var eTo = gToV[i];
                        if (Unsafe.Add(ref visref, eTo) || gCapV[i] == 0) continue;
                        var thisCost = gCostV[i] - Unsafe.Add(ref dualref, eTo) + dualv;
                        if (Unsafe.Add(ref distref, eTo) - distv > thisCost)
                        {
                            Unsafe.Add(ref distref, eTo) = distv + thisCost;
                            Unsafe.Add(ref pvref, eTo) = v;
                            Unsafe.Add(ref peref, eTo) = i;
                            que.Push(Unsafe.Add(ref distref, eTo), eTo);
                        }
                    }
                }
                if (!Unsafe.Add(ref visref, (int)t)) break;
                for (var v = 0; v < vis.Length; ++v)
                {
                    if (!Unsafe.Add(ref visref, v)) continue;
                    Unsafe.Add(ref dualref, v) -= Unsafe.Add(ref distref, (int)t) - Unsafe.Add(ref distref, v);
                }
                var c = flowLimit - flow;
                for (var v = (int)t; v != s; v = Unsafe.Add(ref pvref, v)) c = Math.Min(c, gCap[Unsafe.Add(ref pvref, (int)v)][Unsafe.Add(ref peref, v)]);
                for (var v = (int)t; v != s; v = Unsafe.Add(ref pvref, v))
                {
                    gCap[Unsafe.Add(ref pvref, (int)v)][Unsafe.Add(ref peref, v)] -= c;
                    gCap[v][gRev[Unsafe.Add(ref pvref, (int)v)][Unsafe.Add(ref peref, v)]] += c;
                }
                var d = -Unsafe.Add(ref dualref, (int)s);
                flow += c;
                cost += c * d;
                if (prevCost == d) result.RemoveAt(result.Count - 1);
                result.Add(new CapCost { cap = flow, cost = cost });
                prevCost = cost;
            }
            return result;
        }
    }
    ////end
}