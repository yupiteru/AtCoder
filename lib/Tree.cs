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
        public class TreeDPResult<T>
        {
            Dictionary<int, T>[] dp;
            public TreeDPResult(Dictionary<int, T>[] dp)
            {
                this.dp = dp;
            }
            public T this[long vtx, long parent]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return dp[vtx][(int)parent];
                }
            }
        }
        /// <summary>
        /// 全方位木DP
        /// </summary>
        /// <param name="idxToVal">頂点番号から値を取得</param>
        /// <param name="mergeSubTrees">部分木と部分木のマージ (subtree, subtree)</param>
        /// <param name="mergeVertexAndSubtree">頂点と部分木の結果のマージ (vertex, subtrees)</param>
        /// <returns>2次元配列[node, parent]の部分木のDP値</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TreeDPResult<T> BuildTreeDP<T>(Func<long, T> idxToVal, Func<T, T, T> mergeSubTrees, Func<T, T, T> mergeVertexAndSubtree)
        {
            var f = mergeSubTrees;
            var h = mergeVertexAndSubtree;
            var dp = Enumerable.Repeat(0, N).Select(_ => new Dictionary<int, T>()).ToArray();
            var done = new bool[N];
            Action<long, T> upd = (vtx, val) =>
            {
                if (vtx == -1) return;
                if (done[vtx]) dp[vtx][-1] = f(dp[vtx][-1], val);
                else dp[vtx][-1] = val;
                done[vtx] = true;
            };
            foreach (var item in BFSFromLeaf(0))
            {
                var ary = path[item.node].Where(e => e != item.parent).ToArray();
                var val = idxToVal(item.node);
                if (ary.Length > 0)
                {
                    var acc = dp[ary[0]][(int)item.node];
                    foreach (var item2 in ary.Skip(1)) acc = f(acc, dp[item2][(int)item.node]);
                    val = h(val, acc);
                }
                upd(item.parent, dp[item.node][(int)item.parent] = val);
            }
            var swag = new LIB_SlidingWindowAggregation<T>(f);
            Action<int, int> process = (node, parent) =>
            {
                var val = idxToVal(node);
                if (path[node].Count == 1)
                {
                    var x = path[node][0];
                    if (x != parent) upd(x, dp[node][x] = val);
                }
                else if (path[node].Count == 2)
                {
                    var x = path[node][0]; var y = path[node][1];
                    if (x != parent) upd(x, dp[node][x] = h(val, dp[y][node]));
                    if (y != parent) upd(y, dp[node][y] = h(val, dp[x][node]));
                }
                else if (path[node].Count == 3)
                {
                    var x = path[node][0]; var y = path[node][1]; var z = path[node][2];
                    if (x != parent) upd(x, dp[node][x] = h(val, f(dp[y][node], dp[z][node])));
                    if (y != parent) upd(y, dp[node][y] = h(val, f(dp[x][node], dp[z][node])));
                    if (z != parent) upd(z, dp[node][z] = h(val, f(dp[x][node], dp[y][node])));
                }
                else if (path[node].Count == 4)
                {
                    var x = path[node][0]; var y = path[node][1]; var z = path[node][2]; var w = path[node][3];
                    var xy = f(dp[x][node], dp[y][node]);
                    var zw = f(dp[z][node], dp[w][node]);
                    if (x != parent) upd(x, dp[node][x] = h(val, f(dp[y][node], zw)));
                    if (y != parent) upd(y, dp[node][y] = h(val, f(dp[x][node], zw)));
                    if (z != parent) upd(z, dp[node][z] = h(val, f(xy, dp[w][node])));
                    if (w != parent) upd(w, dp[node][w] = h(val, f(xy, dp[z][node])));
                }
                else
                {
                    swag.Clear();
                    foreach (var item2 in path[node]) swag.PushBack(dp[item2][node]);
                    foreach (var item2 in path[node])
                    {
                        swag.PopFront();
                        if (item2 != parent) upd(item2, dp[node][item2] = h(val, swag.Aggregate()));
                        swag.PushBack(dp[item2][node]);
                    }
                }
            };
            foreach (var item in BFSFromRoot(0)) process((int)item.node, (int)item.parent);
            for (var i = 0; i < N; i++) dp[i][-1] = done[i] ? h(idxToVal(i), dp[i][-1]) : idxToVal(i);
            return new TreeDPResult<T>(dp);
        }
    }
    ////end
}