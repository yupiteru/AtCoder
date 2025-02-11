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
    // copy key class LIB_AuxiliaryTree
    partial class /* not copy key */ LIB_Tree
    {
        public class AuxiliaryTree
        {
            IDictionary<long, List<(long vtx, long len)>>[] edges;
            IDictionary<long, int> idToIdx;
            long selectId = 0;
            public AuxiliaryTree(IDictionary<long, int> idToIdx, IDictionary<long, List<(long vtx, long len)>>[] edges)
            {
                this.idToIdx = idToIdx;
                this.edges = edges;
            }
            public AuxiliaryTree Select(long id)
            {
                selectId = idToIdx[id];
                return this;
            }
            public (long node, long parent, long dist)[] BFSFromRoot(long root = -1)
            {
                var bfsList = new (long node, long parent, long dist)[edges[selectId].Count];
                var q = new Queue<long>();
                var done = new HashSet<long>();
                var bfsIdx = 0;
                if (edges[selectId].Count == 0) return bfsList;
                if (root == -1) root = edges[selectId].Keys.First();
                bfsList[bfsIdx++] = (root, -1, 0);
                done.Add(root);
                q.Enqueue(root);
                while (q.Count > 0)
                {
                    var w = q.Dequeue();
                    foreach (var edge in edges[selectId][w])
                    {
                        if (done.Contains(edge.vtx)) continue;
                        done.Add(edge.vtx);
                        q.Enqueue(edge.vtx);
                        bfsList[bfsIdx++] = (edge.vtx, w, edge.len);
                    }
                }
                return bfsList;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (long node, long parent, long dist)[] BFSFromLeaf(long root = -1) => BFSFromRoot(root).Reverse().ToArray();
        }
        public AuxiliaryTree LIB_AuxiliaryTree(long[] idList)
        {
            var idToIdx = idList.Distinct().Select((e, i) => (e, i)).ToDictionary(e => e.e, e => e.i);
            var idxList = Enumerable.Range(0, N).Select(i => idToIdx[idList[i]]).ToArray();
            var stacks = Enumerable.Repeat(0, idToIdx.Count).Select(_ => new LIB_Deque<long>()).ToArray();
            var edges = Enumerable.Range(0, idToIdx.Count).Select(_ => new Dictionary<long, List<(long vtx, long len)>>()).ToArray();
            var depth = GetDistanceFrom(0);
            var lca = LIB_LCA(0);
            Action<long, long, long> addEdge = (id, u, v) =>
            {
                var dic = edges[id];
                if (!dic.ContainsKey(u)) dic[u] = new List<(long vtx, long len)>();
                if (!dic.ContainsKey(v)) dic[v] = new List<(long vtx, long len)>();
                var len = Abs(depth[u] - depth[v]);
                dic[u].Add((v, len));
                dic[v].Add((u, len));
            };
            Action<long, long> dfs = null;
            dfs = (v, p) =>
            {
                var idIdx = idxList[v];
                if (stacks[idIdx].Count == 0) stacks[idIdx].PushBack(v);
                else
                {
                    var u = stacks[idIdx].Back;
                    var w = lca[u, v];
                    if (w == u)
                    {
                        stacks[idIdx].PushBack(v);
                    }
                    else
                    {
                        while (stacks[idIdx].Count > 0)
                        {
                            if (stacks[idIdx].Back == w)
                            {
                                stacks[idIdx].PushBack(v);
                                break;
                            }
                            if (depth[stacks[idIdx].Back] < depth[w])
                            {
                                stacks[idIdx].PushBack(w);
                                stacks[idIdx].PushBack(v);
                                break;
                            }
                            var x = stacks[idIdx].PopBack();
                            if (stacks[idIdx].Count > 0 && depth[stacks[idIdx].Back] > depth[w])
                            {
                                addEdge(idIdx, stacks[idIdx].Back, x);
                            }
                            else
                            {
                                addEdge(idIdx, w, x);
                            }
                        }
                        if (stacks[idIdx].Count == 0)
                        {
                            stacks[idIdx].PushBack(w);
                            stacks[idIdx].PushBack(v);
                        }
                    }
                }

                foreach (var item in path[v])
                {
                    if (item == p) continue;
                    dfs(item, v);
                }
            };
            dfs(0, -1);
            for (var i = 0; i < idToIdx.Count; ++i)
            {
                while (stacks[i].Count > 1)
                {
                    addEdge(i, stacks[i].PopBack(), stacks[i].Back);
                }
            }

            return new AuxiliaryTree(idToIdx, edges);
        }
    }
    ////end
}