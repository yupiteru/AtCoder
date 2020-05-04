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
        public class LCAResult
        {
            int[] depth;
            int l;
            int[][] pr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LCAResult(int[] depth, int l, int[][] pr)
            {
                this.depth = depth;
                this.l = l;
                this.pr = pr;
            }
            public long this[long u, long v]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    if (depth[u] > depth[v])
                    {
                        var t = u; u = v; v = t;
                    }
                    for (var k = 0; k < l; k++)
                    {
                        if ((((depth[v] - depth[u]) >> k) & 1) != 0) v = pr[k][v];
                    }
                    if (u == v) return u;
                    for (var k = l - 1; k >= 0; k--)
                    {
                        if (pr[k][u] != pr[k][v])
                        {
                            u = pr[k][u];
                            v = pr[k][v];
                        }
                    }
                    return pr[0][u];
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LCAResult LIB_LCA(long root)
        {
            var depth = new int[N];
            var l = 0;
            while (N > (1 << l)) l++;
            var pr = Enumerable.Repeat(0, l).Select(_ => new int[N]).ToArray();
            depth[root] = 0;
            pr[0][root] = -1;
            var q = new Stack<int>();
            q.Push((int)root);
            while (q.Count > 0)
            {
                var w = q.Pop();
                foreach (var i in path[w])
                {
                    if (i == pr[0][w]) continue;
                    q.Push(i);
                    depth[i] = depth[w] + 1;
                    pr[0][i] = w;
                }
            }
            for (var k = 0; k + 1 < l; k++)
            {
                for (var w = 0; w < N; w++)
                {
                    if (pr[k][w] < 0) pr[k + 1][w] = -1;
                    else pr[k + 1][w] = pr[k][pr[k][w]];
                }
            }
            return new LCAResult(depth, l, pr);
        }
    }
    ////end
}