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
    class LIB_Tree
    {
        int N;
        List<long>[] path;
        int root;
        public struct EulerRes
        {
            public int node;
            public int parent;
            public int direction;
        }
        List<EulerRes> eulerList;
        public struct BFSRes
        {
            public int node;
            public int parent;
        }
        List<BFSRes> bfsList;
        bool lca;
        bool euler;
        bool bfs;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Tree(List<long>[] _path, long _root)
        {
            N = _path.Length;
            path = _path;
            root = (int)_root;
            lca = false;
            euler = false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<BFSRes> BFSRoot()
        {
            if (!bfs)
            {
                bfsList = new List<BFSRes>();
                var q = new Queue<int>();
                var done = new bool[N];
                bfsList.Add(new BFSRes { node = root, parent = -1 });
                done[root] = true;
                q.Enqueue(root);
                while (q.Count > 0)
                {
                    var w = q.Dequeue();
                    foreach (var i in path[w])
                    {
                        if (done[i]) continue;
                        done[i] = true;
                        q.Enqueue((int)i);
                        bfsList.Add(new BFSRes { node = (int)i, parent = w });
                    }
                }
                bfs = true;
            }
            return bfsList;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<BFSRes> BFSLeaf() => BFSRoot().ToArray().Reverse().ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<EulerRes> Euler()
        {
            if (!euler)
            {
                eulerList = new List<EulerRes>();
                var s = new Stack<Tuple<int, int>>();
                var done = new bool[N];
                done[root] = true;
                s.Push(Tuple.Create(root, -1));
                while (s.Count > 0)
                {
                    var w = s.Peek();
                    var ad = true;
                    foreach (var i in path[w.Item1])
                    {
                        if (done[i]) continue;
                        done[i] = true;
                        ad = false;
                        s.Push(Tuple.Create((int)i, w.Item1));
                    }
                    if (!ad || path[w.Item1].Count == 1) eulerList.Add(new EulerRes { node = w.Item1, parent = w.Item2, direction = 1 });
                    if (ad)
                    {
                        s.Pop();
                        eulerList.Add(new EulerRes { node = w.Item1, parent = w.Item2, direction = -1 });
                    }
                }
                euler = true;
            }
            return eulerList;
        }
        int[] depth;
        int l;
        int[][] pr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LCA(long u, long v)
        {
            if (!lca)
            {
                depth = new int[N];
                l = 0;
                while (N > (1 << l)) l++;
                pr = Enumerable.Repeat(0, l).Select(_ => new int[N]).ToArray();
                depth[root] = 0;
                pr[0][root] = -1;
                var q = new Stack<long>();
                q.Push(root);
                while (q.Count > 0)
                {
                    var w = q.Pop();
                    foreach (var i in path[w])
                    {
                        if (i == pr[0][w]) continue;
                        q.Push(i);
                        depth[i] = depth[w] + 1;
                        pr[0][i] = (int)w;
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
                lca = true;
            }
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
    ////end
}