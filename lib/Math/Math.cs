using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;

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
        static public long LCM(long a, long b) => a / GCD(a, b) * b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Pow(long x, long y)
        {
            long a = 1;
            while (y != 0)
            {
                if ((y & 1) == 1) a *= x;
                if (x < long.MaxValue / x) x *= x;
                y >>= 1;
            }
            return a;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public decimal Sqrt(decimal x)
        {
            decimal prev, cur = (decimal)Math.Sqrt((double)x);
            do
            {
                prev = cur;
                if (prev == 0) return 0;
                cur = (prev + x / prev) / 2;
            } while (cur != prev);
            return cur;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<List<int>> MakePermutation(long n, bool zeroIndexed = true)
        {
            if (n <= 0) throw new Exception();
            var c = new int[n];
            var a = new int[n];
            if (!zeroIndexed) a[0] = 1;
            for (var i = 1; i < n; i++) a[i] = a[i - 1] + 1;
            yield return new List<int>(a);
            for (var i = 0; i < n;)
            {
                if (c[i] < i)
                {
                    if (i % 2 == 0)
                    {
                        var t = a[0]; a[0] = a[i]; a[i] = t;
                    }
                    else
                    {
                        var t = a[c[i]]; a[c[i]] = a[i]; a[i] = t;
                    }
                    yield return new List<int>(a);
                    ++c[i];
                    i = 0;
                }
                else
                {
                    c[i] = 0;
                    ++i;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool NextPermutation(long[] ary)
        {
            var n = ary.Length;
            var i = n - 1;
            while (i - 1 >= 0 && ary[i - 1] > ary[i]) --i;
            if (i == 0) return false;
            var j = i;
            while (j + 1 < n && ary[i - 1] < ary[j + 1]) ++j;
            var tmp = ary[i - 1]; ary[i - 1] = ary[j]; ary[j] = tmp;
            var s = i;
            var t = n - 1;
            while (t - s > 0)
            {
                tmp = ary[t]; ary[t] = ary[s]; ary[s] = tmp;
                ++s; --t;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public (long, long) InvGCD(long a, long b)
        {
            a = (a + b) % b;
            if (a == 0) return (b, 0);
            var s = b;
            var t = a;
            var m0 = 0L;
            var m1 = 1L;
            while (t > 0)
            {
                var u = s / t; s -= t * u; m0 -= m1 * u;
                var tmp = s; s = t; t = tmp;
                tmp = m0; m0 = m1; m1 = tmp;
            }
            if (m0 < 0) m0 += b / s;
            return (s, m0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public (long, long) CRT(long[] r, long[] m)
        {
            var r0 = 0L;
            var m0 = 1L;
            for (var i = 0; i < m.Length; i++)
            {
                var m1 = m[i];
                var r1 = (r[i] + m1) % m1;
                if (m0 < m1)
                {
                    r0 ^= r1; r1 ^= r0; r0 ^= r1;
                    m0 ^= m1; m1 ^= m0; m0 ^= m1;
                }
                if (m0 % m1 == 0)
                {
                    if (r0 % m1 != r1) return (0, 0);
                    continue;
                }
                var gim = InvGCD(m0, m1);
                var u1 = m1 / gim.Item1;
                if ((r1 - r0) % gim.Item1 != 0) return (0, 0);
                var x = (r1 - r0) / gim.Item1 % u1 * gim.Item2 % u1;
                r0 += x * m0;
                m0 *= u1;
                if (r0 < 0) r0 += m0;
            }
            return (r0, m0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Phi(long n)
        {
            var ret = n;
            for (var i = 2L; i * i <= n; ++i)
            {
                if (n % i == 0)
                {
                    ret -= ret / i;
                    while (n % i == 0) n /= i;
                }
            }
            if (n > 1) ret -= ret / n;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        /// <summary>
        /// a*i + b
        /// i=0...n-1
        /// 格子高さm
        /// </summary>
        static public long FloorSum(long n, long m, long a, long b)
        {
            unchecked
            {
                var ans = 0UL;
                var un = (ulong)n;
                var um = (ulong)m;
                var ua = (ulong)a;
                var ub = (ulong)b;
                if (a < 0)
                {
                    var am = a % m;
                    if (am < 0) am += m;
                    var a2 = (ulong)am;
                    ans -= un * (un - 1) / 2 * ((a2 + (ulong)(-a)) / um);
                    ua = a2;
                }
                if (b < 0)
                {
                    var bm = b % m;
                    if (bm < 0) bm += m;
                    var b2 = (ulong)bm;
                    ans -= un * ((b2 + (ulong)(-b)) / um);
                    ub = b2;
                }
                while (true)
                {
                    if (ua >= um)
                    {
                        ans += (un - 1) * un / 2 * (ua / um);
                        ua %= um;
                    }
                    if (ub >= um)
                    {
                        ans += un * (ub / um);
                        ub %= um;
                    }
                    var ymax = ua * un + ub;
                    if (ymax < um) return (long)ans;
                    un = ymax / um;
                    ub = ymax % um;
                    um ^= ua; ua ^= um; um ^= ua;
                }
            }
        }
        static public long BabyStepGiantStep<T>(T init, T target, long step, Func<T, T> babyStep, Func<T, T> giantStep) where T : IEquatable<T>
        {
            // minimal check
            var chk = init;
            for (var i = 0; i < step; ++i)
            {
                if (chk.Equals(target)) return i;
                chk = babyStep(chk);
            }

            var dic = new Dictionary<T, long>();
            chk = target;
            for (var i = 1; i <= step; ++i)
            {
                chk = babyStep(chk);
                dic[chk] = i;
            }
            var now = init;
            var fail = false;
            for (var i = 1; i <= step; ++i)
            {
                var next = giantStep(now);
                if (dic.ContainsKey(next))
                {
                    chk = now;
                    for (var j = 0; j < step; ++j)
                    {
                        if (target.Equals(chk))
                        {
                            return (i - 1) * step + j;
                        }
                        chk = babyStep(chk);
                    }
                    if (fail) return -1;
                    fail = true;
                }
                now = next;
            }
            return -1;
        }
    }
    ////end
}