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
    class LIB_SparseTableMin
    {
        long[][] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SparseTableMin(IEnumerable<long> initArray)
        {
            var ary = initArray.ToArray();
            var n = ary.Length;
            dat = new long[Max(1, LIB_BitUtil.MSB(n - 1))][];
            dat[0] = ary;
            var len = 1;
            for (var k = 1; k < dat.Length; ++k)
            {
                var orgdat = dat[k - 1];
                var tgtdat = dat[k] = new long[n];
                for (var i = 0; i < n - len; ++i)
                {
                    tgtdat[i] = Min(orgdat[i], orgdat[i + len]);
                }
                len <<= 1;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Query(long l, long r)
        {
            if (r <= l) return long.MaxValue;
            var shiftbit = Max(0, LIB_BitUtil.MSB(r - l - 1) - 1);
            var tgtdat = dat[shiftbit];
            return Min(tgtdat[l], tgtdat[r - (1 << shiftbit)]);
        }
    }
    class LIB_SparseTableMax
    {
        long[][] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SparseTableMax(IEnumerable<long> initArray)
        {
            var ary = initArray.ToArray();
            var n = ary.Length;
            dat = new long[Max(1, LIB_BitUtil.MSB(n - 1))][];
            dat[0] = ary;
            var len = 1;
            for (var k = 1; k < dat.Length; ++k)
            {
                var orgdat = dat[k - 1];
                var tgtdat = dat[k] = new long[n];
                for (var i = 0; i < n - len; ++i)
                {
                    tgtdat[i] = Max(orgdat[i], orgdat[i + len]);
                }
                len <<= 1;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Query(long l, long r)
        {
            if (r <= l) return long.MinValue;
            var shiftbit = Max(0, LIB_BitUtil.MSB(r - l - 1) - 1);
            var tgtdat = dat[shiftbit];
            return Max(tgtdat[l], tgtdat[r - (1 << shiftbit)]);
        }
    }
    ////end
}
