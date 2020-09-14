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
    class LIB_MaxFlow
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
        int N;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_MaxFlow(long n)
        {
            N = (int)n;
            pos = new List<ulong>();
            gTo = new List<int>[n];
            gRev = new List<int>[n];
            gCap = new List<long>[n];
            for (var i = 0; i < n; i++)
            {
                gTo[i] = new List<int>();
                gRev[i] = new List<int>();
                gCap[i] = new List<long>();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddEdge(long from, long to, long cap)
        {
            var ret = pos.Count;
            pos.Add(((ulong)from << SHIFT_SIZE) | (uint)gTo[from].Count);
            gTo[from].Add((int)to);
            gRev[from].Add(gRev[to].Count);
            gCap[from].Add(cap);
            gTo[to].Add((int)from);
            gRev[to].Add(gRev[from].Count - 1);
            gCap[to].Add(0);
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
            return new Edge { from = (int)(pos[idx] >> SHIFT_SIZE), to = eTo, cap = gCap[idxf][idxt] + reCap, flow = reCap };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Edge[] GetAllEdge()
        {
            var res = new Edge[pos.Count];
            for (var i = 0; i < res.Length; ++i) res[i] = GetEdge(i);
            return res;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ChangeEdge(long i, long newCap, long newFlow)
        {
            var idx = (int)i;
            var idxf = pos[idx] >> SHIFT_SIZE;
            var idxt = (int)(pos[idx] & MASK);
            gCap[idxf][idxt] = newCap - newFlow;
            gCap[gTo[idxf][idxt]][gRev[idxf][idxt]] = newFlow;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Flow(long s, long t) => Flow(s, t, long.MaxValue >> 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Flow(long s, long t, long flowLimit)
        {
            var level = new int[N];
            var iter = new int[N];
            var que = new Queue<int>();
            Func<int, long, long> dfs = null;
            dfs = (v, up) =>
            {
                var res = 0L;
                var levelv = level[v];
                var gToV = gTo[v];
                var gCapV = gCap[v];
                var gRevV = gRev[v];
                for (ref int i = ref iter[v]; i < gToV.Count; ++i)
                {
                    var gToVi = gToV[i];
                    var gRevVi = gRevV[i];
                    var gcap = gCap[gToVi][gRevVi];
                    if (levelv <= level[gToVi] || gcap == 0) continue;
                    var param = Math.Min(up - res, gcap);
                    var d = gToVi == s ? param : dfs(gToVi, param);
                    if (d <= 0) continue;
                    gCapV[i] += d;
                    gCap[gToVi][gRevVi] -= d;
                    res += d;
                    if (res == up) break;
                }
                return res;
            };
            var flow = 0L;
            while (flow < flowLimit)
            {
                for (var i = 0; i < level.Length; ++i) level[i] = -1;
                level[s] = 0;
                que.Enqueue((int)s);
                while (que.Count > 0)
                {
                    var v = que.Dequeue();
                    var gToV = gTo[v];
                    var gCapV = gCap[v];
                    var levelv = level[v] + 1;
                    for (var i = 0; i < gToV.Count; ++i)
                    {
                        var gToVi = gToV[i];
                        if (gCapV[i] == 0 || level[gToVi] >= 0) continue;
                        level[gToVi] = levelv;
                        if (gToVi == t)
                        {
                            que.Clear();
                            break;
                        }
                        que.Enqueue(gToVi);
                    }
                }
                if (level[t] == -1) break;
                iter = new int[N];
                while (flow < flowLimit)
                {
                    var f = t == s ? (flowLimit - flow) : dfs((int)t, flowLimit - flow);
                    if (f == 0) break;
                    flow += f;
                }
            }
            return flow;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool[] MinCut(long s)
        {
            var visited = new bool[N];
            var que = new Queue<int>();
            que.Enqueue((int)s);
            while (que.Count > 0)
            {
                var p = que.Dequeue();
                visited[p] = true;
                var gToP = gTo[p];
                var gCapP = gCap[p];
                for (var i = 0; i < gToP.Count; ++i)
                {
                    var gToPi = gToP[i];
                    if (gCapP[i] > 0 && !visited[gToPi])
                    {
                        visited[gToPi] = true;
                        que.Enqueue(gToPi);
                    }
                }
            }
            return visited;
        }
    }
    ////end
}