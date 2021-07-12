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
            LIB_SparseTableMin dst;
            int[] visited;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LCAResult(LIB_SparseTableMin dst, int[] visited)
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
            var visited = new int[N];
            var nodestack = new long[N];
            var parentstack = new long[N];
            var depthlist = new long[N];
            ref var refvisited = ref visited[0];
            ref var refnodestack = ref nodestack[0];
            ref var refparentstack = ref parentstack[0];
            ref var refdepth = ref depthlist[0];
            var stackcount = 1;
            refnodestack = root;
            refparentstack = -1;
            while (stackcount > 0)
            {
                var node = Unsafe.Add(ref refnodestack, stackcount);
                var parent = Unsafe.Add(ref refparentstack, stackcount--);
                ref var depth = ref Unsafe.Add(ref refdepth, (int)node);
                euler.Add(((depth - 1) << SHIFT) | parent);
                Unsafe.Add(ref refvisited, (int)node) = euler.Count;
                var leaf = true;
                foreach (var child in path[node])
                {
                    if (child == parent) continue;
                    leaf = false;
                    Unsafe.Add(ref refdepth, child) = depth + 1;
                    Unsafe.Add(ref refnodestack, ++stackcount) = child;
                    Unsafe.Add(ref refparentstack, stackcount) = node;
                }
                if (leaf) euler.Add((depth << SHIFT) | node);
            }
            var dst = new LIB_SparseTableMin(euler);
            return new LCAResult(dst, visited);
        }
    }
    ////end
}