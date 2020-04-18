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
        List<int>[] path;
        public struct EulerRes
        {
            public long node;
            public long parent;
            public long direction;
        }
        List<EulerRes> eulerList;
        public struct BFSRes
        {
            public long node;
            public long parent;
        }
        List<BFSRes> bfsList;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Tree(long n)
        {
            N = (int)n;
            path = Enumerable.Repeat(0, N).Select(_ => new List<int>()).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long u, long v)
        {
            if (u >= N || v >= N) throw new Exception();
            path[u].Add((int)v);
            path[v].Add((int)u);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] GetSurround(long u) => path[u].ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<BFSRes> BFSFromRoot(long root)
        {
            bfsList = new List<BFSRes>();
            var q = new Queue<int>();
            var done = new bool[N];
            bfsList.Add(new BFSRes { node = root, parent = -1 });
            done[root] = true;
            q.Enqueue((int)root);
            while (q.Count > 0)
            {
                var w = q.Dequeue();
                foreach (var i in path[w])
                {
                    if (done[i]) continue;
                    done[i] = true;
                    q.Enqueue(i);
                    bfsList.Add(new BFSRes { node = i, parent = w });
                }
            }
            return bfsList;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<BFSRes> BFSFromLeaf(long root) => BFSFromRoot(root).ToArray().Reverse().ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<EulerRes> EulerTour(long root)
        {
            eulerList = new List<EulerRes>();
            var s = new Stack<Tuple<int, int>>();
            var done = new bool[N];
            done[root] = true;
            s.Push(Tuple.Create((int)root, -1));
            while (s.Count > 0)
            {
                var w = s.Peek();
                var ad = true;
                foreach (var i in path[w.Item1])
                {
                    if (done[i]) continue;
                    done[i] = true;
                    ad = false;
                    s.Push(Tuple.Create(i, w.Item1));
                }
                if (!ad || path[w.Item1].Count == 1) eulerList.Add(new EulerRes { node = w.Item1, parent = w.Item2, direction = 1 });
                if (ad)
                {
                    s.Pop();
                    eulerList.Add(new EulerRes { node = w.Item1, parent = w.Item2, direction = -1 });
                }
            }
            return eulerList;
        }
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
        public LCAResult BuildLCA(long root)
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
        /// <summary>
        /// 全方位木DP
        /// </summary>
        /// <param name="idxToVal">頂点番号から値を取得</param>
        /// <param name="mergeSubTrees">部分木と部分木のマージ (subtree, subtree)</param>
        /// <returns>2次元配列[node, parent]の部分木のDP値。parent=-1はnodeをルートとした木の値</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CReRooting<T> ReRooting<T>(Func<long, T> idxToVal, Func<T, T, T> mergeSubTrees) => new CReRooting<T>(this, idxToVal, mergeSubTrees);
        public class CReRooting<T>
        {
            LIB_Tree tree;
            Dictionary<int, T>[] dp;
            Func<T, T, T> f;
            Func<long, T> g;
            Func<long, long, T, T> g2;
            Func<T, T, T> h;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting(LIB_Tree tree, Func<long, T> idxToVal, Func<T, T, T> mergeSubTrees)
            {
                this.tree = tree;
                dp = Enumerable.Repeat(0, tree.N).Select(_ => new Dictionary<int, T>()).ToArray();
                f = mergeSubTrees;
                g = idxToVal;
                g2 = (r, p, v) => v;
                h = (v, t) => t;
            }
            public T this[long vtx, long parent]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return dp[vtx][(int)parent]; }
            }
            /// <summary>
            /// 頂点番号(root, parent)で表す辺とrootをルートとする部分木をマージするメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetMergeEdgeAndSubtree(Func<long, long, T, T> mergeEdgeAndSubtree) { g2 = mergeEdgeAndSubtree; }
            /// <summary>
            /// 頂点と部分木の結果をマージ(vertex, subtrees)するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetMergeVertexAndSubtree(Func<T, T, T> mergeVertexAndSubtree) { h = mergeVertexAndSubtree; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Build()
            {
                var done = new bool[tree.N];
                Action<long, long, T> upd = (vtx, rt, val) =>
                {
                    if (vtx == -1) return;
                    if (done[vtx]) dp[vtx][-1] = f(dp[vtx][-1], g2(rt, vtx, val));
                    else dp[vtx][-1] = g2(rt, vtx, val);
                    done[vtx] = true;
                };
                foreach (var item in tree.BFSFromLeaf(0))
                {
                    var ary = tree.path[item.node].Where(e => e != item.parent).ToArray();
                    var val = g(item.node);
                    if (ary.Length > 0)
                    {
                        var acc = g2(ary[0], item.node, dp[ary[0]][(int)item.node]);
                        foreach (var item2 in ary.Skip(1)) acc = f(acc, g2(item2, item.node, dp[item2][(int)item.node]));
                        val = h(val, acc);
                    }
                    upd(item.parent, item.node, dp[item.node][(int)item.parent] = val);
                }
                var swag = new LIB_SlidingWindowAggregation<T>(f);
                Action<int, int> process = (node, parent) =>
                {
                    var val = g(node);
                    if (tree.path[node].Count == 1)
                    {
                        var x = tree.path[node][0];
                        if (x != parent) upd(x, node, dp[node][x] = val);
                    }
                    else if (tree.path[node].Count == 2)
                    {
                        var x = tree.path[node][0]; var y = tree.path[node][1];
                        if (x != parent) upd(x, node, dp[node][x] = h(val, g2(y, node, dp[y][node])));
                        if (y != parent) upd(y, node, dp[node][y] = h(val, g2(x, node, dp[x][node])));
                    }
                    else if (tree.path[node].Count == 3)
                    {
                        var x = tree.path[node][0]; var y = tree.path[node][1]; var z = tree.path[node][2];
                        if (x != parent) upd(x, node, dp[node][x] = h(val, f(g2(y, node, dp[y][node]), g2(z, node, dp[z][node]))));
                        if (y != parent) upd(y, node, dp[node][y] = h(val, f(g2(x, node, dp[x][node]), g2(z, node, dp[z][node]))));
                        if (z != parent) upd(z, node, dp[node][z] = h(val, f(g2(x, node, dp[x][node]), g2(y, node, dp[y][node]))));
                    }
                    else if (tree.path[node].Count == 4)
                    {
                        var x = tree.path[node][0]; var y = tree.path[node][1]; var z = tree.path[node][2]; var w = tree.path[node][3];
                        var xy = f(g2(x, node, dp[x][node]), g2(y, node, dp[y][node]));
                        var zw = f(g2(z, node, dp[z][node]), g2(w, node, dp[w][node]));
                        if (x != parent) upd(x, node, dp[node][x] = h(val, f(g2(y, node, dp[y][node]), zw)));
                        if (y != parent) upd(y, node, dp[node][y] = h(val, f(g2(x, node, dp[x][node]), zw)));
                        if (z != parent) upd(z, node, dp[node][z] = h(val, f(xy, g2(w, node, dp[w][node]))));
                        if (w != parent) upd(w, node, dp[node][w] = h(val, f(xy, g2(z, node, dp[z][node]))));
                    }
                    else
                    {
                        swag.Clear();
                        var pre = new T[tree.path[node].Count];
                        for (var i = 0; i < pre.Length; ++i) swag.PushBack(pre[i] = g2(tree.path[node][i], node, dp[tree.path[node][i]][node]));
                        for (var i = 0; i < pre.Length; ++i)
                        {
                            var item2 = tree.path[node][i];
                            swag.PopFront();
                            if (item2 != parent) upd(item2, node, dp[node][item2] = h(val, swag.Aggregate()));
                            swag.PushBack(pre[i]);
                        }
                    }
                };
                foreach (var item in tree.BFSFromRoot(0)) process((int)item.node, (int)item.parent);
                for (var i = 1; i < tree.N; i++) dp[i][-1] = done[i] ? h(g(i), dp[i][-1]) : g(i);
            }
        }
    }
    ////end
}