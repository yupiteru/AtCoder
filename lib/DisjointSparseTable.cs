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
    class LIB_DisjointSparseTable<T>
    {
        T[][] dat;
        Func<T, T, T> _f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DisjointSparseTable(IEnumerable<T> initArray, Func<T, T, T> f)
        {
            _f = f;
            var ary = initArray.ToArray();
            ref var refary = ref ary[0];
            var n = ary.Length;
            dat = new T[Max(1, LIB_BitUtil.MSB(n - 1))][];
            dat[0] = ary;
            var baselen = 1;
            for (var k = 1; k < dat.Length; ++k)
            {
                baselen <<= 1;
                var tgtdat = dat[k] = new T[n];
                ref var reftgtdat = ref tgtdat[0];
                var flip = 1;
                var done = 0;
                var len = baselen;
                for (var i = 0; i < n; ++i)
                {
                    if (flip == 0)
                    {
                        if (i == done) Unsafe.Add(ref reftgtdat, i) = Unsafe.Add(ref refary, i);
                        else Unsafe.Add(ref reftgtdat, i) = f(Unsafe.Add(ref reftgtdat, i - 1), Unsafe.Add(ref refary, i));
                    }
                    else
                    {
                        if (i == done) Unsafe.Add(ref reftgtdat, done + len - 1) = Unsafe.Add(ref refary, done + len - 1);
                        else Unsafe.Add(ref reftgtdat, done + len - 1) = f(Unsafe.Add(ref refary, done + len - 1), Unsafe.Add(ref reftgtdat, done + len));
                    }
                    if (--len == 0)
                    {
                        done += baselen;
                        flip = 1 - flip;
                        if (done + baselen > n) len = n - done;
                        else len = baselen;
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r)
        {
            if (r <= l) return default(T);
            if (l + 1 == r) return dat[0][l];
            --r;
            var tgtdat = dat[LIB_BitUtil.MSB(l ^ r) - 1];
            return _f(tgtdat[l], tgtdat[r]);
        }
    }
    ////end
}
