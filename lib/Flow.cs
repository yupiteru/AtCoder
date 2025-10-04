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
    class LIB_Flow<FlowCost> where FlowCost : struct, System.Numerics.INumber<FlowCost>
    {
        public class Edge
        {
            public int from;
            public int to;
            public FlowCost cap;
            public FlowCost flow;
            public FlowCost cost;
            public bool isFlip;
        }
        struct Parent
        {
            public int p;
            public int e;
            public FlowCost up;
            public FlowCost down;
        }

        int N;
        int M;
        List<Edge> edges;
        List<FlowCost> lowers;
        FlowCost[] dss;

        int BUCKET_SIZE;
        int MINOR_LIMIT;
        Parent[] parents;
        int[] depth;
        int[] nex;
        int[] pre;
        List<int> candidates;

        bool feasible;
        FlowCost totalCost;
        public FlowCost[] pots;

        public class FlowsTmp
        {
            LIB_Flow<FlowCost> f;
            public int Length => f.edges.Count >> 1;
            public FlowCost this[long i]
            {
                get => f.edges[(int)i << 1].isFlip ? f.edges[(int)i << 1].cap : f.edges[(int)i << 1].flow;
                private set { }
            }
            public FlowsTmp(LIB_Flow<FlowCost> flow)
            {
                f = flow;
            }
        }
        FlowsTmp _flowsTmp;

        public FlowsTmp Flows
        {
            get => _flowsTmp;
            private set { }
        }

        public LIB_Flow(long N)
        {
            _flowsTmp = new FlowsTmp(this);
            this.N = (int)N;
            dss = new FlowCost[N];
            edges = new List<Edge>();
            lowers = new List<FlowCost>();
            candidates = new List<int>();
            totalCost = FlowCost.Zero;
        }

        public long AddEdge(long from, long to, FlowCost lower, FlowCost upper, FlowCost? cost = null)
        {
            if (cost == null) cost = FlowCost.Zero;
            if (cost < FlowCost.Zero)
            {
                cost = -cost;
                totalCost -= upper * cost.Value;
                (from, to) = (to, from);
                edges.Add(new Edge() { from = (int)from, to = (int)to, cap = upper - lower, cost = cost.Value, isFlip = true });
                edges.Add(new Edge() { from = (int)to, to = (int)from, cap = FlowCost.Zero, cost = -cost.Value, isFlip = true });
                lowers.Add(lower);
                dss[from] -= lower;
                dss[to] += lower;
                AddDS(from, upper);
                AddDS(to, -upper);
                M = edges.Count;
            }
            else
            {
                edges.Add(new Edge() { from = (int)from, to = (int)to, cap = upper - lower, cost = cost.Value, isFlip = false });
                edges.Add(new Edge() { from = (int)to, to = (int)from, cap = FlowCost.Zero, cost = -cost.Value, isFlip = false });
                lowers.Add(lower);
                dss[from] -= lower;
                dss[to] += lower;
                M = edges.Count;
            }
            return lowers.Count - 1;
        }

        public void AddDS(long v, FlowCost ds)
        {
            dss[v] += ds;
        }


        public (bool feasible, FlowCost flow, FlowCost cost) Solve(long s, long g, FlowCost? flowLimit = null)
        {
            if (flowLimit == null) flowLimit = FlowCost.Parse("1000000000000000000", null);
            AddDS(s, flowLimit.Value);
            AddDS(g, -flowLimit.Value);
            var res = Solve(true);
            return (res.feasible, edges[M + (int)g * 2 + 1].cap, res.cost);
        }

        public (bool feasible, FlowCost cost) Solve(bool ignoreFeasibility = false)
        {
            BUCKET_SIZE = Max((int)(Sqrt(M) * 0.2), 10);
            MINOR_LIMIT = Max((int)(BUCKET_SIZE * 0.1), 3);
            Precompute();
            candidates.Capacity = BUCKET_SIZE;
            var ei = 0;
            while (true)
            {
                for (var i = 0; i < MINOR_LIMIT; ++i)
                {
                    if (!Minor()) break;
                }
                var best = FlowCost.Zero;
                var bestEi = -1;
                candidates.Clear();
                for (var i = 0; i < edges.Count; ++i)
                {
                    if (edges[ei].cap != FlowCost.Zero)
                    {
                        var clen = edges[ei].cost + pots[edges[ei ^ 1].to] - pots[edges[ei].to];
                        if (clen < FlowCost.Zero)
                        {
                            if (clen < best)
                            {
                                best = clen;
                                bestEi = ei;
                            }
                            candidates.Add(ei);
                            if (candidates.Count == BUCKET_SIZE) break;
                        }
                    }
                    ++ei;
                    if (ei == edges.Count) ei = 0;
                }
                if (candidates.Count == 0) break;
                PushFlow(bestEi);
            }
            if (!Postcompute(ignoreFeasibility)) return (false, FlowCost.Zero);
            return (true, totalCost);
        }

        void Connect(int u, int v)
        {
            nex[u] = v;
            pre[v] = u;
        }

        void Precompute()
        {
            pots = new FlowCost[N + 1];
            parents = new Parent[N];
            depth = new int[N + 1];
            depth.Fill(1);
            nex = new int[(N + 1) * 2];
            pre = new int[(N + 1) * 2];
            var infCost = FlowCost.One;
            for (var i = 0; i < M; i += 2)
            {
                infCost += edges[i].cost >= FlowCost.Zero ? edges[i].cost : -edges[i].cost;
            }
            edges.Capacity = M + N * 2;
            for (var i = 0; i < N; ++i)
            {
                if (dss[i] >= FlowCost.Zero)
                {
                    edges.Add(new Edge() { from = i, to = N, cap = FlowCost.Zero, cost = infCost });
                    edges.Add(new Edge() { from = N, to = i, cap = dss[i], cost = -infCost });
                    pots[i] = -infCost;
                }
                else
                {
                    edges.Add(new Edge() { from = i, to = N, cap = -dss[i], cost = -infCost });
                    edges.Add(new Edge() { from = N, to = i, cap = FlowCost.Zero, cost = infCost });
                    pots[i] = infCost;
                }
                var e = edges.Count - 2;
                parents[i] = new Parent() { p = N, e = e, up = edges[e].cap, down = edges[e ^ 1].cap };
            }
            depth[N] = 0;
            for (var i = 0; i < N + 1; ++i) Connect(i * 2, i * 2 + 1);
            for (var i = 0; i < N; ++i)
            {
                Connect(i * 2 + 1, nex[N * 2]);
                Connect(N * 2, i * 2);
            }
        }

        bool Postcompute(bool ignoreFeasibility)
        {
            for (var i = 0; i < N; ++i)
            {
                edges[parents[i].e].cap = parents[i].up;
                edges[parents[i].e ^ 1].cap = parents[i].down;
            }
            if (!ignoreFeasibility)
            {
                feasible = true;
                for (var i = 0; i < N; ++i)
                {
                    if (dss[i] >= FlowCost.Zero)
                    {
                        if (edges[M + i * 2 + 1].cap != FlowCost.Zero)
                        {
                            feasible = false;
                            break;
                        }
                    }
                    else
                    {
                        if (edges[M + i * 2].cap != FlowCost.Zero)
                        {
                            feasible = false;
                            break;
                        }
                    }
                }
                if (!feasible) return false;
            }
            for (var i = 0; i < M; i += 2)
            {
                var f = lowers[i >> 1] + edges[i ^ 1].cap;
                edges[i].flow = f;
                totalCost += f * edges[i].cost;
            }
            Array.Resize(ref pots, N);
            return true;
        }

        void PushFlow(int ei0)
        {
            var u0 = edges[ei0 ^ 1].to;
            var v0 = edges[ei0].to;
            var delU = v0;
            var flow = edges[ei0].cap;
            var clen = edges[ei0].cost + pots[u0] - pots[v0];
            var delUSide = true;
            var lca = GetLCA(u0, v0, ref flow, ref delUSide, ref delU);
            if (flow != FlowCost.Zero)
            {
                var u = u0;
                var v = v0;
                while (u != lca)
                {
                    parents[u].up += flow;
                    parents[u].down -= flow;
                    u = parents[u].p;
                }
                while (v != lca)
                {
                    parents[v].up -= flow;
                    parents[v].down += flow;
                    v = parents[v].p;
                }
            }
            {

                var u = u0;
                var par = v0;
                var pCaps1 = edges[ei0].cap - flow;
                var pCaps2 = edges[ei0 ^ 1].cap + flow;
                var pDiff = -clen;
                if (!delUSide)
                {
                    (u, par) = (par, u);
                    (pCaps1, pCaps2) = (pCaps2, pCaps1);
                    pDiff = -pDiff;
                }
                var parE = ei0 ^ (delUSide ? 0 : 1);
                while (par != delU)
                {
                    var d = depth[par];
                    var idx = u * 2;
                    while (idx != u * 2 + 1)
                    {
                        if ((idx & 1) == 0)
                        {
                            ++d;
                            pots[idx / 2] += pDiff;
                            depth[idx / 2] = d;
                        }
                        else
                        {
                            --d;
                        }
                        idx = nex[idx];
                    }
                    Connect(pre[u * 2], nex[u * 2 + 1]);
                    Connect(u * 2 + 1, nex[par * 2]);
                    Connect(par * 2, u * 2);
                    (parents[u].e, parE) = (parE, parents[u].e);
                    parE ^= 1;
                    (parents[u].up, pCaps1) = (pCaps1, parents[u].up);
                    (parents[u].down, pCaps2) = (pCaps2, parents[u].down);
                    (pCaps1, pCaps2) = (pCaps2, pCaps1);
                    var nextU = parents[u].p;
                    parents[u].p = par;
                    par = u;
                    u = nextU;
                }
                edges[parE].cap = pCaps1;
                edges[parE ^ 1].cap = pCaps2;
            }
        }

        bool Minor()
        {
            if (candidates.Count == 0) return false;
            var best = FlowCost.Zero;
            var bestEi = -1;
            var i = 0;
            while (i < candidates.Count)
            {
                var ei = candidates[i];
                if (edges[ei].cap == FlowCost.Zero)
                {
                    (candidates[i], candidates[^1]) = (candidates[^1], candidates[i]);
                    candidates.RemoveAt(candidates.Count - 1);
                    continue;
                }
                var clen = edges[ei].cost + pots[edges[ei ^ 1].to] - pots[edges[ei].to];
                if (clen >= FlowCost.Zero)
                {
                    (candidates[i], candidates[^1]) = (candidates[^1], candidates[i]);
                    candidates.RemoveAt(candidates.Count - 1);
                    continue;
                }
                if (clen < best)
                {
                    best = clen;
                    bestEi = ei;
                }
                ++i;
            }
            if (bestEi == -1) return false;
            PushFlow(bestEi);
            return true;
        }

        int GetLCA(int u, int v, ref FlowCost flow, ref bool delUSide, ref int delU)
        {
            if (depth[u] >= depth[v])
            {
                var num = depth[u] - depth[v];
                for (var i = 0; i < num; ++i)
                {
                    if (parents[u].down < flow)
                    {
                        flow = parents[u].down;
                        delU = u;
                        delUSide = true;
                    }
                    u = parents[u].p;
                }
            }
            else
            {
                var num = depth[v] - depth[u];
                for (var i = 0; i < num; ++i)
                {
                    if (parents[v].up <= flow)
                    {
                        flow = parents[v].up;
                        delU = v;
                        delUSide = false;
                    }
                    v = parents[v].p;
                }
            }
            while (u != v)
            {
                {
                    if (parents[u].down < flow)
                    {
                        flow = parents[u].down;
                        delU = u;
                        delUSide = true;
                    }
                    u = parents[u].p;
                }
                {
                    if (parents[v].up <= flow)
                    {
                        flow = parents[v].up;
                        delU = v;
                        delUSide = false;
                    }
                    v = parents[v].p;
                }
            }
            return u;
        }
    }
    ////end
}