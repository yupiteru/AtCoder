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
    class LIB_MillerRabin
    {
        static readonly ulong[] a1 = new ulong[] { 2, 7, 61 };
        static readonly ulong[] a2 = new ulong[] { 2, 325, 9375, 28178, 450775, 9780504, 1795265022 };
        static readonly byte[] smallPrimes = new byte[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251 };
        static ulong N;
        static ulong NEG_INV;
        static public bool IsPrime(ulong N)
        {
            LIB_MillerRabin.N = N;
            if (N < 66049)
            {
                var n = (int)N;
                if (n < 2) return false;
                if ((n & 1) == 0) return n == 2;
                if (n == 3) return true;
                foreach (int p in smallPrimes)
                {
                    if (p * p > n) break;
                    if (n % p == 0) return false;
                }
                return true;
            }
            if ((N & 1) == 0) return false;
            if (1000000000 < N) return IsPrime64();
            var br = new LIB_BarrettReduction(N);
            var d = N - 1;
            var cnt = 0;
            while ((d & 1) == 0)
            {
                d >>= 1;
                ++cnt;
            }
            foreach (var item in a1)
            {
                var x = 1UL;
                var d2 = d;
                var num = item;
                while (d2 > 0)
                {
                    if ((d2 & 1) == 1) x = br.Reduce(x * num);
                    d2 >>= 1;
                    num = br.Reduce(num * num);
                }

                var i = 0;
                for (; i < cnt; ++i)
                {
                    if (x == 1 || x == N - 1) break;
                    x = br.Reduce(x * x);
                }
                if (i > 0 && x != N - 1) return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong reduce(UInt128 v) => (ulong)((v + (UInt128)((ulong)v * NEG_INV) * N) >> 64);
        static bool IsPrime64()
        {
            unchecked
            {
                var T128 = (ulong)(-(UInt128)N % N);
                var INV_MOD = N;
                INV_MOD *= 2 - N * INV_MOD;
                INV_MOD *= 2 - N * INV_MOD;
                INV_MOD *= 2 - N * INV_MOD;
                INV_MOD *= 2 - N * INV_MOD;
                INV_MOD *= 2 - N * INV_MOD;
                NEG_INV = ~INV_MOD + 1;

                var ONE = reduce(T128);
                var MINUS_ONE = reduce(((UInt128)N - 1) * T128);
                if (ONE >= N) ONE -= N;
                if (MINUS_ONE >= N) MINUS_ONE -= N;

                var d = N - 1;
                var cnt = 0;
                while ((d & 1) == 0)
                {
                    d >>= 1;
                    ++cnt;
                }
                foreach (var item in a2)
                {
                    var x = ONE;
                    var d2 = d;
                    var num = reduce((UInt128)item * T128);
                    while (d2 > 0)
                    {
                        if ((d2 & 1) == 1) x = reduce((UInt128)x * num);
                        d2 >>= 1;
                        num = reduce((UInt128)num * num);
                    }

                    var i = 0;
                    var v = x >= N ? x - N : x;
                    for (; i < cnt; ++i)
                    {
                        if (v == ONE || v == MINUS_ONE) break;
                        x = reduce((UInt128)x * x);
                        v = x >= N ? x - N : x;
                    }
                    if (i > 0 && v != MINUS_ONE) return false;
                }
                return true;
            }
        }
    }
    ////end
}