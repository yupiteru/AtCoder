
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
    class LIB_BiConnectedComponents : LIB_LowLink
    {
        // use LIB_LowLink
        int n;
        LIB_Deque<(int u, int v)> tmp;
        bool[] used;
        public List<List<(int u, int v)>> BunkaiEdges
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BiConnectedComponents(long n) : base(n)
        {
            this.n = (int)n;
            used = new bool[n];
            tmp = new LIB_Deque<(int u, int v)>();
            BunkaiEdges = new List<List<(int u, int v)>>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void dfs(int idx, int par)
        {
            used[idx] = true;
            foreach (var to in graph[idx])
            {
                if (to == par) continue;
                if (!used[to] || ord[to] < ord[idx])
                {
                    tmp.PushBack((Min(idx, to), Max(idx, to)));
                }
                if (!used[to])
                {
                    dfs(to, idx);
                    if (low[to] >= ord[idx])
                    {
                        var list = new List<(int u, int v)>();
                        while (true)
                        {
                            var e = tmp.PopBack();
                            list.Add(e);
                            if (e.u == Min(idx, to) && e.v == Max(idx, to)) break;
                        }
                        BunkaiEdges.Add(list);
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new public void Build()
        {
            base.Build();
            for (var i = 0; i < n; ++i)
            {
                if (!used[i]) dfs(i, -1);
            }
        }
    }
    ////end
}