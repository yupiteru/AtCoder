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
    // copy key class LIB_PathQuery
    partial class /* not copy key */ LIB_Tree
    {
        readonly struct Query
        {
            public readonly int idx;
            public readonly int l;
            public readonly int r;
            public Query(int idx, int l, int r) { this.idx = idx; this.l = l; this.r = r; }
        }
        readonly struct Edge
        {
            public readonly int idx;
            public readonly int a;
            public readonly int b;
            public Edge(int a, int b, Dictionary<long, int> table)
            {
                var key = 0L;
                if (a > b) key = ((long)a << 30) | (long)b;
                else key = ((long)b << 30) | (long)a;
                if (!table.ContainsKey(key))
                {
                    this.idx = table.Count;
                    table[key] = this.idx;
                }
                else
                {
                    this.idx = table[key];
                }
                this.a = a;
                this.b = b;
            }
        }
        public CPathQuery LIB_PathQuery(long root) => new CPathQuery((int)root, this);
        public class CPathQuery
        {
            LIB_Tree tree;
            Edge[] array;
            int[] vtxToArrayIndex;
            int root;
            List<Query> queries;
            int maxL;
            int maxR;
            int minL;
            int minR;
            int queryNum;
            Action<int, int> addEdge;
            Action<int, int> deleteEdge;
            Action<int> checker;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CPathQuery(int root, LIB_Tree tree)
            {
                maxL = maxR = int.MinValue;
                minL = minR = int.MaxValue;
                queryNum = 0;
                queries = new List<Query>();
                addEdge = deleteEdge = (e1, e2) => { };
                checker = e => { };
                var table = new Dictionary<long, int>();
                this.root = root;
                this.tree = tree;
                array = new Edge[tree.N * 2 - 1];
                var tour = tree.EulerTour(root);
                vtxToArrayIndex = new int[tree.N];
                for (var i = 0; i < tour.Length - 1; ++i)
                {
                    if (tour[i].direction == 1)
                    {
                        array[i] = new Edge((int)tour[i].parent, (int)tour[i].node, table);
                        vtxToArrayIndex[tour[i].node] = i;
                    }
                    else
                    {
                        array[i] = new Edge((int)tour[i].node, (int)tour[i].parent, table);
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int AddQuery(long vtx1, long vtx2)
            {
                var l = vtxToArrayIndex[vtx1];
                var r = vtxToArrayIndex[vtx2];
                if (r < l) { var t = l; l = r; r = t; }
                if (maxL < l) maxL = l;
                if (minL > l) minL = l;
                if (maxR < r) maxR = r;
                if (minR > r) minR = r;
                queries.Add(new Query(queryNum, l + 1, r + 1));
                return queryNum++;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            /// <summary>
            /// 指定した 辺 を追加するメソッドを設定
            /// (from, to)
            /// </summary>
            public CPathQuery SetAddEdgeMethod(Action<int, int> act)
            {
                addEdge = act;
                return this;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            /// <summary>
            /// 指定した 辺 を削除するメソッドを設定
            /// (from, to)
            /// </summary>
            public CPathQuery SetDeleteEdgeMethod(Action<int, int> act)
            {
                deleteEdge = act;
                return this;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            /// <summary>
            /// 指定した クエリindex の答えを処理するメソッドを設定
            /// </summary>
            public CPathQuery SetChecker(Action<int> act)
            {
                checker = act;
                return this;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Build()
            {
                Query[] sorted;
                if (maxL - minL > maxR - minR)
                {
                    var blockSize = Max(1, (int)(1.7320508 * (maxR - minR + 1) / Sqrt(2 * queryNum)));
                    sorted = queries.OrderBy(e => (e.r - minR + 1) / blockSize).ThenBy(e => (((e.r - minR + 1) / blockSize & 1) == 1) ? -e.l : e.l).ToArray();
                }
                else
                {
                    var blockSize = Max(1, (int)(1.7320508 * (maxL - minL + 1) / Sqrt(2 * queryNum)));
                    sorted = queries.OrderBy(e => (e.l - minL + 1) / blockSize).ThenBy(e => (((e.l - minL + 1) / blockSize & 1) == 1) ? -e.r : e.r).ToArray();
                }
                var lidx = 1;
                var ridx = 1;
                var isOdd = new bool[tree.N];
                ref var isOddRef = ref isOdd[0];
                ref var arrayRef = ref array[0];
                addEdge(-1, root);
                foreach (var item in sorted)
                {
                    while (item.l < lidx)
                    {
                        var edge = Unsafe.Add(ref arrayRef, --lidx);
                        if (Unsafe.Add(ref isOddRef, edge.idx)) deleteEdge(edge.a, edge.b);
                        else addEdge(edge.b, edge.a);
                        Unsafe.Add(ref isOddRef, edge.idx) = !Unsafe.Add(ref isOddRef, edge.idx);
                    }
                    while (ridx < item.r)
                    {
                        var edge = Unsafe.Add(ref arrayRef, ridx++);
                        if (Unsafe.Add(ref isOddRef, edge.idx)) deleteEdge(edge.b, edge.a);
                        else addEdge(edge.a, edge.b);
                        Unsafe.Add(ref isOddRef, edge.idx) = !Unsafe.Add(ref isOddRef, edge.idx);
                    }
                    while (lidx < item.l)
                    {
                        var edge = Unsafe.Add(ref arrayRef, lidx++);
                        if (Unsafe.Add(ref isOddRef, edge.idx)) deleteEdge(edge.b, edge.a);
                        else addEdge(edge.a, edge.b);
                        Unsafe.Add(ref isOddRef, edge.idx) = !Unsafe.Add(ref isOddRef, edge.idx);
                    }
                    while (item.r < ridx)
                    {
                        var edge = Unsafe.Add(ref arrayRef, --ridx);
                        if (Unsafe.Add(ref isOddRef, edge.idx)) deleteEdge(edge.a, edge.b);
                        else addEdge(edge.b, edge.a);
                        Unsafe.Add(ref isOddRef, edge.idx) = !Unsafe.Add(ref isOddRef, edge.idx);
                    }
                    checker(item.idx);
                }
            }
        }
    }
    ////end
}