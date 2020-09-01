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
    class LIB_UnionFind<T>
    {
        int[] d;
        T[] v;
        Func<T, T, T> f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_UnionFind(IEnumerable<T> s, Func<T, T, T> func)
        {
            v = s.ToArray();
            d = Enumerable.Repeat(-1, v.Length).ToArray();
            f = func;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Unite(long x, long y)
        {
            x = Root(x);
            y = Root(y);
            if (x != y)
            {
                d[x] = d[x] + d[y];
                v[x] = f(v[x], v[y]);
                d[y] = (int)x;
            }
            return x != y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(long x, long y) => Root(x) == Root(y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Root(long x) => d[x] < 0 ? x : d[x] = (int)Root(d[x]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Count(long x) => -d[Root(x)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Calc(long x) => v[Root(x)];
    }
    class LIB_UnionFind : LIB_UnionFind<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_UnionFind(long n) : base(Enumerable.Repeat(0, (int)n), (x, y) => x + y) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        new private void Calc(long x) { }
    }
    ////end
}