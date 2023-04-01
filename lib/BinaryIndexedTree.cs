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
    class LIB_BinaryIndexedTree
    {
        int n;
        int beki;
        long[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BinaryIndexedTree(long size)
        {
            n = (int)size;
            beki = 1;
            while ((beki << 1) <= n) beki <<= 1;
            dat = new long[n + 1];
        }
        public long this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Sum_0indexed(idx) - Sum_0indexed(idx - 1); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Add_0indexed(idx, -this[idx]); Add_0indexed(idx, value); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Sum_0indexed(long idx)
        {
            var ret = 0L;
            var i = (int)idx + 1;
            ref long datref = ref dat[0];
            while (i > 0)
            {
                ret += Unsafe.Add(ref datref, i);
                i -= i & -i;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add_0indexed(long idx, long value)
        {
            var i = (int)idx + 1;
            ref long datref = ref dat[0];
            while (i <= n)
            {
                Unsafe.Add(ref datref, i) += value;
                i += i & -i;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LowerBound(long value)
        {
            if (value <= 0) return -1;
            var x = 0;
            ref long datref = ref dat[0];
            for (var k = beki; k > 0; k >>= 1)
            {
                var xk = x + k;
                if (xk <= n)
                {
                    var v = Unsafe.Add(ref datref, xk);
                    if (v < value)
                    {
                        value -= v;
                        x = xk;
                    }
                }
            }
            return x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long UpperBound(long value)
        {
            if (value < 0) return -1;
            var x = 0;
            ref long datref = ref dat[0];
            for (var k = beki; k > 0; k >>= 1)
            {
                var xk = x + k;
                if (xk <= n)
                {
                    var v = Unsafe.Add(ref datref, xk);
                    if (v <= value)
                    {
                        value -= v;
                        x = xk;
                    }
                }
            }
            return x;
        }
    }
    ////end
}