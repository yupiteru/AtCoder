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
        long[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BinaryIndexedTree(long size)
        {
            n = (int)size;
            dat = new long[n + 1];
        }
        public long this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Sum(idx) - Sum(idx - 1); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Add(idx, -this[idx]); Add(idx, value); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Sum(long idx)
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
        public void Add(long idx, long value)
        {
            var i = (int)idx + 1;
            ref long datref = ref dat[0];
            while (i <= n)
            {
                Unsafe.Add(ref datref, i) += value;
                i += i & -i;
            }
        }
    }
    ////end
}