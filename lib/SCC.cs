
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
    class LIB_SCC
    {
        int n;
        List<long> edges;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SCC(long n)
        {
            this.n = (int)n;
            edges = new List<long>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long from, long to) => edges.Add((from << 30) | to);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected (int, int[]) SCCIDs()
        {
            var start = new int[n + 1];
            var elist = new int[edges.Count];
            foreach (var e in edges) ++start[(e >> 30) + 1];
            for (var i = 1; i < start.Length; ++i) start[i] += start[i - 1];
            var counter = new int[start.Length];
            for (var i = 0; i < counter.Length; i++) counter[i] = start[i];
            foreach (var e in edges) elist[counter[e >> 30]++] = (int)(e & 1073741823);
            var nowOrd = -1;
            var groupNum = 0;
            var visitedIdx = -1;
            var visited = new int[n];
            var low = new int[n];
            var ord = new int[n];
            var ids = new int[n];
            for (var i = 0; i < ord.Length; i++) ord[i] = -1;
            Action<int> dfs = null;
            dfs = v =>
            {
                low[v] = ord[v] = ++nowOrd;
                visited[++visitedIdx] = v;
                for (var i = start[v]; i < start[v + 1]; ++i)
                {
                    var to = elist[i];
                    if (ord[to] == -1)
                    {
                        dfs(to);
                        low[v] = Math.Min(low[v], low[to]);
                    }
                    else low[v] = Math.Min(low[v], ord[to]);
                }
                if (low[v] == ord[v])
                {
                    while (true)
                    {
                        var u = visited[visitedIdx];
                        --visitedIdx;
                        ord[u] = n;
                        ids[u] = groupNum;
                        if (u == v) break;
                    }
                    ++groupNum;
                }
            };
            for (var i = 0; i < ord.Length; ++i)
            {
                if (ord[i] == -1) dfs(i);
            }
            for (var i = 0; i < ids.Length; i++) ids[i] = groupNum - 1 - ids[i];
            return (groupNum, ids);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[][] SCC()
        {
            var ids = SCCIDs();
            var counts = new int[ids.Item1];
            var groups = new int[ids.Item1][];
            foreach (var x in ids.Item2) ++counts[x];
            for (var i = 0; i < groups.Length; ++i) groups[i] = new int[counts[i]];
            for (var i = 0; i < ids.Item2.Length; i++) groups[ids.Item2[i]][--counts[ids.Item2[i]]] = i;
            return groups;
        }
    }
    ////end
}