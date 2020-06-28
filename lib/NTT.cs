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
    class LIB_NTT
    {
        uint mod;
        uint root;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        LIB_NTT(uint mod, uint root) { this.mod = mod; this.root = root; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint add(uint x, uint y) => (x + y < mod) ? (x + y) : (x + y - mod);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint sub(uint x, uint y) => (x >= y) ? (x - y) : (mod - y + x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint mul(uint x, uint y) => (uint)((ulong)x * y % mod);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint pow(uint x, uint n)
        {
            uint res = 1;
            while (n > 0)
            {
                if ((n & 1) != 0) res = mul(res, x);
                x = mul(x, x);
                n >>= 1;
            }
            return res;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint inverse(uint x)
        {
            long b = mod, r = 1, u = 0, t = 0, x2 = x;
            while (b > 0)
            {
                var q = x2 / b;
                t = u;
                u = r - q * u;
                r = t;
                t = b;
                b = x2 - q * b;
                x2 = t;
            }
            return (uint)(r < 0 ? r + mod : r);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ntt(ref uint[] a, bool rev = false)
        {
            var n = (uint)a.Length;
            if (n == 1) return;
            var b = new uint[n];
            var s = pow(root, rev ? (mod - 1 - (mod - 1) / n) : (mod - 1) / n);
            var kp = Enumerable.Repeat((uint)1, (int)(n / 2 + 1)).ToArray();
            uint i, j, k, l, r;
            for (i = 0; i < n / 2; ++i) kp[i + 1] = mul(kp[i], s);
            for (i = 1, l = n / 2; i < n; i <<= 1, l >>= 1)
            {
                for (j = 0, r = 0; j < l; ++j, r += i)
                {
                    for (k = 0, s = kp[i * j]; k < i; ++k)
                    {
                        var p = a[k + r]; var q = a[k + r + n / 2];
                        b[k + 2 * r] = add(p, q);
                        b[k + 2 * r + i] = mul(sub(p, q), s);
                    }
                }
                var t = a; a = b; b = t;
            }
            if (rev)
            {
                s = inverse(n);
                for (i = 0; i < n; ++i) a[i] = mul(a[i], s);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply(uint[] a, uint[] b, uint mod, uint root)
        {
            var n = a.Length + b.Length - 1;
            var t = 1;
            while (t < n) t <<= 1;
            var na = new uint[t];
            var nb = new uint[t];
            for (var i = 0; i < a.Length; ++i) na[i] = a[i];
            for (var i = 0; i < b.Length; ++i) nb[i] = b[i];
            var ntt = new LIB_NTT(mod, root);
            ntt.ntt(ref na);
            ntt.ntt(ref nb);
            for (var i = 0; i < t; ++i) na[i] = ntt.mul(na[i], nb[i]);
            ntt.ntt(ref na, true);
            return na.Take(n).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b) => Multiply(a.Select(e => (uint)e).ToArray(), b.Select(e => (uint)e).ToArray(), 998244353, 3).Select(e => (long)e).ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b, long mod)
        {
            var m1 = new LIB_NTT(1045430273, 3);
            var m2 = new LIB_NTT(1051721729, 6);
            var m3 = new LIB_NTT(1053818881, 7);
            var uinta = a.Select(e => (uint)e).ToArray();
            var uintb = b.Select(e => (uint)e).ToArray();
            var x = Multiply(uinta, uintb, m1.mod, m1.root);
            var y = Multiply(uinta, uintb, m2.mod, m2.root);
            var z = Multiply(uinta, uintb, m3.mod, m3.root);
            var m1_inv_m2 = m2.inverse(m1.mod);
            var m12_inv_m3 = m3.inverse(m3.mul(m1.mod, m2.mod));
            var m12_mod = (long)m1.mod * m2.mod % mod;
            var ret = new long[x.Length];
            for (var i = 0; i < x.Length; ++i)
            {
                var v1 = m2.mul(m2.sub(m2.add(y[i], m2.mod), x[i]), m1_inv_m2);
                var v2 = m3.mul(m3.sub(m3.sub(m3.add(z[i], m3.mod), x[i]), m3.mul(m1.mod, v1)), m12_inv_m3);
                ret[i] = (x[i] + ((long)m1.mod * v1 % mod) + ((long)m12_mod * v2 % mod)) % mod;
            }
            return ret;
        }
    }
    ////end
}