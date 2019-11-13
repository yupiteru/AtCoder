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
    class LIB_UnionFind
    {
        int[] d;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_UnionFind(int s)
        {
            d = Enumerable.Repeat(-1, s).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Unite(int x, int y)
        {
            x = Root(x);
            y = Root(y);
            if (x != y)
            {
                if (d[y] < d[x])
                {
                    var t = y; y = x; x = t;
                }
                d[x] += d[y];
                d[y] = x;
            }
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(int x, int y) => Root(x) == Root(y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Root(int x) => d[x] < 0 ? x : d[x] = Root(d[x]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(int x) => -d[Root(x)];
    }
    ////end
}