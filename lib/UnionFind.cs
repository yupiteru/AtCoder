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
        long[] d;
        Func<long, long, long> f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_UnionFind(int s)
        {
            d = Enumerable.Repeat(-1L, s).ToArray();
            f = (x, y) => x + y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_UnionFind(IEnumerable<long> s, Func<long, long, long> func)
        {
            d = s.Select(e => -e).ToArray();
            f = func;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Unite(int x, int y)
        {
            x = Root(x);
            y = Root(y);
            if (x != y)
            {
                d[x] = -f(-d[x], -d[y]);
                d[y] = x;
            }
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(int x, int y) => Root(x) == Root(y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Root(int x) => d[x] < 0 ? x : (int)(d[x] = Root((int)d[x]));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Calc(int x) => -d[Root(x)];
    }
    ////end
}