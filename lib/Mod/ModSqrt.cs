using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using MathNet.Numerics.Random;

namespace Library
{
    ////start
    class LIB_ModSqrt
    {
        static long pow(long x, long y, int mod)
        {
            var a = 1L;
            while (y > 0)
            {
                if ((y & 1) != 0) a = a * x % mod;
                x = x * x % mod;
                y >>= 1;
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Sqrt(long x, int mod)
        {
            if (mod == 2) return x;
            if (x == 0) return 0;
            var k = (mod - 1) / 2;
            if (pow(x, k, mod) != 1) return -1;
            var rnd = new Random(0);
            var b = 0L;
            var D = 0L;
            while (true)
            {
                b = rnd.Next(2, mod);
                D = (b * b - x + mod) % mod;
                if (D == 0) return b;
                if (pow(D, k, mod) != 1) break;
            }
            ++k;
            var f0 = b;
            var f1 = 1L;
            var g0 = 1L;
            var g1 = 0L;
            while (k > 0)
            {
                if ((k & 1) != 0)
                {
                    var t = (f1 * g0 + f0 * g1) % mod;
                    g0 = (f0 * g0 + (D * f1 % mod) * g1) % mod;
                    g1 = t;
                }
                {
                    var t = (2 * f0 * f1) % mod;
                    f0 = (f0 * f0 + (D * f1 % mod) * f1) % mod;
                    f1 = t;
                }
                k >>= 1;
            }
            if (g0 * g0 % mod != x) return -1;
            return g0;
        }
    }
    ////end
}