
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
    class LIB_BlockCutTree : LIB_BiConnectedComponents
    {
        // use LIB_BiConnectedComponents
        int n;
        public int[] RevVertexies
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        public List<int>[] Path
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        public List<int>[] Group
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BlockCutTree(long n) : base(n)
        {
            this.n = (int)n;
            RevVertexies = new int[n];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new public void Build()
        {
            base.Build();
            var ptr = BunkaiEdges.Count;
            foreach (var item in base.Articulations)
            {
                RevVertexies[item] = ptr++;
            }
            Path = Enumerable.Repeat(0, ptr).Select(_ => new List<int>()).ToArray();
            var last = new int[ptr];
            for (var i = 0; i < last.Length; ++i) last[i] = -1;
            for (var i = 0; i < BunkaiEdges.Count; ++i)
            {
                foreach (var item in BunkaiEdges[i])
                {
                    Action<int> act = vtx =>
                    {
                        if (RevVertexies[vtx] >= BunkaiEdges.Count)
                        {
                            var tmp = last[RevVertexies[vtx]];
                            last[RevVertexies[vtx]] = i;
                            if (tmp != i)
                            {
                                Path[RevVertexies[vtx]].Add(i);
                                Path[i].Add(RevVertexies[vtx]);
                            }
                        }
                        else
                        {
                            RevVertexies[vtx] = i;
                        }
                    };
                    act(item.u);
                    act(item.v);
                }
            }
            Group = Enumerable.Repeat(0, ptr).Select(_ => new List<int>()).ToArray();
            for (var i = 0; i < RevVertexies.Length; ++i)
            {
                Group[RevVertexies[i]].Add(i);
            }
        }
    }
    ////end
}