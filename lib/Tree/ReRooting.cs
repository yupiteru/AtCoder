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
    // copy key class LIB_ReRooting
    partial class /* not copy key */ LIB_Tree
    {
        /// <summary>
        /// 全方位木DP
        /// </summary>
        /// <returns>2次元配列[node, parent]の部分木のDP値。parent=-1はnodeをルートとした木の値</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CReRooting<T> LIB_ReRooting<T>() => new CReRooting<T>(this);
        public class CReRooting<T>
        {
            LIB_Tree tree;
            Dictionary<int, T>[] dp;
            Func<T, T, T> f;
            Func<long, T> g;
            Func<long, long, T, T> g2;
            Func<T, T, T> h;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting(LIB_Tree tree)
            {
                this.tree = tree;
                dp = Enumerable.Repeat(0, tree.N).Select(_ => new Dictionary<int, T>()).ToArray();
                f = (x, y) => throw new NotImplementedException();
                g = v => throw new NotImplementedException();
                g2 = (r, p, v) => v;
                h = (v, t) => t;
            }
            public T this[long vtx, long parent]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return dp[vtx][(int)parent]; }
            }
            /// <summary>
            /// 部分木と部分木のマージ(subtree, subtree)するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting<T> MergeSubtrees(Func<T, T, T> mergeSubTrees) { f = mergeSubTrees; return this; }
            /// <summary>
            /// 頂点番号から値を取得するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting<T> VertexToValue(Func<long, T> idxToVal) { g = idxToVal; return this; }
            /// <summary>
            /// 頂点番号(root, parent)で表す辺とrootをルートとする部分木をマージするメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting<T> MergeEdgeAndSubtree(Func<long, long, T, T> mergeEdgeAndSubtree) { g2 = mergeEdgeAndSubtree; return this; }
            /// <summary>
            /// 頂点と部分木の結果をマージ(vertex, subtrees)するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting<T> MergeVertexAndSubtree(Func<T, T, T> mergeVertexAndSubtree) { h = mergeVertexAndSubtree; return this; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReRooting<T> Build()
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
                return this;
            }
        }
    }
    ////end
}