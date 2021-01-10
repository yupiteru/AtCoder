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
        }
        const int SHIFT_SIZE = 30;
        const int MASK = 1073741823;
        ulong[] pos;
        int posCnt;
        int[][] gTo;
        int[] gToCnt;
        int[][] gRev;
        int[] gRevCnt;
        long[][] gCap;
        int[] gCapCnt;
        int N;
        public LIB_MaxFlow(long n)
        {
            N = (int)n;
            pos = new ulong[16];
            gTo = new int[n][];
            gRev = new int[n][];
            gCap = new long[n][];
            gToCnt = new int[n];
            gRevCnt = new int[n];
            gCapCnt = new int[n];
            ref var gToref = ref gTo[0];
            ref var gRevref = ref gRev[0];
            ref var gCapref = ref gCap[0];
            for (var i = 0; i < n; i++)
            {
                Unsafe.Add(ref gToref, i) = new int[16];
                Unsafe.Add(ref gRevref, i) = new int[16];
                Unsafe.Add(ref gCapref, i) = new long[16];
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int AddEdge(long from, long to, long cap)
        {
            pos[posCnt++] = ((ulong)from << SHIFT_SIZE) | (uint)gToCnt[from];
            gTo[from][gToCnt[from]++] = (int)to;
            gRev[from][gRevCnt[from]++] = gRevCnt[to];
            gCap[from][gCapCnt[from]++] = cap;
            gTo[to][gToCnt[to]++] = (int)from;
            gRev[to][gRevCnt[to]++] = gRevCnt[from] - 1;
            gCap[to][gCapCnt[to]++] = 0;
            if (posCnt == pos.Length)
            {
                var tmp = new ulong[posCnt << 1];
                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref tmp[0]), ref Unsafe.As<ulong, byte>(ref pos[0]), (uint)(8 * posCnt));
                pos = tmp;
            }
            if (gToCnt[from] == gTo[from].Length)
            {
                {
                    var tmp = new int[gToCnt[from] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gTo[from][0]), (uint)(4 * gToCnt[from]));
                    gTo[from] = tmp;
                }
                {
                    var tmp = new int[gRevCnt[from] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gRev[from][0]), (uint)(4 * gRevCnt[from]));
                    gRev[from] = tmp;
                }
                {
                    var tmp = new long[gCapCnt[from] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp[0]), ref Unsafe.As<long, byte>(ref gCap[from][0]), (uint)(8 * gCapCnt[from]));
                    gCap[from] = tmp;
                }
            }
            if (gToCnt[to] == gTo[to].Length)
            {
                {
                    var tmp = new int[gToCnt[to] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gTo[to][0]), (uint)(4 * gToCnt[to]));
                    gTo[to] = tmp;
                }
                {
                    var tmp = new int[gRevCnt[to] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp[0]), ref Unsafe.As<int, byte>(ref gRev[to][0]), (uint)(4 * gRevCnt[to]));
                    gRev[to] = tmp;
                }
                {
                    var tmp = new long[gCapCnt[to] << 1];
                    Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp[0]), ref Unsafe.As<long, byte>(ref gCap[to][0]), (uint)(8 * gCapCnt[to]));
                    gCap[to] = tmp;
                }
            }
            return posCnt - 1;
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
            var res = new Edge[posCnt];
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
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public long Flow(long s, long t, long flowLimit)
        {
            int si = (int)s;
            int ti = (int)t;
            var level = new int[N];
            var iter = new int[N];
            ref var iterref = ref iter[0];
            Func<int, long, long> dfs = null;
            dfs = (v, up) =>
            {
                var res = 0L;
                ref var levelref = ref level[0];
                ref var gToVref = ref gTo[v][0];
                ref var gCapref = ref gCap[0];
                ref var gCapVref = ref gCap[v][0];
                ref var gRevVref = ref gRev[v][0];
                var levelv = Unsafe.Add(ref levelref, v);
                ref var iterv = ref iter[v];
                var gToCntV = gToCnt[v];
                for (; iterv < gToCntV; ++iterv)
                {
                    var gToVi = Unsafe.Add(ref gToVref, iterv);
                    var gRevVi = Unsafe.Add(ref gRevVref, iterv);
                    var gcap = Unsafe.Add(ref gCapref, gToVi)[gRevVi];
                    if (levelv <= Unsafe.Add(ref levelref, gToVi) || gcap == 0) continue;
                    var param = Math.Min(up - res, gcap);
                    var d = gToVi == si ? param : dfs(gToVi, param);
                    if (d <= 0) continue;
                    Unsafe.Add(ref gCapVref, iterv) += d;
                    Unsafe.Add(ref gCapref, gToVi)[gRevVi] -= d;
                    res += d;
                    if (res == up) break;
                }
                return res;
            };
            var flow = 0L;
            ref var levelref = ref level[0];
            var que = new Queue<int>();
            while (flow < flowLimit)
            {
                Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref levelref), 255, (uint)N << 2);
                Unsafe.Add(ref levelref, si) = 0;
                que.Enqueue(si);
                while (que.Count > 0)
                {
                    var v = que.Dequeue();
                    ref var gToVref = ref gTo[v][0];
                    ref var gCapVref = ref gCap[v][0];
                    var levelv = Unsafe.Add(ref levelref, v) + 1;
                    for (var i = 0; i < gToCnt[v]; ++i)
                    {
                        var gToVi = Unsafe.Add(ref gToVref, i);
                        if (Unsafe.Add(ref gCapVref, i) == 0 || Unsafe.Add(ref levelref, gToVi) >= 0) continue;
                        Unsafe.Add(ref levelref, gToVi) = levelv;
                        if (gToVi == ti)
                        {
                            que.Clear();
                            break;
                        }
                        que.Enqueue(gToVi);
                    }
                }
                if (Unsafe.Add(ref levelref, ti) == -1) break;
                Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref iterref), 0, (uint)N << 2);
                while (flow < flowLimit)
                {
                    var f = ti == si ? (flowLimit - flow) : dfs(ti, flowLimit - flow);
                    if (f == 0) break;
                    flow += f;
                }
            }
            return flow;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
                for (var i = 0; i < gToCnt[p]; ++i)
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