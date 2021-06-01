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
    class LIB_BarrettReduction
    {
        ulong MOD;
        ulong mh;
        ulong ml;
        const ulong ALL1_32 = (ulong)~0U;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_BarrettReduction(ulong mod)
        {
            MOD = mod;
            var m = ~0UL / MOD;
            unchecked { if (m * MOD + MOD == 0) ++m; }
            mh = m >> 32;
            ml = m & ALL1_32;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Reduce(ulong x)
        {
            var z = (x & ALL1_32) * ml;
            z = (x & ALL1_32) * mh + (x >> 32) * ml + (z >> 32);
            z = (x >> 32) * mh + (z >> 32);
            x -= z * MOD;
            return x < MOD ? x : x - MOD;
        }
    }
    ////end
}