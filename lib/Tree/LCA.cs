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
    // copy key class LIB_LCA
    partial class /* not copy key */ LIB_Tree
    {
        const int SHIFT = 30;
        const long MASK = 1073741823;
        public class LCAResult
        {
            LIB_DisjointSparseTable<long> dst;
            int[] visited;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LCAResult(LIB_DisjointSparseTable<long> dst, int[] visited)
            {
                this.dst = dst;
                this.visited = visited;
            }
            public long this[long u, long v]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    var min = Min(visited[u], visited[v]);
                    var max = min ^ visited[u] ^ visited[v];
                    return dst.Query(min, max + 1) & MASK;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LCAResult LIB_LCA(long root)
        {
            var euler = new List<long>();
            var visited = Enumerable.Repeat(-1, N).ToArray();
            Action<long, long, long> dfs = null;
            dfs = (node, parent, depth) =>
            {
                if (visited[node] == -1) visited[node] = euler.Count;
                euler.Add((depth << SHIFT) | node);
                foreach (var child in path[node])
                {
                    if (child == parent) continue;
                    dfs(child, node, depth + 1);
                    euler.Add((depth << SHIFT) | node);
                }
            };
            dfs(root, -1, 0);
            var dst = new LIB_DisjointSparseTable<long>(euler, Min);
            return new LCAResult(dst, visited);
        }
    }
    ////end
}