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
    class LIB_Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<long> Primes(long x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (long)(Math.Sqrt(x) / 2);
            for (long i = 1; i <= max; ++i)
            {
                if (table[i]) continue;
                var add = 2 * i + 1;
                yield return add;
                for (long j = 2 * i * (i + 1); j <= halfx; j += add)
                    table[j] = true;
            }
            for (long i = max + 1; i <= halfx; ++i)
                if (!table[i] && 2 * i + 1 <= x)
                    yield return 2 * i + 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<long> Factors(long x)
        {
            if (x < 2) yield break;
            while (x % 2 == 0)
            {
                x /= 2; yield return 2;
            }
            var max = (long)Math.Sqrt(x);
            for (long i = 3; i <= max; i += 2)
            {
                while (x % i == 0)
                {
                    x /= i; yield return i;
                }
            }
            if (x != 1) yield return x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i <= max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long GCD(long a, long b)
        {
            while (b > 0)
            {
                var tmp = b;
                b = a % b;
                a = tmp;
            }
            return a;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long LCM(long a, long b) => a * b / GCD(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Pow(long x, long y)
        {
            long a = 1;
            while (y != 0)
            {
                if ((y & 1) == 1) a *= x;
                x *= x;
                y >>= 1;
            }
            return a;
        }
        static List<long> _fact = new List<long>() { 1 };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Build(long n)
        {
            if (n >= _fact.Count)
                for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Comb(long n, long k)
        {
            Build(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Perm(long n, long k)
        {
            Build(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[(int)n] / _fact[(int)(n - k)];
        }
    }
    ////end
}