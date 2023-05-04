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
    class LIB_FPSFreeMod
    {
        public static uint MOD;
        uint[] ary;
        public int K
        {
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod(long K)
        {
            this.K = (int)K;
            ary = new uint[K + 1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod(long K, int[] a) : this(K, a.Select(e => (long)e).ToArray())
        {
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod(long K, long[] a) : this(K)
        {
            var ten = Min(ary.Length, a.Length);
            for (var i = 0; i < ten; ++i)
            {
                var val = a[i] % MOD;
                if (val < 0) val += MOD;
                ary[i] = (uint)val;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Clone()
        {
            var ret = new LIB_FPSFreeMod(K);
            ret.ary = (uint[])ary.Clone();
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long InverseMod(long x)
        {
            long b = MOD, r = 1, u = 0, t = 0;
            while (b > 0)
            {
                var q = x / b;
                t = u;
                u = r - q * u;
                r = t;
                t = b;
                b = x - q * b;
                x = t;
            }
            return r < 0 ? r + MOD : r;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long PowMod(long x, long y)
        {
            long a = 1;
            while (y != 0)
            {
                if ((y & 1) == 1) a = a * x % MOD;
                x = x * x % MOD;
                y >>= 1;
            }
            return a;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator +(LIB_FPSFreeMod x, LIB_FPSFreeMod y)
        {
            var ret = new LIB_FPSFreeMod(Max(x.K, y.K));
            for (var i = 0; i < x.ary.Length; ++i) ret.ary[i] += x.ary[i];
            for (var i = 0; i < y.ary.Length; ++i)
            {
                var sum = ret.ary[i] + y.ary[i];
                ret.ary[i] = sum >= MOD ? sum - MOD : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator +(LIB_FPSFreeMod x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] + y;
            ret[0] = sum >= MOD ? sum - MOD : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator -(LIB_FPSFreeMod x, LIB_FPSFreeMod y)
        {
            var ret = new LIB_FPSFreeMod(Max(x.K, y.K));
            for (var i = 0; i < x.ary.Length; ++i) ret.ary[i] += x.ary[i];
            for (var i = 0; i < y.ary.Length; ++i)
            {
                var sum = ret.ary[i] + MOD - y.ary[i];
                ret.ary[i] = sum >= MOD ? sum - MOD : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator -(LIB_FPSFreeMod x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] + MOD - y;
            ret[0] = sum >= MOD ? sum - MOD : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator *(LIB_FPSFreeMod x, LIB_FPSFreeMod y)
        {
            return new LIB_FPSFreeMod(Max(x.K, y.K), LIB_NTT.Multiply(x.ary.Select(e => (long)e).ToArray(), y.ary.Select(e => (long)e).ToArray(), MOD));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator *(LIB_FPSFreeMod x, long y)
        {
            var ret = x.Clone();
            for (var i = 0; i < ret.ary.Length; ++i) ret.ary[i] = (uint)(y * ret.ary[i] % MOD);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator /(LIB_FPSFreeMod x, LIB_FPSFreeMod y) => x * y.Inverse();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPSFreeMod operator /(LIB_FPSFreeMod x, long y) => x * InverseMod(y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long BostanMori(long N, LIB_FPSFreeMod nume, LIB_FPSFreeMod deno)
        {
            var p = nume.ary.Select(e => (long)e).ToArray();
            var q = deno.ary.Select(e => (long)e).ToArray();
            while (N > 0)
            {
                var flipQ = q.ToArray();
                for (var i = 1; i < flipQ.Length; i += 2) flipQ[i] = flipQ[i] == 0 ? 0 : MOD - flipQ[i];
                var tmp = LIB_NTT.Multiply(p, flipQ, MOD);
                p = new long[(tmp.Length + (~N & 1)) / 2];
                for (var i = 0; i < p.Length; ++i) p[i] = tmp[i * 2 + (N & 1)];
                tmp = LIB_NTT.Multiply(q, flipQ, MOD);
                q = new long[(tmp.Length + 1) / 2];
                for (var i = 0; i < q.Length; ++i) q[i] = tmp[i * 2];
                N /= 2;
            }
            return p[0] * InverseMod(q[0]) % MOD;
        }
        /// <summary>
        /// べき乗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Pow(long M)
        {
            var ret = Clone();
            ret.Pow_inplace(M);
            return ret;
        }
        /// <summary>
        /// べき乗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pow_inplace(long M)
        {
            if (M == 0)
            {
                for (var i = 1; i < ary.Length; ++i) ary[i] = 0;
                ary[0] = 1;
                return;
            }
            var l = 0;
            while (l < ary.Length && ary[l] == 0) ++l;
            if (l == ary.Length || l > (ary.Length - 1) / M)
            {
                for (var i = 0; i < ary.Length; ++i) ary[i] = 0;
                return;
            }
            var powc = PowMod(ary[l], M);
            var invc = InverseMod(ary[l]);
            var g = new LIB_FPSFreeMod(K - l);
            for (var i = l; i < ary.Length; ++i) g.ary[i - l] = (uint)(ary[i] * invc % MOD);

            var dat = new List<(int idx, long val)>();
            for (var i = 1; i < g.ary.Length; ++i) if (g.ary[i] != 0) dat.Add((i, g.ary[i]));

            var ten = l * M;
            M %= MOD;
            if (dat.Count > 100)
            {
                g.Log_inplace_dense();
                for (var i = 0; i < g.ary.Length; ++i) g.ary[i] = (uint)(M * g.ary[i] % MOD);
                g.Exp_inplace_dense();
            }
            else
            {
                var inv = new long[g.ary.Length];
                inv[1] = 1;
                for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;

                for (var n = 0; n < g.ary.Length - 1; ++n)
                {
                    g.ary[n + 1] = 0;
                    foreach (var item in dat)
                    {
                        if (item.idx > n + 1) break;
                        var t = item.val * g.ary[n - item.idx + 1] % MOD;
                        t = t * ((M * item.idx % MOD) - n + item.idx - 1) % MOD;
                        if (t < 0) t += MOD;
                        g.ary[n + 1] += (uint)t;
                        if (g.ary[n + 1] >= MOD) g.ary[n + 1] -= MOD;
                    }
                    g.ary[n + 1] = (uint)(g.ary[n + 1] * inv[n + 1] % MOD);
                }
            }
            for (var i = ary.Length - 1; i >= ten; --i) ary[i] = (uint)(powc * g.ary[i - ten] % MOD);
            for (var i = 0; i < ten; ++i) ary[i] = 0;
        }
        /// <summary>
        /// 指数 (a0 == 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Exp()
        {
            var ret = Clone();
            ret.Exp_inplace();
            return ret;
        }
        /// <summary>
        /// 指数 (a0 == 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exp_inplace()
        {
            var dat = new List<(int idx, long val)>();
            for (var i = 1; i < ary.Length; ++i) if (ary[i] != 0) dat.Add((i - 1, (long)i * ary[i] % MOD));
            if (dat.Count > 320)
            {
                Exp_inplace_dense();
                return;
            }

            // sparse
            var inv = new long[ary.Length + 1];
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            ary[0] = 1;
            for (var n = 1; n < ary.Length; ++n)
            {
                var rhs = 0L;
                foreach (var item in dat)
                {
                    if (item.idx > n - 1) break;
                    rhs += item.val * ary[n - 1 - item.idx];
                }
                ary[n] = (uint)((rhs % MOD) * inv[n] % MOD);
            }
        }
        /// <summary>
        /// 指数 (a0 == 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exp_inplace_dense()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 対数 (a0 == 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Log()
        {
            var ret = Clone();
            ret.Log_inplace();
            return ret;
        }
        /// <summary>
        /// 対数 (a0 == 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log_inplace()
        {
            if (K == 0)
            {
                ary[0] = 0;
                return;
            }
            var dat = new List<(int idx, uint val)>();
            for (var i = 1; i < ary.Length; ++i) if (ary[i] != 0) dat.Add((i, ary[i]));
            if (dat.Count > 160)
            {
                Log_inplace_dense();
                return;
            }

            // sparse
            var g = new long[ary.Length - 1];
            var inv = new long[ary.Length];
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            ary[0] = 0;
            for (var n = 0; n < ary.Length - 1; ++n)
            {
                var rhs = (long)(n + 1) * ary[n + 1] % MOD;
                foreach (var item in dat)
                {
                    if (item.idx > n) break;
                    rhs = rhs - item.val * g[n - item.idx] % MOD;
                    if (rhs < 0) rhs += MOD;
                }
                g[n] = rhs;
                ary[n + 1] = (uint)(rhs * inv[n + 1] % MOD);
            }
        }
        /// <summary>
        /// 対数 (a0 == 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Log_inplace_dense()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 微分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Differential()
        {
            var ret = Clone();
            ret.Differential_inplace();
            return ret;
        }
        /// <summary>
        /// 微分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Differential_inplace()
        {
            if (K == 0)
            {
                ary[0] = 0;
                return;
            }
            for (var i = 1L; i < ary.Length; ++i) ary[i - 1] = (uint)(i * ary[i] % MOD);
            ary[ary.Length - 1] = 0;
        }
        /// <summary>
        /// 積分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Integral()
        {
            var ret = Clone();
            ret.Integral_inplace();
            return ret;
        }
        /// <summary>
        /// 積分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Integral_inplace()
        {
            var inv = new long[ary.Length];
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            for (var i = ary.Length - 1; i > 0; --i) ary[i] = (uint)(inv[i] * ary[i - 1] % MOD);
            ary[0] = 0;
        }
        /// <summary>
        /// 逆元 (a0 != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPSFreeMod Inverse()
        {
            var ret = Clone();
            ret.Inverse_inplace();
            return ret;
        }
        /// <summary>
        /// 逆元 (a0 != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse_inplace()
        {
            var dat = new List<(int idx, long val)>();
            for (var i = 1; i < ary.Length; ++i) if (ary[i] != 0) dat.Add((i, ary[i]));
            if (dat.Count > 160)
            {
                Inverse_inplace_dense();
                return;
            }

            // sparse
            ary[0] = (uint)InverseMod(ary[0]);
            for (var n = 1; n < ary.Length; ++n)
            {
                var rhs = 0L;
                foreach (var item in dat)
                {
                    if (item.idx > n) break;
                    rhs -= item.val * ary[n - item.idx];
                }
                rhs = (rhs % MOD) * ary[0] % MOD;
                if (rhs < 0) rhs += MOD;
                ary[n] = (uint)rhs;
            }
        }
        /// <summary>
        /// 逆元 (a0 != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse_inplace_dense()
        {
            throw new NotImplementedException();
        }
        public long this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (long)ary[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var v = value % MOD;
                ary[index] = (uint)(v < 0 ? v + MOD : v);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Join(string separator = "") => string.Join(separator, ary);
    }
    ////end
}