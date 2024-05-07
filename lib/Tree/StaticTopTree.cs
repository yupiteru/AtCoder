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
    // copy key class LIB_StaticTopTree
    partial class /* not copy key */ LIB_Tree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CReStaticTopTree<Path, Point, Vertex> LIB_StaticTopTree<Path, Point, Vertex>(IEnumerable<Vertex> vertexArray, Point e) => new CReStaticTopTree<Path, Point, Vertex>(this, vertexArray, e);
        public class CReStaticTopTree<Path, Point, Vertex>
        {
            struct Node
            {
                public Path path;
                public Point point;
                public bool addEdge;
                public bool isPoint;
                public int parent;
                public int left;
                public int right;
                public int depth;
                public int vtx;
                public int vvtx;
            }
            LIB_Tree tree;
            Point e;
            Vertex[] vertexArray;
            Node[] nodes;
            int[] vtxToNode;
            int nodeIdx;
            int root;
            Func<Point, Point, Point> rake;
            Func<Path, Path, Path> compress;
            Func<Vertex, Point, Path> addVertex;
            Func<Path, Point> addEdge;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree(LIB_Tree tree, IEnumerable<Vertex> vertexArray, Point e)
            {
                this.vertexArray = vertexArray.ToArray();
                this.tree = tree;
                this.e = e;
            }
            public Vertex this[long vtx]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return vertexArray[vtx]; }
                set { vertexArray[vtx] = value; Recalc(vtx); }
            }
            public Point Query() => addEdge(nodes[root].path);
            public Point Query(long vtx)
            {
                var node = vtxToNode[vtx];
                var val = nodes[node].vtx == vtx + 1 ? addVertex(vertexArray[nodes[node].vtx - 1], e) : nodes[node].path;
                var id = true;
                while (!nodes[node].addEdge || nodes[node].vvtx == vtx + 1)
                {
                    var parent = nodes[node].parent;
                    if (parent == 0) break;
                    if (nodes[parent].left == node)
                    {
                        if (id && (!nodes[parent].addEdge || nodes[parent].vvtx == vtx + 1)) val = nodes[parent].path;
                        else val = compress(val, nodes[nodes[parent].right].path);
                    }
                    else
                    {
                        id = false;
                    }
                    node = parent;
                }
                return addEdge(val);
            }
            /// <summary>
            /// Rakeメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree<Path, Point, Vertex> Rake(Func<Point, Point, Point> rake) { this.rake = rake; return this; }
            /// <summary>
            /// Compress(parent, child)メソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree<Path, Point, Vertex> Compress(Func<Path, Path, Path> compress) { this.compress = compress; return this; }
            /// <summary>
            /// PointCluster に頂点を追加(vertex, pointCluster)するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree<Path, Point, Vertex> AddVertex(Func<Vertex, Point, Path> addVertex) { this.addVertex = addVertex; return this; }
            /// <summary>
            /// PathCluster に辺を追加(pathCluster)するメソッドを設定
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree<Path, Point, Vertex> AddEdge(Func<Path, Point> addEdge) { this.addEdge = addEdge; return this; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Update(int idx)
            {
                if (nodes[idx].isPoint)
                {
                    nodes[idx].point = rake(nodes[nodes[idx].left].point, nodes[nodes[idx].right].point);
                }
                else
                {
                    if (nodes[idx].vtx > 0) nodes[idx].path = addVertex(vertexArray[nodes[idx].vtx - 1], e);
                    else nodes[idx].path = compress(nodes[nodes[idx].left].path, nodes[nodes[idx].right].path);
                    if (nodes[idx].addEdge) nodes[idx].point = addEdge(nodes[idx].path);
                }
                if (nodes[idx].vvtx > 0) nodes[idx].path = addVertex(vertexArray[nodes[idx].vvtx - 1], nodes[idx].point);
            }
            void Recalc(long idx)
            {
                var node = vtxToNode[idx];
                while (node != 0)
                {
                    Update(node);
                    node = nodes[node].parent;
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int CreateNode(int left, int right)
            {
                var node = ++nodeIdx;
                nodes[node].depth = Max(nodes[left].depth, nodes[right].depth) + 1;
                nodes[node].left = left;
                nodes[node].right = right;
                nodes[left].parent = node;
                nodes[right].parent = node;
                return node;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public CReStaticTopTree<Path, Point, Vertex> Build(long root)
            {
                nodes = new Node[tree.N * 2];
                vtxToNode = new int[tree.N];

                var childCount = new long[tree.N];
                var depth = new long[tree.N];
                foreach (var (node, parent) in tree.BFSFromLeaf(0))
                {
                    childCount[node] = 1;
                    foreach (var item2 in tree.path[node])
                    {
                        if (item2 == parent) continue;
                        childCount[node] += childCount[item2];
                        if (depth[node] < depth[item2]) depth[node] = depth[item2];
                    }
                    ++depth[node];
                }

                var pq = new LIB_PriorityQueue();
                var deq = new LIB_Deque<int>();
                Func<int, int, int> makeCluster = null;
                makeCluster = (vtx, parent) =>
                {
                    var heavyList = new List<int>();
                    while (vtx != -1)
                    {
                        var lightVtxList = new List<int>();
                        var heavyVtx = -1;
                        foreach (var item in tree.path[vtx])
                        {
                            if (item == parent) continue;
                            if (heavyVtx == -1)
                            {
                                heavyVtx = item;
                            }
                            else
                            {
                                if (childCount[heavyVtx] < childCount[item] || (childCount[heavyVtx] == childCount[item] && depth[heavyVtx] > depth[item]))
                                {
                                    lightVtxList.Add(heavyVtx);
                                    heavyVtx = item;
                                }
                                else
                                {
                                    lightVtxList.Add(item);
                                }
                            }
                        }
                        if (lightVtxList.Count == 0)
                        {
                            var node = CreateNode(0, 0);
                            nodes[node].vtx = vtx + 1;
                            Update(node);
                            vtxToNode[vtx] = node;
                            heavyList.Add(node);
                        }
                        else
                        {
                            // Rake
                            var lightList = new List<int>();
                            foreach (var item in lightVtxList)
                            {
                                var cluster = makeCluster(item, vtx);
                                nodes[cluster].addEdge = true;
                                Update(cluster);
                                lightList.Add(cluster);
                            }
                            if (lightList.Count == 1)
                            {
                                var node = lightList[0];
                                nodes[node].vvtx = vtx + 1;
                                vtxToNode[vtx] = node;
                                Update(node);
                                heavyList.Add(node);
                            }
                            else
                            {
                                foreach (var item in lightList)
                                {
                                    pq.Push(nodes[item].depth, item);
                                }
                                while (pq.Count > 1)
                                {
                                    var merged = CreateNode(pq.Pop().Value, pq.Pop().Value);
                                    nodes[merged].isPoint = true;
                                    Update(merged);
                                    pq.Push(nodes[merged].depth, merged);
                                }
                                var node = pq.Pop().Value;
                                nodes[node].vvtx = vtx + 1;
                                vtxToNode[vtx] = node;
                                Update(node);
                                heavyList.Add(node);
                            }
                        }
                        parent = vtx;
                        vtx = heavyVtx;
                    }

                    // Compress
                    if (heavyList.Count == 1)
                    {
                        return heavyList[0];
                    }
                    else
                    {
                        foreach (var item in heavyList)
                        {
                            deq.PushFront(item);
                            while (true)
                            {
                                if (deq.Count >= 3 && (nodes[deq[2]].depth == nodes[deq[1]].depth || nodes[deq[2]].depth <= nodes[deq[0]].depth))
                                {
                                    var tmp = deq.PopFront();
                                    var right = deq.PopFront();
                                    var left = deq.PopFront();
                                    var merged = CreateNode(left, right);
                                    Update(merged);
                                    deq.PushFront(merged);
                                    deq.PushFront(tmp);
                                }
                                else if (deq.Count >= 2 && nodes[deq[1]].depth <= nodes[deq[0]].depth)
                                {
                                    var right = deq.PopFront();
                                    var left = deq.PopFront();
                                    var merged = CreateNode(left, right);
                                    Update(merged);
                                    deq.PushFront(merged);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        while (deq.Count > 1)
                        {
                            var right = deq.PopFront();
                            var left = deq.PopFront();
                            var merged = CreateNode(left, right);
                            Update(merged);
                            deq.PushFront(merged);
                        }
                        return deq.PopFront();
                    }
                };
                this.root = makeCluster((int)root, -1);

                return this;
            }
        }
    }
    ////end
}