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
        List<int>[] gTo;
        List<int>[] gRev;
        List<long>[] gCap;
        List<long>[] gCost;
        int N;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_MinCostFlow(long n)
        {
            N = (int)n;
            pos = new List<ulong>();
            gTo = new List<int>[n];
            gRev = new List<int>[n];
            gCap = new List<long>[n];
            gCost = new List<long>[n];
            for (var i = 0; i < n; i++)
            {
                gTo[i] = new List<int>();
                gRev[i] = new List<int>();
                gCap[i] = new List<long>();
                gCost[i] = new List<long>();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddEdge(long from, long to, long cap, long cost)
        {
            var ret = pos.Count;
            pos.Add(((ulong)from << SHIFT_SIZE) | (uint)gTo[from].Count);
            gTo[from].Add((int)to);
            gRev[from].Add(gRev[to].Count);
            gCap[from].Add(cap);
            gCost[from].Add(cost);
            gTo[to].Add((int)from);
            gRev[to].Add(gRev[from].Count - 1);
            gCap[to].Add(0);
            gCost[to].Add(-cost);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Edge GetEdge(long i)
        {
            var idx = (int)i;
            var idxf = pos[idx] >> SHIFT_SIZE;
            var idxt = (int)(pos[idx] & MASK);
            var eTo = gTo[idxf][idxt];
            var eRev = gRev[idxf][idxt];
            var reCap = gCap[eTo][eRev];
            return new Edge { from = (int)(pos[idx] >> SHIFT_SIZE), to = eTo, cap = gCap[idxf][idxt] + reCap, flow = reCap, cost = gCost[idxf][idxt] };
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<CapCost> Slope(long s, long t, long flowLimit)
        {
            var dual = new long[N];
            var dist = new long[N];
            var pv = new int[N];
            var pe = new int[N];
            var flow = 0L;
            var cost = 0L;
            var prevCost = -1L;
            var result = new List<CapCost>();
            result.Add(new CapCost { cap = flow, cost = cost });
            while (flow < flowLimit)
            {
                for (var i = 0; i < dist.Length; ++i) dist[i] = long.MaxValue >> 2;
                for (var i = 0; i < pv.Length; ++i) pv[i] = -1;
                for (var i = 0; i < pe.Length; ++i) pe[i] = -1;
                var vis = new bool[N];
                var que = new LIB_PriorityQueue<long, int>();
                dist[s] = 0;
                que.Push(0, (int)s);
                while (que.Count > 0)
                {
                    var v = que.Pop().Value;
                    if (vis[v]) continue;
                    vis[v] = true;
                    if (v == t) break;
                    var gToV = gTo[v];
                    var gCapV = gCap[v];
                    var gCostV = gCost[v];
                    var dualv = dual[v];
                    var distv = dist[v];
                    for (var i = 0; i < gToV.Count; ++i)
                    {
                        var eTo = gToV[i];
                        if (vis[eTo] || gCapV[i] == 0) continue;
                        var thisCost = gCostV[i] - dual[eTo] + dualv;
                        if (dist[eTo] - distv > thisCost)
                        {
                            dist[eTo] = distv + thisCost;
                            pv[eTo] = v;
                            pe[eTo] = i;
                            que.Push(dist[eTo], eTo);
                        }
                    }
                }
                if (!vis[t]) break;
                for (var v = 0; v < vis.Length; ++v)
                {
                    if (!vis[v]) continue;
                    dual[v] -= dist[t] - dist[v];
                }
                var c = flowLimit - flow;
                for (var v = t; v != s; v = pv[v]) c = Math.Min(c, gCap[pv[v]][pe[v]]);
                for (var v = t; v != s; v = pv[v])
                {
                    gCap[pv[v]][pe[v]] -= c;
                    gCap[v][gRev[pv[v]][pe[v]]] += c;
                }
                var d = -dual[s];
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