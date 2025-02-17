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
    static class LIB_Convolution998244353
    {
        const int mod = 998244353;
        static public void SupersetZetaTransform(long[] a)
        {
            for (var i = 1; i < a.Length; i <<= 1)
            {
                for (var j = 0; j < a.Length; j += i << 1)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        if ((a[j + k] += a[j + i + k]) >= mod) a[j + k] -= mod;
                    }
                }
            }
        }
        static public void SupersetMebiusTransform(long[] a)
        {
            for (var i = 1; i < a.Length; i <<= 1)
            {
                for (var j = 0; j < a.Length; j += i << 1)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        if ((a[j + k] -= a[j + i + k]) < 0) a[j + k] += mod;
                    }
                }
            }
        }
        static public void SubsetZetaTransform(long[] a)
        {
            for (var i = 1; i < a.Length; i <<= 1)
            {
                for (var j = 0; j < a.Length; j += i << 1)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        if ((a[j + i + k] += a[j + k]) >= mod) a[j + i + k] -= mod;
                    }
                }
            }
        }
        static public void SubsetMebiusTransform(long[] a)
        {
            for (var i = 1; i < a.Length; i <<= 1)
            {
                for (var j = 0; j < a.Length; j += i << 1)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        if ((a[j + i + k] -= a[j + k]) < 0) a[j + i + k] += mod;
                    }
                }
            }
        }
        static int Inverse(int x) { long b = mod, r = 1, u = 0, t = 0, x2 = x; while (b > 0) { var q = x2 / b; t = u; u = r - q * u; r = t; t = b; b = x2 - q * b; x2 = t; } return (int)(r < 0 ? r + mod : r); }
        static public void HadamardTransform(long[] a, bool inv = false)
        {
            for (var i = 1; i < a.Length; i <<= 1)
            {
                for (var j = 0; j < a.Length; j += i << 1)
                {
                    for (var k = 0; k < i; ++k)
                    {
                        var x = a[j + k];
                        var y = a[j + i + k];
                        if ((a[j + k] = x + y) >= mod) a[j + k] -= mod;
                        if ((a[j + i + k] = x - y) < 0) a[j + i + k] += mod;
                    }
                }
            }
            if (inv)
            {
                var invN = Inverse(a.Length);
                for (var i = 0; i < a.Length; ++i)
                {
                    a[i] = a[i] * invN % mod;
                }
            }
        }
        static public void DivisorZetaTransform(long[] a)
        {
            foreach (var prime in LIB_Math.Primes(a.Length - 1))
            {
                var i = (a.Length - 1) / prime;
                var j = i * prime;
                while (i > 0)
                {
                    if ((a[i] += a[j]) >= mod) a[i] -= mod;
                    --i;
                    j -= prime;
                }
            }
        }
        static public void DivisorMebiusTransform(long[] a)
        {
            foreach (var prime in LIB_Math.Primes(a.Length - 1))
            {
                var i = 1;
                var j = prime;
                while (j < a.Length)
                {
                    if ((a[i] -= a[j]) < 0) a[i] += mod;
                    ++i;
                    j += prime;
                }
            }
        }
        static public void MultipleZetaTransform(long[] a)
        {
            foreach (var prime in LIB_Math.Primes(a.Length - 1))
            {
                var i = 1;
                var j = prime;
                while (j < a.Length)
                {
                    if ((a[j] += a[i]) >= mod) a[j] -= mod;
                    ++i;
                    j += prime;
                }
            }
        }
        static public void MultipleMebiusTransform(long[] a)
        {
            foreach (var prime in LIB_Math.Primes(a.Length - 1))
            {
                var i = (a.Length - 1) / prime;
                var j = i * prime;
                while (i > 0)
                {
                    if ((a[j] -= a[i]) < 0) a[j] += mod;
                    --i;
                    j -= prime;
                }
            }
        }
        static public long[] BitwiseAnd(long[] a, long[] b)
        {
            var aCopy = a.ToArray();
            var bCopy = b.ToArray();
            SupersetZetaTransform(aCopy);
            SupersetZetaTransform(bCopy);
            for (var i = 0; i < aCopy.Length; ++i)
            {
                aCopy[i] = aCopy[i] * bCopy[i] % mod;
            }
            SupersetMebiusTransform(aCopy);
            return aCopy;
        }
        static public long[] BitwiseOr(long[] a, long[] b)
        {
            var aCopy = a.ToArray();
            var bCopy = b.ToArray();
            SubsetZetaTransform(aCopy);
            SubsetZetaTransform(bCopy);
            for (var i = 0; i < aCopy.Length; ++i)
            {
                aCopy[i] = aCopy[i] * bCopy[i] % mod;
            }
            SubsetMebiusTransform(aCopy);
            return aCopy;
        }
        static public long[] BitwiseXor(long[] a, long[] b)
        {
            var aCopy = a.ToArray();
            var bCopy = b.ToArray();
            HadamardTransform(aCopy);
            HadamardTransform(bCopy);
            for (var i = 0; i < aCopy.Length; ++i)
            {
                aCopy[i] = aCopy[i] * bCopy[i] % mod;
            }
            HadamardTransform(aCopy, true);
            return aCopy;
        }
        static public long[] Gcd(long[] a, long[] b)
        {
            var aCopy = a.ToArray();
            var bCopy = b.ToArray();
            DivisorZetaTransform(aCopy);
            DivisorZetaTransform(bCopy);
            for (var i = 0; i < aCopy.Length; ++i)
            {
                aCopy[i] = aCopy[i] * bCopy[i] % mod;
            }
            DivisorMebiusTransform(aCopy);
            return aCopy;
        }
        static public long[] Lcm(long[] a, long[] b)
        {
            var aCopy = a.ToArray();
            var bCopy = b.ToArray();
            MultipleZetaTransform(aCopy);
            MultipleZetaTransform(bCopy);
            for (var i = 0; i < aCopy.Length; ++i)
            {
                aCopy[i] = aCopy[i] * bCopy[i] % mod;
            }
            MultipleMebiusTransform(aCopy);
            return aCopy;
        }
    }
    ////end
}