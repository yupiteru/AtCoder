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
    class LIB_ModLog
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] LogBuild(long a, int mod)
        {
            var ret = new long[mod];
            ret[1] = 0;
            for (var i = 1; i < mod; i++)
            {
                ret[a] = i;
                a *= 2;
                a %= mod;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Log(long a, long b, int mod)
        {
            var g = 1L;
            for (var i = mod; i != 0; i >>= 1)
            {
                g *= a;
                g %= mod;
            }
            g = LIB_Math.GCD(g, mod);
            var t = 1L;
            var c = 0L;
            for (; t % g != 0; ++c)
            {
                if (t == b) return c;
                t *= a;
                t %= mod;
            }
            if (b % g != 0) return -1;
            t /= g;
            b /= g;
            var n = mod / g;
            var h = 0L;
            var gs = 1L;
            for (; h * h < n; ++h)
            {
                gs *= a;
                gs %= n;
            }
            var bs = new Dictionary<long, long>();
            for (long s = 0L, e = b; s < h; bs[e] = ++s)
            {
                e *= a;
                e %= n;
            }
            for (long s = 0L, e = t; s < n;)
            {
                e *= gs;
                e %= n;
                s += h;
                if (bs.ContainsKey(e) && bs[e] > 0) return c + s - bs[e];
            }
            return -1;
        }
    }
    ////end
}