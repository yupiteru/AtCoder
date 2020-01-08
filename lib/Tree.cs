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
        long N;
        List<long>[] path;
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
            N = n;
            path = Enumerable.Repeat(0, (int)N).Select(_ => new List<long>()).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long u, long v)
        {
            if (u >= N || v >= N) throw new Exception();
            path[u].Add(v);
            path[v].Add(u);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long[] GetSurround(long u) => path[u].ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<BFSRes> BFSFromRoot(long root)
        {
            bfsList = new List<BFSRes>();
            var q = new Queue<long>();
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
            var s = new Stack<Tuple<long, long>>();
            var done = new bool[N];
            done[root] = true;
            s.Push(Tuple.Create(root, -1L));
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
            long[][] pr;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LCAResult(int[] depth, int l, long[][] pr)
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
            var pr = Enumerable.Repeat(0, l).Select(_ => new long[N]).ToArray();
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
            Dictionary<long, T>[] dp;
            public TreeDPResult(Dictionary<long, T>[] dp)
            {
                this.dp = dp;
            }
            public T this[long vtx, long parent]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return dp[vtx][parent];
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TreeDPResult<T> BuildTreeDP<T>(Func<long, T> idxToVal, Func<T, T, T> f)
        {
            var dp = Enumerable.Repeat(0, (int)N).Select(_ => new Dictionary<long, T>()).ToArray();
            foreach (var item in BFSFromLeaf(0))
            {
                var acc = idxToVal(item.node);
                foreach (var item2 in path[item.node])
                {
                    if (item2 == item.parent) continue;
                    acc = f(acc, dp[item2][item.node]);
                }
                dp[item.node][item.parent] = acc;
            }
            Func<long, long, T> fun = null;
            fun = (vtx, parent) =>
            {
                var acc = idxToVal(vtx);
                if (path[vtx].Count == 1)
                {
                    var x = path[vtx][0];
                    if (x != parent) { dp[vtx][x] = acc; acc = f(acc, fun(x, vtx)); }
                }
                else if (path[vtx].Count == 2)
                {
                    var x = path[vtx][0]; var y = path[vtx][1];
                    var xval = f(acc, dp[x][vtx]); var yval = f(acc, dp[y][vtx]);
                    if (x != parent) { dp[vtx][x] = yval; acc = f(acc, fun(x, vtx)); }
                    if (y != parent) { dp[vtx][y] = xval; acc = f(acc, fun(y, vtx)); }
                }
                else if (path[vtx].Count == 3)
                {
                    var x = path[vtx][0]; var y = path[vtx][1]; var z = path[vtx][2];
                    var yzval = f(acc, f(dp[y][vtx], dp[z][vtx])); var xzval = f(acc, f(dp[x][vtx], dp[z][vtx])); var xyval = f(acc, f(dp[x][vtx], dp[y][vtx]));
                    if (x != parent) { dp[vtx][x] = yzval; acc = f(acc, fun(x, vtx)); }
                    if (y != parent) { dp[vtx][y] = xzval; acc = f(acc, fun(y, vtx)); }
                    if (z != parent) { dp[vtx][z] = xyval; acc = f(acc, fun(z, vtx)); }
                }
                else if (path[vtx].Count == 4)
                {
                    var x = path[vtx][0]; var y = path[vtx][1]; var z = path[vtx][2]; var w = path[vtx][3];
                    var xyval = f(dp[x][vtx], dp[y][vtx]); var zwval = f(dp[z][vtx], dp[w][vtx]);
                    var yzwval = f(acc, f(dp[y][vtx], zwval)); var xzwval = f(acc, f(dp[x][vtx], zwval)); var xywval = f(acc, f(xyval, dp[w][vtx])); var xyzval = f(acc, f(xyval, dp[z][vtx]));
                    if (x != parent) { dp[vtx][x] = yzwval; acc = f(acc, fun(x, vtx)); }
                    if (y != parent) { dp[vtx][y] = xzwval; acc = f(acc, fun(y, vtx)); }
                    if (z != parent) { dp[vtx][z] = xywval; acc = f(acc, fun(z, vtx)); }
                    if (w != parent) { dp[vtx][w] = xyzval; acc = f(acc, fun(w, vtx)); }
                }
                else
                {
                    var ary = new T[path[vtx].Count + 1];
                    for (var i = 0; i < path[vtx].Count; i++) ary[i] = dp[path[vtx][i]][vtx];
                    ary[path[vtx].Count] = acc;
                    var swag = new LIB_SlidingWindowAggregation<T>(ary, f);
                    foreach (var item in path[vtx])
                    {
                        swag.PopFront();
                        if (item != parent)
                        {
                            dp[vtx][item] = swag.Aggregate();
                            acc = f(acc, fun(item, vtx));
                        }
                        swag.PushBack(dp[item][vtx]);
                    }
                }
                return dp[vtx][parent] = acc;
            };
            fun(0, -1);
            return new TreeDPResult<T>(dp);
        }
    }
    ////end
}