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
        public long[] GetSurround(long u) => path[u].Cast<long>().ToArray();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TreeDPResult<T> BuildTreeDP<T>(Func<long, T> idxToVal, Func<T, T, T> f)
        {
            var dp = Enumerable.Repeat(0, N).Select(_ => new Dictionary<int, T>()).ToArray();
            foreach (var item in BFSFromLeaf(0))
            {
                var acc = idxToVal(item.node);
                foreach (var item2 in path[item.node])
                {
                    if (item2 == item.parent) continue;
                    acc = f(acc, dp[item2][(int)item.node]);
                }
                dp[item.node][(int)item.parent] = acc;
            }
            var swag = new LIB_SlidingWindowAggregation<T>(f);
            var done = new bool[N];
            var q = new Queue<int>();
            done[0] = true;
            q.Enqueue(0);
            foreach (var item in path[0]) swag.PushBack(dp[item][0]);
            swag.PushBack(idxToVal(0));
            foreach (var item in path[0])
            {
                swag.PopFront();
                dp[0][item] = swag.Aggregate();
                swag.PushBack(dp[item][0]);
            }
            while (q.Count > 0)
            {
                var v = q.Dequeue();
                foreach (var item in path[v])
                {
                    if (done[item]) continue;
                    done[item] = true;
                    q.Enqueue(item);
                    var val = idxToVal(item);
                    if (path[item].Count == 1)
                    {
                        var x = path[item][0];
                        if (x != v) dp[item][x] = val;
                    }
                    else if (path[item].Count == 2)
                    {
                        var x = path[item][0]; var y = path[item][1];
                        if (x != v) dp[item][x] = f(val, dp[y][item]);
                        if (y != v) dp[item][y] = f(val, dp[x][item]);
                    }
                    else if (path[item].Count == 3)
                    {
                        var x = path[item][0]; var y = path[item][1]; var z = path[item][2];
                        if (x != v) dp[item][x] = f(val, f(dp[y][item], dp[z][item]));
                        if (y != v) dp[item][y] = f(val, f(dp[x][item], dp[z][item]));
                        if (z != v) dp[item][z] = f(val, f(dp[x][item], dp[y][item]));
                    }
                    else if (path[item].Count == 4)
                    {
                        var x = path[item][0]; var y = path[item][1]; var z = path[item][2]; var w = path[item][3];
                        var xyval = f(val, f(dp[x][item], dp[y][item]));
                        var zwval = f(val, f(dp[z][item], dp[w][item]));
                        if (x != v) dp[item][x] = f(dp[y][item], zwval);
                        if (y != v) dp[item][y] = f(dp[x][item], zwval);
                        if (z != v) dp[item][z] = f(xyval, dp[w][item]);
                        if (w != v) dp[item][w] = f(xyval, dp[z][item]);
                    }
                    else
                    {
                        swag.Clear();
                        foreach (var item2 in path[item]) swag.PushBack(dp[item2][item]);
                        swag.PushBack(val);
                        foreach (var item2 in path[item])
                        {
                            swag.PopFront();
                            if (item2 != v) dp[item][item2] = swag.Aggregate();
                            swag.PushBack(dp[item2][item]);
                        }
                    }
                }
            }
            return new TreeDPResult<T>(dp);
        }
    }
    ////end
}