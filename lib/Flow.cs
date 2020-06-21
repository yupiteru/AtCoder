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
    class LIB_Flow
    {
        LIB_Dict<int, long>[] path;
        int N;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Flow(long n)
        {
            N = (int)n;
            path = Enumerable.Repeat(0, N).Select(_ => new LIB_Dict<int, long>()).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long a, long b, long c)
        {
            path[a][(int)b] += c;
            path[b][(int)a] += 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long FordFulkerson(long s, long t)
        {
            var f = Enumerable.Repeat(0, N).Select(_ => new LIB_Dict<int, long>()).ToArray();
            for (var i = 0; i < N; i++) foreach (var item in path[i]) f[i][item.Key] = item.Value;
            var done = new long[N];
            var counter = 1L;
            Func<int, long, long> dfs = null;
            dfs = (vtx, c) =>
            {
                done[vtx] = counter;
                if (vtx == t) return c;
                foreach (var item in f[vtx])
                {
                    if (done[item.Key] == counter) continue;
                    if (item.Value == 0) continue;
                    var flow = dfs(item.Key, Min(c, item.Value));
                    if (flow == 0) continue;
                    f[vtx][item.Key] -= flow;
                    f[item.Key][vtx] += flow;
                    return flow;
                }
                return 0;
            };
            var sum = 0L;
            for (long flow; (flow = dfs((int)s, long.MaxValue)) > 0; ++counter) sum += flow;
            return sum;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Dinic(long s, long t)
        {
            var f = Enumerable.Repeat(0, N).Select(_ => new LIB_Dict<int, long>()).ToArray();
            for (var i = 0; i < N; i++) foreach (var item in path[i]) f[i][item.Key] = item.Value;
            var level = new int[N];
            Func<bool> reset = () =>
            {
                level = new int[N];
                var q = new Queue<int>();
                level[s] = 1;
                q.Enqueue((int)s);
                while (q.Count > 0)
                {
                    var v = q.Dequeue();
                    foreach (var item in f[v])
                    {
                        if (item.Value > 0 && level[item.Key] == 0)
                        {
                            level[item.Key] = level[v] + 1;
                            q.Enqueue(item.Key);
                        }
                    }
                }
                return true;
            };
            Func<int, long, long> dfs = null;
            dfs = (vtx, c) =>
            {
                if (vtx == t) return c;
                foreach (var item in f[vtx])
                {
                    if (item.Value > 0 && level[vtx] < level[item.Key])
                    {
                        var flow = dfs(item.Key, Min(c, item.Value));
                        if (flow == 0) continue;
                        f[vtx][item.Key] -= flow;
                        f[item.Key][vtx] += flow;
                        return flow;
                    }
                }
                return 0;
            };
            var sum = 0L;
            while (reset() && level[t] > 0) for (long flow; (flow = dfs((int)s, long.MaxValue)) > 0;) sum += flow;
            return sum;
        }
    }
    ////end
}