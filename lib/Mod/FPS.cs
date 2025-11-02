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
        public delegate LIB_FPS FPSBuilder(params long[] a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public FPSBuilder MakeBuilder(long K) => new FPSBuilder(a => new LIB_FPS(K, a));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS(long K)
        {
            this.K = (int)K;
            ary = new uint[K + 1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS(long K, int[] a) : this(K, a.Select(e => (long)e).ToArray())
        {
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS(long K, long[] a) : this(K)
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
                ret.ary[i] = sum >= MOD ? sum - MOD : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator +(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] + y;
            sum %= MOD;
            ret[0] = sum < 0 ? sum + MOD : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator +(long x, LIB_FPS y) => y + x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator -(LIB_FPS x, LIB_FPS y)
        {
            var ret = new LIB_FPS(Max(x.K, y.K));
            for (var i = 0; i < x.ary.Length; ++i) ret.ary[i] += x.ary[i];
            for (var i = 0; i < y.ary.Length; ++i)
            {
                var sum = ret.ary[i] + MOD - y.ary[i];
                ret.ary[i] = sum >= MOD ? sum - MOD : sum;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator -(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            var sum = ret[0] - y;
            sum %= MOD;
            ret[0] = sum < 0 ? sum + MOD : sum;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator -(long x, LIB_FPS y) => -1 * (y - x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator *(LIB_FPS x, LIB_FPS y)
        {
            return new LIB_FPS(Max(x.K, y.K), LIB_NTT.Multiply(x.ary.Select(e => (long)e).ToArray(), y.ary.Select(e => (long)e).ToArray()));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator *(LIB_FPS x, long y)
        {
            var ret = x.Clone();
            for (var i = 0; i < ret.ary.Length; ++i) ret.ary[i] = (uint)((MOD + y % MOD) * ret.ary[i] % MOD);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator *(long x, LIB_FPS y) => y * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator /(LIB_FPS x, LIB_FPS y) => x * y.Inverse();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator /(LIB_FPS x, long y) => x * LIB_Mod998244353.Inverse(y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_FPS operator /(long x, LIB_FPS y) => y.Inverse() * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long BostanMori(long N, LIB_FPS nume, LIB_FPS deno)
        {
            var p = nume.ary.Select(e => (long)e).ToArray();
            var q = deno.ary.Select(e => (long)e).ToArray();
            while (N > 0)
            {
                var flipQ = q.ToArray();
                for (var i = 1; i < flipQ.Length; i += 2) flipQ[i] = flipQ[i] == 0 ? 0 : MOD - flipQ[i];
                var tmp = LIB_NTT.Multiply(p, flipQ);
                p = new long[(tmp.Length + (~N & 1)) / 2];
                for (var i = 0; i < p.Length; ++i) p[i] = tmp[i * 2 + (N & 1)];
                tmp = LIB_NTT.Multiply(q, flipQ);
                q = new long[(tmp.Length + 1) / 2];
                for (var i = 0; i < q.Length; ++i) q[i] = tmp[i * 2];
                N /= 2;
            }
            return p[0] * LIB_Mod998244353.Inverse(q[0]) % MOD;
        }
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
            var arySpan = ary.AsSpan();
            if (M == 0)
            {
                arySpan[1..].Clear();
                arySpan[0] = 1;
                return;
            }
            if (M == 1) return;
            if (ary[0] == 1)
            {
                Pow_1_inplace(M);
                return;
            }
            var l = 0;
            while (l < arySpan.Length && arySpan[l] == 0) ++l;
            if (l == arySpan.Length || l > (arySpan.Length - 1) / M)
            {
                arySpan.Clear();
                return;
            }
            var powc = LIB_Mod998244353.Pow(arySpan[l], M);
            var invc = LIB_Mod998244353.Inverse(arySpan[l]);
            var g = new LIB_FPS(K - l);
            var garySpan = g.ary.AsSpan();
            for (var i = l; i < arySpan.Length; ++i) garySpan[i - l] = (uint)(arySpan[i] * invc % MOD);

            var dat = new List<(int idx, long val)>();
            for (var i = 1; i < garySpan.Length; ++i) if (garySpan[i] != 0) dat.Add((i, garySpan[i]));

            var ten = l * (int)M;
            M %= MOD;
            if (dat.Count > 100)
            {
                g.Log_inplace_dense();
                for (var i = 0; i < garySpan.Length; ++i) garySpan[i] = (uint)(M * garySpan[i] % MOD);
                g.Exp_inplace_dense();
            }
            else
            {
                var invBuf = System.Buffers.ArrayPool<long>.Shared.Rent(garySpan.Length);
                var inv = invBuf.AsSpan();
                inv[1] = 1;
                for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[(int)(MOD % i)] * (MOD / i) % MOD;

                for (var n = 0; n < garySpan.Length - 1; ++n)
                {
                    garySpan[n + 1] = 0;
                    foreach (var item in dat)
                    {
                        if (item.idx > n + 1) break;
                        var t = item.val * garySpan[n - item.idx + 1] % MOD;
                        t = t * ((M * item.idx % MOD) - n + item.idx - 1) % MOD;
                        if (t < 0) t += MOD;
                        garySpan[n + 1] += (uint)t;
                        if (garySpan[n + 1] >= MOD) garySpan[n + 1] -= MOD;
                    }
                    garySpan[n + 1] = (uint)(garySpan[n + 1] * inv[n + 1] % MOD);
                }
                System.Buffers.ArrayPool<long>.Shared.Return(invBuf);
            }
            for (var i = arySpan.Length - 1; i >= ten; --i) arySpan[i] = (uint)(powc * garySpan[i - ten] % MOD);
            arySpan.Slice(0, ten).Clear();
        }
        /// <summary>
        /// べき乗（a0 == 1）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Pow_1_inplace(long M)
        {
            var arySpan = ary.AsSpan();

            var dat = new List<(int idx, long val)>();
            for (var i = 1; i < arySpan.Length; ++i) if (arySpan[i] != 0) dat.Add((i, arySpan[i]));

            if (dat.Count > 100)
            {
                Log_inplace_dense();
                for (var i = 0; i < arySpan.Length; ++i) arySpan[i] = (uint)(M * arySpan[i] % MOD);
                Exp_inplace_dense();
            }
            else
            {
                var invBuf = System.Buffers.ArrayPool<long>.Shared.Rent(arySpan.Length);
                var inv = invBuf.AsSpan();
                inv[1] = 1;
                for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[(int)(MOD % i)] * (MOD / i) % MOD;
                arySpan.Clear();
                arySpan[0] = 1;
                for (var n = 1; n < arySpan.Length; ++n)
                {
                    foreach (var item in dat)
                    {
                        if (item.idx > n) break;
                        var t = item.val * arySpan[n - item.idx] % MOD;
                        arySpan[n] = (uint)((arySpan[n] + t * ((M * item.idx % MOD) + MOD - n + item.idx)) % MOD);
                    }
                    arySpan[n] = (uint)(arySpan[n] * inv[n] % MOD);
                }
                System.Buffers.ArrayPool<long>.Shared.Return(invBuf);
            }
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
            var dat = new List<(int idx, long val)>();
            var arySpan = ary.AsSpan();
            for (var i = 1; i < arySpan.Length; ++i) if (arySpan[i] != 0) dat.Add((i - 1, (long)i * arySpan[i] % MOD));
            if (dat.Count > 320)
            {
                Exp_inplace_dense();
                return;
            }

            // sparse
            var inv = System.Buffers.ArrayPool<long>.Shared.Rent(arySpan.Length + 1);
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            arySpan[0] = 1;
            for (var n = 1; n < arySpan.Length; ++n)
            {
                var rhs = 0L;
                foreach (var item in dat)
                {
                    if (item.idx > n - 1) break;
                    rhs += item.val * arySpan[n - 1 - item.idx] % MOD;
                }
                arySpan[n] = (uint)((rhs % MOD) * inv[n] % MOD);
            }
            System.Buffers.ArrayPool<long>.Shared.Return(inv);
        }
        /// <summary>
        /// 指数 (a0 == 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exp_inplace_dense()
        {
            var maxlen = 2;
            while ((maxlen << 1) <= K) maxlen <<= 1;
            var bufferLength = maxlen * 2;
            var buf1 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen);
            var buf2 = System.Buffers.ArrayPool<uint>.Shared.Rent(bufferLength * 6);
            var buf3 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen * 3);
            var inv = System.Buffers.ArrayPool<long>.Shared.Rent(maxlen * 2);
            var buf = buf2.AsSpan();
            var halfBuf = buf3.AsSpan();
            var g = buf1.AsSpan();
            var arySpan = ary.AsSpan();
            g.Clear();
            g[0] = 1;
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[MOD % i] * (MOD / i) % MOD;
            var nttg = buf.Slice(bufferLength * 3, 2);
            nttg[0] = 1;
            nttg[1] = 1;
            arySpan[0] = 1;
            var h_drv = Differential();

            var len = 2;
            while (len <= K)
            {
                var nextlen = len * 2;
                var nttf = buf.Slice(0, nextlen);
                nttf.Clear();
                var ntttmp = buf.Slice(bufferLength, nextlen);
                var ntttmphalf1 = halfBuf.Slice(0, len);
                var ntttmphalf2 = halfBuf.Slice(maxlen, len);
                var ntttmphalf3 = halfBuf.Slice(maxlen * 2, len);
                arySpan.Slice(0, len).CopyTo(nttf);
                LIB_NTT.ntt4(ref nttf, ref ntttmp);

                {
                    ref var ntth = ref ntttmphalf1;
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)nttf[i * 2] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, ref ntttmphalf2, true);
                    for (var i = 0; i < len / 2; ++i) ntth[i] = ntth[i + len / 2];
                    for (var i = len / 2; i < len; ++i) ntth[i] = 0;
                    LIB_NTT.ntt4(ref ntth, ref ntttmphalf2);
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, ref ntttmphalf2, true);
                    for (var i = len / 2; i < len; ++i) g[i] = (ntth[i - len / 2] == 0 ? 0 : MOD - ntth[i - len / 2]);
                }

                var t = buf.Slice(bufferLength * 2, nextlen);
                t.Clear();
                {
                    ref var ntth = ref ntttmphalf1;
                    for (var i = 0; i < len - 1; ++i) ntth[i] = h_drv.ary[i];
                    LIB_NTT.ntt4(ref ntth, ref ntttmphalf2);
                    for (var i = 0; i < len; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttf[i * 2] % MOD);
                    LIB_NTT.ntt4(ref ntth, ref ntttmphalf2, true);
                    for (var i = 1; i < len; ++i) t[i] = (uint)(((ulong)i * ary[i] + MOD - ntth[i - 1]) % MOD);
                    t[0] = ntth[len - 1] == 0 ? 0 : MOD - ntth[len - 1];
                }
                if (2 * len <= K)
                {
                    var newt = buf.Slice(bufferLength * 5, nextlen);
                    t.CopyTo(newt);
                    LIB_NTT.ntt4(ref newt, ref ntttmp);
                    nttg = buf.Slice(bufferLength * 3, nextlen);
                    nttg.Clear();
                    g.Slice(0, len).CopyTo(nttg);
                    LIB_NTT.ntt4(ref nttg, ref ntttmp);
                    for (var i = 0; i < nextlen; ++i) t[i] = (uint)((ulong)newt[i] * nttg[i] % MOD);
                    LIB_NTT.ntt4(ref t, ref ntttmp, true);
                }
                else
                {
                    ref var g1 = ref ntttmphalf1;
                    ref var s1 = ref ntttmphalf2;
                    var t1 = t.Slice(0, len);
                    var t2 = t.Slice(len, len);
                    g1.Clear();
                    s1.Clear();
                    g.Slice(len / 2, len / 2).CopyTo(g1);
                    t.Slice(len / 2, len / 2).CopyTo(s1);
                    t1.Slice(len / 2).Clear();
                    LIB_NTT.ntt4(ref g1, ref ntttmphalf3);
                    LIB_NTT.ntt4(ref s1, ref ntttmphalf3);
                    LIB_NTT.ntt4(ref t1, ref t2);
                    for (var i = 0; i < len; ++i)
                    {
                        s1[i] = (uint)(((ulong)nttg[i] * s1[i] + (ulong)g1[i] * t1[i]) % MOD);
                        t1[i] = (uint)((ulong)nttg[i] * t1[i] % MOD);
                    }
                    LIB_NTT.ntt4(ref t1, ref t2, true);
                    LIB_NTT.ntt4(ref s1, ref ntttmphalf3, true);
                    for (var i = len / 2; i < len; ++i) t1[i] = (t1[i] + s1[i - len / 2]) % MOD;
                    t1.CopyTo(t);
                }

                {
                    var ntth = buf.Slice(bufferLength * 4, nextlen);
                    ntth.Clear();
                    var ten = Min(ary.Length, 2 * len);
                    arySpan.Slice(len, ten - len).CopyTo(ntth);
                    for (var i = 0; i < len; ++i) ntth[i] = (ntth[i] + MOD - (uint)(inv[i + len] * t[i] % MOD)) % MOD;

                    LIB_NTT.ntt4(ref ntth, ref ntttmp);
                    for (var i = 0; i < nextlen; ++i) ntth[i] = (uint)((ulong)ntth[i] * nttf[i] % MOD);
                    LIB_NTT.ntt4(ref ntth, ref ntttmp, true);

                    ntth.Slice(0, Min(ary.Length - len, len)).CopyTo(arySpan.Slice(len));
                }

                len = nextlen;
            }
            System.Buffers.ArrayPool<uint>.Shared.Return(buf1);
            System.Buffers.ArrayPool<uint>.Shared.Return(buf2);
            System.Buffers.ArrayPool<long>.Shared.Return(inv);
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
            var dat = new List<(int idx, uint val)>();
            var arySpan = ary.AsSpan();
            for (var i = 1; i < ary.Length; ++i) if (arySpan[i] != 0) dat.Add((i, arySpan[i]));
            if (dat.Count > 200)
            {
                Log_inplace_dense();
                return;
            }

            // sparse
            var gBuf = System.Buffers.ArrayPool<long>.Shared.Rent(arySpan.Length - 1);
            var invBuf = System.Buffers.ArrayPool<long>.Shared.Rent(arySpan.Length);
            var g = gBuf.AsSpan();
            var inv = invBuf.AsSpan();
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[(int)(MOD % i)] * (MOD / i) % MOD;
            arySpan[0] = 0;
            for (var n = 0; n < arySpan.Length - 1; ++n)
            {
                var rhs = (long)(n + 1) * arySpan[n + 1] % MOD;
                foreach (var item in dat)
                {
                    if (item.idx > n) break;
                    rhs = rhs - item.val * g[n - item.idx] % MOD;
                    if (rhs < 0) rhs += MOD;
                }
                g[n] = rhs;
                arySpan[n + 1] = (uint)(rhs * inv[n + 1] % MOD);
            }
            System.Buffers.ArrayPool<long>.Shared.Return(invBuf);
            System.Buffers.ArrayPool<long>.Shared.Return(gBuf);
        }
        /// <summary>
        /// 対数 (a0 == 1)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log_inplace_dense()
        {
            var f2 = Differential();
            Inverse_inplace();
            var len = 1;
            while (len < ary.Length + f2.ary.Length - 1) len <<= 1;
            var buf1 = System.Buffers.ArrayPool<uint>.Shared.Rent(len * 3);
            var buf = buf1.AsSpan();
            buf.Clear();
            var arySpan = ary.AsSpan();
            var nttdiff = buf.Slice(0, len);
            var nttinv = buf.Slice(len, len);
            var ntttmp = buf.Slice(len * 2, len);
            f2.ary.CopyTo(nttdiff);
            ary.CopyTo(nttinv);
            LIB_NTT.ntt4(ref nttdiff, ref ntttmp);
            LIB_NTT.ntt4(ref nttinv, ref ntttmp);
            for (var i = 0; i < nttinv.Length; ++i) nttinv[i] = (uint)((long)nttinv[i] * nttdiff[i] % MOD);
            LIB_NTT.ntt4(ref nttinv, ref ntttmp, true);
            nttinv.Slice(0, ary.Length).CopyTo(arySpan);
            Integral_inplace();
            System.Buffers.ArrayPool<uint>.Shared.Return(buf1);
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
            var invBuf = System.Buffers.ArrayPool<long>.Shared.Rent(ary.Length);
            var inv = invBuf.AsSpan();
            inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[(int)(MOD % i)] * (MOD / i) % MOD;
            for (var i = ary.Length - 1; i > 0; --i) ary[i] = (uint)(inv[i] * ary[i - 1] % MOD);
            ary[0] = 0;
            System.Buffers.ArrayPool<long>.Shared.Return(invBuf);
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
            var arySpan = ary.AsSpan();
            var dat = new List<(int idx, long val)>(arySpan.Length);
            for (var i = 1; i < arySpan.Length; ++i) if (arySpan[i] != 0) dat.Add((i, arySpan[i]));
            if (dat.Count > 160)
            {
                Inverse_inplace_dense();
                return;
            }

            // sparse
            arySpan[0] = (uint)LIB_Mod998244353.Inverse(arySpan[0]);
            for (var n = 1; n < arySpan.Length; ++n)
            {
                var rhs = 0L;
                foreach (var item in dat)
                {
                    if (item.idx > n) break;
                    rhs -= item.val * arySpan[n - item.idx] % MOD;
                }
                rhs = (rhs % MOD) * arySpan[0] % MOD;
                if (rhs < 0) rhs += MOD;
                arySpan[n] = (uint)rhs;
            }
        }
        /// <summary>
        /// 逆元 (a0 != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse_inplace_dense()
        {
            var maxlen = 1;
            while (maxlen <= K) maxlen <<= 1;
            var buf1 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen);
            var buf2 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen * 3);
            var buf = buf2.AsSpan();
            var arySpan = ary.AsSpan();
            var g = buf1.AsSpan();
            g.Clear();
            g[0] = (uint)LIB_Mod998244353.Inverse(arySpan[0]);
            var len = 1;
            while (len <= K)
            {
                var nextlen = len << 1;
                var nttf = buf.Slice(0, nextlen);
                var nttg = buf.Slice(maxlen, nextlen);
                var ntth = buf.Slice(maxlen * 2, nextlen);
                nttf.Clear();
                nttg.Clear();
                var ten = Min(nextlen, ary.Length);
                arySpan.Slice(0, ten).CopyTo(nttf);
                g.Slice(0, ten).CopyTo(nttg);
                LIB_NTT.ntt4(ref nttf, ref ntth);
                LIB_NTT.ntt4(ref nttg, ref ntth);
                for (var i = 0; i < nextlen; ++i) nttf[i] = (uint)((ulong)nttf[i] * nttg[i] % MOD);
                LIB_NTT.ntt4(ref nttf, ref ntth, true);
                for (var i = 0; i < len; ++i) nttf[i] = 0;
                LIB_NTT.ntt4(ref nttf, ref ntth);
                for (var i = 0; i < nextlen; ++i) nttf[i] = (uint)((ulong)nttf[i] * nttg[i] % MOD);
                LIB_NTT.ntt4(ref nttf, ref ntth, true);
                for (var i = len; i < nextlen; ++i) g[i] = (nttf[i] == 0 ? 0 : MOD - nttf[i]);
                len = nextlen;
            }
            g.Slice(0, ary.Length).CopyTo(arySpan);
            System.Buffers.ArrayPool<uint>.Shared.Return(buf1);
            System.Buffers.ArrayPool<uint>.Shared.Return(buf2);
        }
        /// <summary>
        /// 平方根 (a0 != 0 または [x^(2n)] != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Sqrt()
        {
            var ret = Clone();
            if (!ret.Sqrt_inplace()) return null;
            return ret;
        }
        /// <summary>
        /// 平方根 (a0 != 0 または [x^(2n)] != 0)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Sqrt_inplace()
        {
            var arySpan = ary.AsSpan();
            var d = arySpan.Length;
            var nonZeroCnt = 0;
            for (var i = arySpan.Length - 1; i >= 0; --i)
            {
                if (arySpan[i] != 0)
                {
                    d = i;
                    ++nonZeroCnt;
                }
            }
            if (d == arySpan.Length) return true;
            if ((d & 1) != 0) return false;
            var y = arySpan[d];
            var x = y;
            {
                var k = (MOD - 1) / 2;
                if (LIB_Mod998244353.Pow(x, k) != 1) return false;
                var rnd = new Random(0);
                var b = 0L;
                var D = 0L;
                while (true)
                {
                    b = rnd.Next(2, (int)MOD);
                    D = (b * b - x + MOD) % MOD;
                    if (D == 0) break;
                    if (LIB_Mod998244353.Pow(D, k) != 1) break;
                }
                if (D == 0)
                {
                    x = (uint)b;
                }
                else
                {
                    ++k;
                    var f0 = b;
                    var f1 = 1L;
                    var g0 = 1L;
                    var g1 = 0L;
                    while (k > 0)
                    {
                        if ((k & 1) != 0)
                        {
                            var t = (f1 * g0 + f0 * g1) % MOD;
                            g0 = (f0 * g0 + (D * f1 % MOD) * g1) % MOD;
                            g1 = t;
                        }
                        {
                            var t = (2 * f0 * f1) % MOD;
                            f0 = (f0 * f0 + (D * f1 % MOD) * f1) % MOD;
                            f1 = t;
                        }
                        k >>= 1;
                    }
                    if (g0 * g0 % MOD != x) return false;
                    x = (uint)g0;
                }
            }
            var c = LIB_Mod998244353.Inverse(y);
            var gLength = arySpan.Length - d;
            for (var i = d; i < arySpan.Length; ++i)
            {
                arySpan[i - d] = (uint)(arySpan[i] * c % MOD);
            }

            var maxlen = 1;
            while (maxlen < gLength) maxlen <<= 1;
            var buf1 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen * 2);
            var rSpan = buf1.AsSpan().Slice(0, maxlen);
            rSpan.Clear();
            rSpan[0] = 1;

            if (nonZeroCnt <= 200)
            {
                Pow_1_inplace(499122177);
                arySpan.Slice(0, gLength).CopyTo(rSpan);
            }
            else
            {
                for (var i = 0; i < gLength; ++i) arySpan[i] = (uint)(arySpan[i] * 499122177L % MOD);
                var buf2 = System.Buffers.ArrayPool<uint>.Shared.Rent(maxlen * 5);
                var buf = buf2.AsSpan();
                var invRSpan = buf1.AsSpan().Slice(maxlen, maxlen);
                invRSpan.Clear();
                invRSpan[0] = 1;
                var len = 1;
                Span<uint> halfInv1 = null;
                Span<uint> halfInv2 = null;
                Span<uint> convTmpSpan = null;
                while (len < gLength)
                {
                    var nextlen = len << 1;
                    if (len >= 2)
                    {
                        var halflen = len >> 1;
                        rSpan.Slice(0, len).CopyTo(halfInv1);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan);
                        for (var i = 0; i < len; ++i) halfInv1[i] = (uint)((ulong)halfInv1[i] * halfInv2[i] % MOD);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan, true);
                        for (var i = 0; i < halflen; ++i) halfInv1[i] = 0;
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan);
                        for (var i = 0; i < len; ++i) halfInv1[i] = (uint)((ulong)halfInv1[i] * halfInv2[i] % MOD);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan, true);
                        for (var i = halflen; i < len; ++i) invRSpan[i] = halfInv1[i] == 0 ? 0 : MOD - halfInv1[i];
                    }
                    convTmpSpan = buf.Slice(0, nextlen);
                    halfInv1 = buf.Slice(maxlen, nextlen);
                    halfInv2 = buf.Slice(maxlen * 2, nextlen);
                    halfInv2.Clear();
                    invRSpan.Slice(0, len).CopyTo(halfInv2);
                    LIB_NTT.ntt4(ref halfInv2, ref convTmpSpan);
                    {
                        rSpan.Slice(0, nextlen).CopyTo(halfInv1);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan);
                        for (var i = 0; i < nextlen; ++i) halfInv1[i] = (uint)((ulong)halfInv1[i] * halfInv2[i] % MOD);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan, true);
                        for (var i = 0; i < len; ++i) halfInv1[i] = 0;
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan);
                        for (var i = 0; i < nextlen; ++i) halfInv1[i] = (uint)((ulong)halfInv1[i] * halfInv2[i] % MOD);
                        LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan, true);
                        for (var i = len; i < nextlen; ++i) invRSpan[i] = halfInv1[i] == 0 ? 0 : MOD - halfInv1[i];
                    }
                    invRSpan.Slice(0, nextlen).CopyTo(halfInv1);
                    LIB_NTT.ntt4(ref halfInv1, ref convTmpSpan);

                    var frontHalf = buf.Slice(maxlen * 3, nextlen);
                    var lastHalf = buf.Slice(maxlen * 4, nextlen);
                    frontHalf.Clear();
                    lastHalf.Clear();
                    arySpan.Slice(0, len).CopyTo(frontHalf);
                    arySpan.Slice(len, Min(len, arySpan.Length - len)).CopyTo(lastHalf.Slice(len));
                    LIB_NTT.ntt4(ref frontHalf, ref convTmpSpan);
                    LIB_NTT.ntt4(ref lastHalf, ref convTmpSpan);

                    for (var i = 0; i < nextlen; ++i)
                    {
                        frontHalf[i] = (uint)(((long)frontHalf[i] * halfInv1[i] + (long)lastHalf[i] * halfInv2[i]) % MOD);
                    }
                    LIB_NTT.ntt4(ref frontHalf, ref convTmpSpan, true);
                    frontHalf.Slice(len, len).CopyTo(rSpan.Slice(len));
                    len = nextlen;
                }
                System.Buffers.ArrayPool<uint>.Shared.Return(buf2);
            }

            arySpan.Clear();
            for (var i = 0; i < gLength; ++i)
            {
                arySpan[i + d / 2] = (uint)((long)rSpan[i] * x % MOD);
            }
            System.Buffers.ArrayPool<uint>.Shared.Return(buf1);
            return true;
        }
        /// <summary>
        /// 平行移動
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS TaylorShift(long c)
        {
            var ret = Clone();
            ret.TaylorShift_inplace(c);
            return ret;
        }
        /// <summary>
        /// 平行移動
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TaylorShift_inplace(long c)
        {
            var arySpan = ary.AsSpan();

            var len = 1;
            while (len < arySpan.Length) len <<= 1;
            len <<= 1;
            var buf1 = System.Buffers.ArrayPool<uint>.Shared.Rent(len * 3);
            var buf = buf1.AsSpan();
            var ntta = buf.Slice(0, len);
            var nttb = buf.Slice(len, len);
            var ntth = buf.Slice(len * 3, len);
            ntta.Clear();
            nttb.Clear();

            var invBuf = System.Buffers.ArrayPool<long>.Shared.Rent(arySpan.Length);
            var inv = invBuf.AsSpan();
            inv[0] = inv[1] = 1;
            for (var i = 2; i < inv.Length; ++i) inv[i] = MOD - inv[(int)(MOD % i)] * (MOD / i) % MOD;
            var fact = 1L;
            ntta[arySpan.Length - 1] = arySpan[0];
            for (var i = 1; i < arySpan.Length; ++i)
            {
                fact = fact * i % MOD;
                ntta[arySpan.Length - i - 1] = (uint)(fact * arySpan[i] % MOD);
            }

            var power = 1L;
            fact = 1L;
            nttb[0] = 1;
            for (var i = 1; i < arySpan.Length; ++i)
            {
                power = power * c % MOD;
                fact = fact * inv[i] % MOD;
                nttb[i] = (uint)(power * fact % MOD);
            }

            LIB_NTT.ntt4(ref ntta, ref ntth);
            LIB_NTT.ntt4(ref nttb, ref ntth);
            for (var i = 0; i < ntta.Length; ++i) ntta[i] = (uint)((ulong)ntta[i] * nttb[i] % MOD);
            LIB_NTT.ntt4(ref ntta, ref ntth, true);

            fact = 1L;
            for (var i = 0; i < arySpan.Length; ++i)
            {
                fact = fact * inv[i] % MOD;
                arySpan[i] = (uint)(ntta[arySpan.Length - 1 - i] * fact % MOD);
            }

            System.Buffers.ArrayPool<uint>.Shared.Return(buf1);
        }
        /// <summary>
        /// 合成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS Composition(LIB_FPS g)
        {
            var ret = Clone();
            ret.Composition_inplace(g);
            return ret;
        }
        // 合成のためのヘルパ関数（後で整理する）
        static uint[] root = new uint[] { 1, 998244352, 911660635, 372528824, 929031873, 452798380, 922799308, 781712469, 476477967, 166035806, 258648936, 584193783, 63912897, 350007156, 666702199, 968855178, 629671588, 24514907, 996173970, 363395222, 565042129, 733596141, 267099868, 15311432, 0 };
        static uint[] iroot = new uint[] { 1, 998244352, 86583718, 509520358, 337190230, 87557064, 609441965, 135236158, 304459705, 685443576, 381598368, 335559352, 129292727, 358024708, 814576206, 708402881, 283043518, 3707709, 121392023, 704923114, 950391366, 428961804, 382752275, 469870224, 0 };
        static uint[] rate2 = new uint[] { 911660635, 509520358, 369330050, 332049552, 983190778, 123842337, 238493703, 975955924, 603855026, 856644456, 131300601, 842657263, 730768835, 942482514, 806263778, 151565301, 510815449, 503497456, 743006876, 741047443, 56250497, 867605899, 0 };
        static uint[] irate2 = new uint[] { 86583718, 372528824, 373294451, 645684063, 112220581, 692852209, 155456985, 797128860, 90816748, 860285882, 927414960, 354738543, 109331171, 293255632, 535113200, 308540755, 121186627, 608385704, 438932459, 359477183, 824071951, 103369235, 0 };
        static uint[] rate3 = new uint[] { 372528824, 337190230, 454590761, 816400692, 578227951, 180142363, 83780245, 6597683, 70046822, 623238099, 183021267, 402682409, 631680428, 344509872, 689220186, 365017329, 774342554, 729444058, 102986190, 128751033, 395565204, 0 };
        static uint[] irate3 = new uint[] { 509520358, 929031873, 170256584, 839780419, 282974284, 395914482, 444904435, 72135471, 638914820, 66769500, 771127074, 985925487, 262319669, 262341272, 625870173, 768022760, 859816005, 914661783, 430819711, 272774365, 530924681, 0 };
        static int ceil_pow2(int n)
        {
            int i = 0;
            while ((1U << i) < (uint)(n)) i++;
            return i;
        }
        static void ntt_trans(Span<uint> v)
        {
            var n = v.Length;
            var h = ceil_pow2(n);
            const ulong mod2 = (ulong)MOD * MOD;

            var len = 0;
            while (len < h)
            {
                if (h - len == 1)
                {
                    var rot = 1U;
                    for (var s = 0; s < (1 << len); s++)
                    {
                        var offset = s << (h - len);
                        var l = v[offset];
                        var r = (uint)(v[offset + 1] * (ulong)rot % MOD);
                        v[offset] = (l + r) % MOD;
                        v[offset + 1] = (l + MOD - r) % MOD;
                        rot = (uint)(rot * (ulong)rate2[LIB_BitUtil.LSB(~(uint)(s)) - 1] % MOD);
                    }
                    len++;
                }
                else
                {
                    var p = 1 << (h - len - 2);
                    var rot = 1U;
                    var imag = root[2];
                    for (var s = 0; s < (1 << len); s++)
                    {
                        var rot2 = (uint)(rot * (ulong)rot % MOD);
                        var rot3 = (uint)(rot2 * (ulong)rot % MOD);
                        var offset = s << (h - len);
                        for (var i = 0; i < p; i++)
                        {
                            var a0 = (ulong)v[i + offset];
                            var a1 = (ulong)v[i + offset + p] * rot;
                            var a2 = (ulong)v[i + offset + p * 2] * rot2;
                            var a3 = (ulong)v[i + offset + p * 3] * rot3;
                            var tmp = ((a1 + mod2 - a3) % MOD) * imag;
                            var na2 = mod2 - a2;
                            v[i + offset] = (uint)((a0 + a2 + a1 + a3) % MOD);
                            v[i + offset + p] = (uint)((a0 + a2 + (mod2 * 2 - (a1 + a3))) % MOD);
                            v[i + offset + p * 2] = (uint)((a0 + na2 + tmp) % MOD);
                            v[i + offset + p * 3] = (uint)((a0 + na2 + (mod2 - tmp)) % MOD);
                        }
                        rot = (uint)(rot * (ulong)rate3[LIB_BitUtil.LSB(~(uint)(s)) - 1] % MOD);
                    }
                    len += 2;
                }
            }
        }
        static void ntt_trans_rev(Span<uint> v)
        {
            var n = v.Length;
            var h = ceil_pow2(n);

            var len = h;
            while (len > 0)
            {
                if (len == 1)
                {
                    var p = 1 << (h - len);
                    var irot = 1U;
                    for (var s = 0; s < (1 << (len - 1)); s++)
                    {
                        var offset = s << (h - len + 1);
                        for (var i = 0; i < p; i++)
                        {
                            var l = v[i + offset];
                            var r = v[i + offset + p];
                            v[i + offset] = (l + r) % MOD;
                            v[i + offset + p] = (l + MOD - r) * irot % MOD;
                        }
                        irot = (uint)(irot * (ulong)irate2[LIB_BitUtil.LSB(~(uint)(s)) - 1] % MOD);
                    }
                    len--;
                }
                else
                {
                    var p = 1 << (h - len);
                    var irot = 1U;
                    var iimag = iroot[2];
                    for (var s = 0; s < (1 << (len - 2)); s++)
                    {
                        var irot2 = (uint)(irot * (ulong)irot % MOD);
                        var irot3 = (uint)(irot2 * (ulong)irot % MOD);
                        var offset = s << (h - len + 2);
                        for (var i = 0; i < p; i++)
                        {
                            var a0 = (ulong)v[i + offset];
                            var a1 = (ulong)v[i + offset + p];
                            var a2 = (ulong)v[i + offset + p * 2];
                            var a3 = (ulong)v[i + offset + p * 3];
                            var tmp = (a2 + MOD - a3) * iimag % MOD;
                            v[i + offset] = (uint)((a0 + a2 + a1 + a3) % MOD);
                            v[i + offset + p] = (uint)((a0 + MOD - a1 + tmp) * irot % MOD);
                            v[i + offset + p * 2] = (uint)((a0 + a1 + MOD * 2 - a2 - a3) * irot2 % MOD);
                            v[i + offset + p * 3] = (uint)((a0 + MOD * 2 - a1 - tmp) * irot3 % MOD);
                        }
                        irot = (uint)(irot * (ulong)irate3[LIB_BitUtil.LSB(~(uint)(s)) - 1] % MOD);
                    }
                    len -= 2;
                }
            }
            var inv = LIB_Mod998244353.Inverse(n);
            for (var i = 0; i < n; ++i)
            {
                v[i] = (uint)((v[i] * (ulong)inv) % MOD);
            }
        }
        /// <summary>
        /// 合成
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Composition_inplace(LIB_FPS g)
        {
            Func<uint[], int, int, int, uint[]> rec = null;
            rec = (Q, n, h, k) =>
            {
                if (n == 0)
                {
                    var t = new LIB_FPS(k);
                    for (var i = 0; i < k; ++i)
                    {
                        t[k - i] = Q[i];
                    }
                    t[0] = 1;
                    t.Inverse_inplace();
                    var u = LIB_NTT.Multiply(ary.Select(e => (long)e).ToArray(), t.ary.Reverse().Select(e => (long)e).ToArray());
                    var P2 = new uint[h * k];
                    for (var i = 0; i < ary.Length; ++i)
                    {
                        P2[k - i - 1] = (uint)u[i + k];
                    }
                    return P2;
                }
                var buf = System.Buffers.ArrayPool<uint>.Shared.Rent(h * k * 8);
                buf.AsSpan().Clear();
                var nQ = buf.AsSpan().Slice(0, h * k * 4);
                var nR = buf.AsSpan().Slice(h * k * 4, h * k * 2);
                for (var i = 0; i < k; ++i)
                {
                    for (var j = 0; j <= n; ++j)
                    {
                        nQ[i * h * 2 + j] = Q[i * h + j];
                    }
                }
                nQ[h * k * 2] += 1;
                ntt_trans(nQ);
                for (var i = 0; i < h * k * 4; i += 2)
                {
                    (nQ[i], nQ[i + 1]) = (nQ[i + 1], nQ[i]);
                }
                for (var i = 0; i < h * k * 2; ++i)
                {
                    nR[i] = (uint)(nQ[i * 2] * (ulong)nQ[i * 2 + 1] % MOD);
                }
                ntt_trans_rev(nR);
                nR[0] -= 1;
                for (var i = 0; i < h * k; ++i)
                {
                    Q[i] = 0;
                }
                for (var i = 0; i < k * 2; ++i)
                {
                    for (var j = 0; j <= n / 2; ++j)
                    {
                        Q[i * h / 2 + j] = nR[i * h + j];
                    }
                }
                var P = rec(Q, n / 2, h / 2, k * 2);
                nR.Clear();
                var nP = buf.AsSpan().Slice(h * k * 4, h * k * 4);
                for (var i = 0; i < k * 2; ++i)
                {
                    for (var j = 0; j <= n / 2; ++j)
                    {
                        nP[i * h * 2 + j * 2 + n % 2] = P[i * h / 2 + j];
                    }
                }
                ntt_trans(nP);
                for (var i = 1; i < h * k * 4; i <<= 1)
                {
                    for (var j = 0; j < i / 2; ++j)
                    {
                        (nQ[i + j], nQ[i * 2 - j - 1]) = (nQ[i * 2 - j - 1], nQ[i + j]);
                    }
                }
                for (var i = 0; i < h * k * 4; ++i)
                {
                    nP[i] = (uint)(nP[i] * (ulong)nQ[i] % MOD);
                }
                ntt_trans_rev(nP);
                for (var i = 0; i < h * k; ++i)
                {
                    P[i] = 0;
                }
                for (var i = 0; i < k; ++i)
                {
                    for (var j = 0; j <= n; ++j)
                    {
                        P[i * h + j] = nP[i * h * 2 + j];
                    }
                }

                System.Buffers.ArrayPool<uint>.Shared.Return(buf);
                return P;
            };
            var deg = Max(ary.Length, g.ary.Length);
            Array.Resize(ref ary, deg);
            Array.Resize(ref g.ary, deg);
            var n = deg - 1;
            var h = 1;
            var k = 1;
            while (h < n + 1) h <<= 1;
            var Q = new uint[h * k];
            for (var i = 0; i <= n; ++i)
            {
                Q[i] = (uint)(MOD - g[i]) % MOD;
            }
            var P = rec(Q, n, h, k);
            ary = P.Take(deg).Reverse().ToArray();
        }
        void power_projection()
        {
            var n = ary.Length - 1;
            var k = 1;
            var h = 1;
            var g = new uint[n + 1];
            g[0] = 1;
            var m = n;
            while (h < n + 1) h <<= 1;
            var P = new uint[h << 2];
            var Q = new uint[h << 2];
            for (int i = 0; i <= n; i++) P[i * k] = g[i];
            for (int i = 0; i <= n; i++) Q[i * k] = (MOD - ary[i]) % MOD;
            Q[0]++;
            if (Q[0] == MOD) Q[0] = 0;
            var inv2 = LIB_Mod998244353.Inverse(2);
            var pr = 3U;
            var buf = new uint[h];
            var buf2 = new uint[h >> 1];
            var nP = new uint[h << 2];
            var nQ = new uint[h << 2];
            var pAry = new uint[h * 3];
            var qAry = new uint[h * 2];
            var btr = new int[h];
            var bufferBlock = h;
            while (n > 0)
            {
                var w = (uint)LIB_Mod998244353.Pow(pr, (MOD - 1) / (2 * k));
                var iw = LIB_Mod998244353.Inverse(w);
                Action ntt_doubling = () =>
                {
                    Array.Copy(buf, buf2, k);
                    var tmp = buf2.AsSpan().Slice(0, k);
                    ntt_trans_rev(tmp);
                    var c = 1U;
                    for (var i = 0; i < k; ++i)
                    {
                        tmp[i] = (uint)((tmp[i] * (ulong)c) % MOD);
                        c = (uint)((ulong)c * w % MOD);
                    }
                    ntt_trans(tmp);
                    for (var i = 0; i < tmp.Length; ++i)
                    {
                        buf[tmp.Length + i] = tmp[i];
                    }
                };
                var nPi = 0;
                var nQi = 0;
                for (var i = 0; i <= n; ++i)
                {
                    for (var j = 0; j < k; ++j)
                    {
                        buf[j] = P[i * k + j];
                    }
                    ntt_doubling();
                    for (var j = 0; j < k * 2; ++j)
                    {
                        nP[nPi++] = buf[j];
                    }
                    for (var j = 0; j < k; ++j)
                    {
                        buf[j] = Q[i * k + j];
                    }
                    if (i == 0)
                    {
                        for (var j = 0; j < k; ++j)
                        {
                            if (buf[j] == 0) buf[j] = MOD - 1;
                            else --buf[j];
                        }
                        ntt_doubling();
                        for (var j = 0; j < k; ++j)
                        {
                            ++buf[j];
                            if (buf[j] == MOD) buf[j] = 0;
                        }
                        for (var j = 0; j < k; ++j)
                        {
                            if (buf[k + j] == 0) buf[k + j] = MOD - 1;
                            else --buf[k + j];
                        }
                    }
                    else
                    {
                        ntt_doubling();
                    }
                    for (var j = 0; j < k * 2; ++j)
                    {
                        nQ[nQi++] = buf[j];
                    }
                }
                nP.AsSpan().Slice(nPi).Clear();
                nQ.AsSpan().Slice(nQi).Clear();
                w = (uint)LIB_Mod998244353.Pow(pr, (MOD - 1) / (h * 2));
                iw = LIB_Mod998244353.Inverse(w);
                if (n % 2 == 1)
                {
                    var i = 0;
                    var lg = LIB_BitUtil.LSB(h) - 1;
                    while (i < h)
                    {
                        btr[i] = (btr[i >> 1] >> 1) + ((i & 1) << (lg - 1));
                        ++i;
                    }
                }
                for (var j = 0; j < k * 2; ++j)
                {
                    var p = pAry.AsSpan().Slice(0, h * 2);
                    var q = qAry.AsSpan().Slice(0, h * 2);
                    p.Clear();
                    q.Clear();
                    for (var i = 0; i < h; ++i)
                    {
                        p[i] = nP[i * k * 2 + j];
                        q[i] = nQ[i * k * 2 + j];
                    }
                    ntt_trans(p);
                    ntt_trans(q);
                    for (var i = 0; i < h * 2; i += 2)
                    {
                        (q[i], q[i + 1]) = (q[i + 1], q[i]);
                    }
                    for (var i = 0; i < h * 2; ++i)
                    {
                        p[i] = (uint)((ulong)p[i] * q[i] % MOD);
                    }
                    for (var i = 0; i < h; ++i)
                    {
                        q[i] = (uint)((ulong)q[i * 2] * q[i * 2 + 1] % MOD);
                    }
                    if ((n & 1) != 0)
                    {
                        p.Slice(0, h).CopyTo(buf);
                        var c = inv2;
                        var newp = pAry.AsSpan().Slice(bufferBlock * 2, h);
                        for (var btrI = 0; btrI < h; ++btrI)
                        {
                            var i = btr[btrI];
                            newp[i] = (uint)(((long)p[i * 2] + MOD - p[i * 2 + 1]) * c % MOD);
                            c = (uint)(c * iw % MOD);
                        }
                        p = newp;
                    }
                    else
                    {
                        for (var i = 0; i < h; ++i)
                        {
                            p[i] = (uint)((p[i * 2] + p[i * 2 + 1]) * inv2 % MOD);
                        }
                    }
                    p = p.Slice(0, h);
                    q = q.Slice(0, h);
                    ntt_trans_rev(p);
                    ntt_trans_rev(q);
                    for (int i = 0; i < h; i++) nP[i * k * 2 + j] = p[i];
                    for (int i = 0; i < h; i++) nQ[i * k * 2 + j] = q[i];
                }
                (P, nP) = (nP, P);
                (Q, nQ) = (nQ, Q);
                n /= 2;
                h /= 2;
                k *= 2;
            }
            Array.Resize(ref P, (n / 2 + 1) * k);
            Array.Resize(ref Q, (n / 2 + 1) * k);
            ntt_trans_rev(P);
            ntt_trans_rev(Q);
            var S = new LIB_FPS(k - 1);
            var T = new LIB_FPS(k);
            for (var i = 0; i < S.ary.Length; ++i)
            {
                S.ary[i] = P[i];
            }
            for (var i = 0; i < Q.Length; ++i)
            {
                T.ary[i] = Q[i];
            }
            if (--T[0] < 0) T[0] = MOD + T[0];
            if (T[0] == 0)
            {
                for (var i = 0; i < ary.Length; ++i)
                {
                    ary[i] = S.ary[S.ary.Length - i - 1];
                }
            }
            else
            {
                for (var i = 0; i < S.ary.Length / 2; ++i)
                {
                    (S.ary[i], S.ary[S.ary.Length - i - 1]) = (S.ary[S.ary.Length - i - 1], S.ary[i]);
                }
                if (++T[k] == MOD) T[k] = 0;
                for (var i = 0; i < T.ary.Length / 2; ++i)
                {
                    (T.ary[i], T.ary[T.ary.Length - i - 1]) = (T.ary[T.ary.Length - i - 1], T.ary[i]);
                }
                T.Inverse_inplace();
                S *= T;
                for (var i = 0; i < ary.Length; ++i)
                {
                    ary[i] = S.ary[i];
                }
            }
        }
        /// <summary>
        /// 逆関数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FPS CompositionalInverse()
        {
            var ret = Clone();
            ret.CompositionalInverse_inplace();
            return ret;
        }
        /// <summary>
        /// 逆関数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CompositionalInverse_inplace()
        {
            var n = ary.Length - 1;
            var inv2 = LIB_Mod998244353.Inverse((MOD - n) % MOD);
            var inv3 = LIB_Mod998244353.Inverse(ary[1]);
            power_projection();
            ary[0] = (uint)(ary[0] * n % MOD);
            for (var k = 1; k <= n; ++k)
            {
                ary[k] = (uint)(ary[k] * LIB_Mod998244353.Inverse(k) % MOD * n % MOD);
            }
            for (var i = 0; i < ary.Length / 2; ++i)
            {
                (ary[i], ary[ary.Length - i - 1]) = (ary[ary.Length - i - 1], ary[i]);
            }
            var inv1 = LIB_Mod998244353.Inverse(ary[0]);
            for (var i = 0; i < ary.Length; ++i) ary[i] = (uint)(ary[i] * inv1 % MOD);
            Log_inplace();
            for (var i = 0; i < ary.Length; ++i) ary[i] = (uint)(ary[i] * inv2 % MOD);
            Exp_inplace();
            for (var i = 0; i < ary.Length; ++i) ary[i] = (uint)(ary[i] * inv3 % MOD);
            for (var i = ary.Length - 1; i > 0; --i) ary[i] = ary[i - 1];
            ary[0] = 0;
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