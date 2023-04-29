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
    class LIB_FPS
    {
        const uint MOD = 998244353;
        uint[] ary;
        public int K
        {
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS(long K)
        {
            this.K = (int)K;
            ary = new uint[K + 1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS(long[] a)
        {
            this.K = a.Length - 1;
            ary = a.Select(e => (uint)(e % MOD)).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Clone()
        {
            var ret = new LIB_FPS(K);
            ret.ary = (uint[])ary.Clone();
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator +(LIB_FPS x, LIB_FPS y)
        {
            var ret = new LIB_FPS(Max(x.K, y.K));
            for (var i = 0; i < x.ary.Length; ++i) ret.ary[i] += x.ary[i];
            for (var i = 0; i < y.ary.Length; ++i)
            {
                var sum = ret.ary[i] + y.ary[i];
                ret.ary[i] = sum >= MOD ? MOD - sum : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator +(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] + y;
            ret[0] = sum >= MOD ? MOD - sum : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator -(LIB_FPS x, LIB_FPS y)
        {
            var ret = new LIB_FPS(Max(x.K, y.K));
            for (var i = 0; i < x.ary.Length; ++i) ret.ary[i] += x.ary[i];
            for (var i = 0; i < y.ary.Length; ++i)
            {
                var sum = ret.ary[i] + MOD - y.ary[i];
                ret.ary[i] = sum >= MOD ? MOD - sum : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator -(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] + MOD - y;
            ret[0] = sum >= MOD ? MOD - sum : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator *(LIB_FPS x, LIB_FPS y)
        {
            return new LIB_FPS(LIB_NTT.Multiply(x.ary.Select(e => (long)e).ToArray(), y.ary.Select(e => (long)e).ToArray()));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator *(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            for (var i = 0; i < ret.ary.Length; ++i) ret.ary[i] = (uint)(y * ret.ary[i] % MOD);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator /(LIB_FPS x, long y) => x * LIB_Mod998244353.Inverse(y);
        /// <summary>
        /// べき乗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Pow(long M)
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
            var powc = LIB_Mod998244353.Pow(ary[l], M);
            var invc = LIB_Mod998244353.Inverse(ary[l]);
            var g = new LIB_FPS(K - l);
            for (var i = l; i < ary.Length; ++i) g.ary[i - l] = (uint)(ary[i] * invc % MOD);
            g.Log_inplace();
            for (var i = 0; i < g.ary.Length; ++i) g.ary[i] = (uint)((M % MOD) * g.ary[i] % MOD);
            g.Exp_inplace();
            var ten = l * M;
            for (var i = ary.Length - 1; i >= ten; --i) ary[i] = (uint)(powc * g.ary[i - ten] % MOD);
            for (var i = 0; i < ten; ++i) ary[i] = 0;
        }
        /// <summary>
        /// 指数 (a0 == 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Exp()
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
            var maxlen = 2;
            while ((maxlen << 1) <= K) maxlen <<= 1;
            var g = new uint[maxlen];
            var inv = new long[maxlen * 2];
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            Span<uint> nttg = new uint[2];
            g[0] = 1;
            nttg[0] = 1;
            nttg[1] = 1;
            ary[0] = 1;
            var h_drv = Differential();

            var len = 2;
            while (len <= K)
            {
                var nextlen = len * 2;
                Span<uint> nttf = new uint[nextlen];
                for (var i = 0; i < len; ++i) nttf[i] = ary[i];
                LIB_NTT.ntt4(ref nttf);

                {
                    Span<uint> ntth = new uint[len];
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)nttf[i * 2] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, true);
                    for (var i = 0; i < len / 2; ++i) ntth[i] = ntth[i + len / 2];
                    for (var i = len / 2; i < len; ++i) ntth[i] = 0;
                    LIB_NTT.ntt4(ref ntth);
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, true);
                    for (var i = len / 2; i < len; ++i) g[i] = (ntth[i - len / 2] == 0 ? 0 : MOD - ntth[i - len / 2]);
                }

                Span<uint> t = new uint[len];
                {
                    Span<uint> ntth = new uint[len];
                    for (var i = 0; i < len - 1; ++i) ntth[i] = h_drv.ary[i];
                    LIB_NTT.ntt4(ref ntth);
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttf[i * 2] % MOD);
                    LIB_NTT.ntt4(ref ntth, true);
                    for (var i = 1; i < len; ++i) t[i] = (uint)(((ulong)i * ary[i] + MOD - ntth[i - 1]) % MOD);
                    t[0] = ntth[len - 1] == 0 ? 0 : MOD - ntth[len - 1];
                }
                if (2 * len <= K)
                {
                    Span<uint> newt = new uint[nextlen];
                    for (var i = 0; i < len; ++i) newt[i] = t[i];
                    LIB_NTT.ntt4(ref newt);
                    nttg = new uint[nextlen];
                    for (var i = 0; i < len; ++i) nttg[i] = g[i];
                    LIB_NTT.ntt4(ref nttg);
                    for (var i = 0; i < nextlen; ++i) newt[i] = (uint)((ulong)newt[i] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref newt, true);
                    for (var i = 0; i < len; ++i) t[i] = newt[i];
                }
                else
                {
                    Span<uint> g1 = new uint[len];
                    Span<uint> s1 = new uint[len];
                    for (var i = 0; i < len / 2; ++i) g1[i] = g[i + len / 2];
                    for (var i = 0; i < len / 2; ++i) s1[i] = t[i + len / 2];
                    for (var i = len / 2; i < len; ++i) t[i] = 0;
                    LIB_NTT.ntt4(ref g1);
                    LIB_NTT.ntt4(ref s1);
                    LIB_NTT.ntt4(ref t);
                    for (var i = 0; i < len; ++i)
                    {
                        s1[i] = (uint)(((ulong)nttg[i] * s1[i] + (ulong)g1[i] * t[i]) % MOD);
                        t[i] = (uint)((ulong)nttg[i] * t[i] % MOD);
                    }
                    LIB_NTT.ntt4(ref t, true);
                    LIB_NTT.ntt4(ref s1, true);
                    for (var i = len / 2; i < len; ++i) t[i] = (t[i] + s1[i - len / 2]) % MOD;
                }

                {
                    Span<uint> ntth = new uint[nextlen];
                    var ten = Min(ary.Length, 2 * len);
                    for (var i = len; i < ten; ++i) ntth[i - len] = ary[i];
                    for (var i = 0; i < len; ++i) ntth[i] = (ntth[i] + MOD - (uint)(inv[i + len] * t[i] % MOD)) % MOD;

                    LIB_NTT.ntt4(ref ntth);
                    for (var i = 0; i < nextlen; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttf[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, true);

                    ten = Min(ary.Length - len, len);
                    for (var i = 0; i < ten; ++i) ary[i + len] = ntth[i];
                }

                len = nextlen;
            }
        }
        /// <summary>
        /// 対数 (a0 == 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Log()
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
            var f2 = Differential();
            Inverse_inplace();
            var len = 1;
            while (len < ary.Length + f2.ary.Length - 1) len <<= 1;
            Span<uint> nttdiff = new uint[len];
            Span<uint> nttinv = new uint[len];
            for (var i = 0; i < f2.ary.Length; ++i) nttdiff[i] = f2.ary[i];
            for (var i = 0; i < ary.Length; ++i) nttinv[i] = ary[i];
            LIB_NTT.ntt4(ref nttdiff);
            LIB_NTT.ntt4(ref nttinv);
            for (var i = 0; i < nttinv.Length; ++i) nttinv[i] = (uint)((long)nttinv[i] * nttdiff[i] % MOD);
            LIB_NTT.ntt4(ref nttinv, true);
            for (var i = 0; i < ary.Length; ++i) ary[i] = nttinv[i];
            Integral_inplace();
        }
        /// <summary>
        /// 微分
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Differential()
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
        public LIB_FPS Integral()
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
        public LIB_FPS Inverse()
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
            var maxlen = 1;
            while (maxlen <= K) maxlen <<= 1;
            var g = new uint[maxlen];
            g[0] = (uint)LIB_Mod998244353.Inverse((long)ary[0]);
            var len = 1;
            while (len <= K)
            {
                var nextlen = len << 1;
                Span<uint> nttf = new uint[nextlen];
                Span<uint> nttg = new uint[nextlen];
                var ten = Min(nextlen, ary.Length);
                for (var i = 0; i < ten; ++i) nttf[i] = ary[i];
                for (var i = 0; i < len; ++i) nttg[i] = g[i];
                LIB_NTT.ntt4(ref nttf);
                LIB_NTT.ntt4(ref nttg);
                for (var i = 0; i < nextlen; ++i) nttf[i] = (uint)((ulong)nttf[i] * nttg[i] % MOD);
                LIB_NTT.ntt4(ref nttf, true);
                for (var i = 0; i < len; ++i) nttf[i] = 0;
                LIB_NTT.ntt4(ref nttf);
                for (var i = 0; i < nextlen; ++i) nttf[i] = (uint)((ulong)nttf[i] * nttg[i] % MOD);
                LIB_NTT.ntt4(ref nttf, true);
                for (var i = len; i < nextlen; ++i) g[i] = (nttf[i] == 0 ? 0 : MOD - nttf[i]);
                len = nextlen;
            }
            ary = g;
        }
        public long this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (long)ary[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { ary[index] = (uint)value % MOD; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Join(string separator = "") => string.Join(separator, ary);
    }
    ////end
}