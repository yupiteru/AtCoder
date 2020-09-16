
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
    class LIB_LowLink
    {
        int n;
        protected List<int>[] graph;
        bool[] used;
        protected int[] ord;
        protected int[] low;
        public List<int> Articulations
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        public struct Edge
        {
            public int from;
            public int to;
        }
        public List<Edge> Bridges
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LowLink(long n)
        {
            this.n = (int)n;
            graph = new List<int>[n];
            Articulations = new List<int>();
            Bridges = new List<Edge>();
            for (var i = 0; i < n; i++) graph[i] = new List<int>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEdge(long u, long v)
        {
            graph[u].Add((int)v);
            graph[v].Add((int)u);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int dfs(int idx, int k, int par)
        {
            used[idx] = true;
            low[idx] = ord[idx] = k++;
            var isArticulation = false;
            var cnt = 0;
            foreach (var to in graph[idx])
            {
                if (!used[to])
                {
                    ++cnt;
                    k = dfs(to, k, idx);
                    low[idx] = Math.Min(low[idx], low[to]);
                    isArticulation |= par != -1 && low[to] >= ord[idx];
                    if (ord[idx] < low[to]) Bridges.Add(new Edge { from = Math.Min(idx, to), to = Math.Max(idx, to) });
                }
                else if (to != par) low[idx] = Math.Min(low[idx], ord[to]);
            }
            isArticulation |= par == -1 && cnt > 1;
            if (isArticulation) Articulations.Add(idx);
            return k;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build()
        {
            used = new bool[n];
            ord = new int[n];
            low = new int[n];
            var k = 0;
            for (var i = 0; i < used.Length; i++)
            {
                if (!used[i]) k = dfs(i, k, -1);
            }
        }
    }
    ////end
}