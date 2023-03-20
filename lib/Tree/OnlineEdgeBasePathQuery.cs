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
    // copy key class LIB_OnlineEdgeBasePathQuery
    partial class /* not copy key */ LIB_Tree
    {
        public class OnlineEdgeBasePathQuery<T>
        {
            LIB_Tree tree;
            LCAResult lca;
            LIB_SegTree<T> seg;
            (long node, long parent, long direction)[] eulerTour;
            int[] vtxToTourIndex;
            Dictionary<int, (int enter, int leave)>[] edgeToTourIndex;
            Func<T, T> rev;
            Func<T, T, T> f;

            public OnlineEdgeBasePathQuery(LIB_Tree tree, LCAResult lca, LIB_SegTree<T> seg, (long node, long parent, long direction)[] eulerTour, Func<T, T> rev, Func<T, T, T> f)
            {
                this.tree = tree;
                this.lca = lca;
                this.seg = seg;
                this.eulerTour = eulerTour;
                this.rev = rev;
                this.f = f;
                vtxToTourIndex = new int[tree.N];
                edgeToTourIndex = Enumerable.Repeat(0, tree.N).Select(_ => new Dictionary<int, (int enter, int leave)>()).ToArray();

                for (var i = 0; i < eulerTour.Length; ++i)
                {
                    if (eulerTour[i].direction == 1)
                    {
                        vtxToTourIndex[eulerTour[i].node] = i;
                    }

                    if (eulerTour[i].parent == -1) continue;
                    if (eulerTour[i].direction == 1)
                    {
                        vtxToTourIndex[eulerTour[i].node] = i;
                        edgeToTourIndex[eulerTour[i].node][(int)eulerTour[i].parent] = (i, -1);
                        edgeToTourIndex[eulerTour[i].parent][(int)eulerTour[i].node] = (i, -1);
                    }
                    else
                    {
                        var v = edgeToTourIndex[eulerTour[i].node][(int)eulerTour[i].parent];
                        v.leave = i;
                        edgeToTourIndex[eulerTour[i].node][(int)eulerTour[i].parent] = v;
                        edgeToTourIndex[eulerTour[i].parent][(int)eulerTour[i].node] = v;
                    }
                }
            }

            public void SetEdgeValue(long idx, T value) => SetEdgeValue(tree.edges[(int)idx].u, tree.edges[(int)idx].v, value);
            public void SetEdgeValue(long u, long v, T value)
            {
                seg[edgeToTourIndex[u][(int)v].enter] = value;
                seg[edgeToTourIndex[u][(int)v].leave] = rev(value);
            }

            public T Query(long u, long v)
            {
                var lcaValue = seg.Query(0, vtxToTourIndex[lca[u, v]] + 1);
                var uValue = seg.Query(0, vtxToTourIndex[u] + 1);
                var vValue = seg.Query(0, vtxToTourIndex[v] + 1);
                return f(uValue, f(vValue, rev(f(lcaValue, lcaValue))));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OnlineEdgeBasePathQuery<T> LIB_OnlineEdgeBasePathQuery<T>(T e, Func<T, T> rev, Func<T, T, T> f)
        {
            var root = 0;
            var eulerTour = EulerTour(root);
            var lca = LIB_LCA(root);
            var seg = new LIB_SegTree<T>(eulerTour.Length, e, f);

            return new OnlineEdgeBasePathQuery<T>(this, lca, seg, eulerTour, rev, f);
        }
    }
    ////end
}