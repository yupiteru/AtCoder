
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
    class LIB_TwoEdgeConnectedComponents : LIB_LowLink
    {
        // use LIB_LowLink
        int n;
        public int[] RevVertexies
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        List<long> edges;
        public List<int>[] Vertexies
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        public List<int>[] ShukuyakuGraph
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_TwoEdgeConnectedComponents(long n) : base(n)
        {
            this.n = (int)n;
            edges = new List<long>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void dfs(int idx, int par, ref int k)
        {
            if (par != -1 && base.ord[par] >= base.low[idx]) RevVertexies[idx] = RevVertexies[par];
            else RevVertexies[idx] = k++;
            foreach (var to in base.graph[idx])
            {
                if (RevVertexies[to] == -1) dfs(to, idx, ref k);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new public void Build()
        {
            base.Build();
            RevVertexies = new int[n];
            for (var i = 0; i < n; i++) RevVertexies[i] = -1;
            var k = 0;
            for (var i = 0; i < RevVertexies.Length; i++)
            {
                if (RevVertexies[i] == -1) dfs(i, -1, ref k);
            }
            Vertexies = new List<int>[k];
            ShukuyakuGraph = new List<int>[k];
            for (var i = 0; i < k; i++)
            {
                Vertexies[i] = new List<int>();
                ShukuyakuGraph[i] = new List<int>();
            }
            for (var i = 0; i < RevVertexies.Length; i++) Vertexies[RevVertexies[i]].Add(i);
            foreach (var edge in base.Bridges)
            {
                var f = RevVertexies[edge.from];
                var t = RevVertexies[edge.to];
                ShukuyakuGraph[f].Add(t);
                ShukuyakuGraph[t].Add(f);
            }
        }
    }
    ////end
}