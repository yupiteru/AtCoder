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
        const uint mod1 = 1045430273;
        const uint root1 = 3;
        const uint mod2 = 1051721729;
        const uint root2 = 6;
        const uint mod3 = 1053818881;
        const uint root3 = 7;
        const uint mod4 = 998244353;
        const uint root4 = 3;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint add1(uint x, uint y) => (x + y < mod1) ? (x + y) : (x + y - mod1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint add2(uint x, uint y) => (x + y < mod2) ? (x + y) : (x + y - mod2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint add3(uint x, uint y) => (x + y < mod3) ? (x + y) : (x + y - mod3);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint add4(uint x, uint y) => (x + y < mod4) ? (x + y) : (x + y - mod4);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint sub1(uint x, uint y) => (x >= y) ? (x - y) : (mod1 - y + x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint sub2(uint x, uint y) => (x >= y) ? (x - y) : (mod2 - y + x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint sub3(uint x, uint y) => (x >= y) ? (x - y) : (mod3 - y + x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint sub4(uint x, uint y) => (x >= y) ? (x - y) : (mod4 - y + x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint mul1(uint x, uint y) => (uint)((ulong)x * y % mod1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint mul2(uint x, uint y) => (uint)((ulong)x * y % mod2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint mul3(uint x, uint y) => (uint)((ulong)x * y % mod3);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint mul4(uint x, uint y) => (uint)((ulong)x * y % mod4);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint pow1(uint x, uint n) { uint res = 1; while (n > 0) { if ((n & 1) != 0) res = mul1(res, x); x = mul1(x, x); n >>= 1; } return res; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint pow2(uint x, uint n) { uint res = 1; while (n > 0) { if ((n & 1) != 0) res = mul2(res, x); x = mul2(x, x); n >>= 1; } return res; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint pow3(uint x, uint n) { uint res = 1; while (n > 0) { if ((n & 1) != 0) res = mul3(res, x); x = mul3(x, x); n >>= 1; } return res; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint pow4(uint x, uint n) { uint res = 1; while (n > 0) { if ((n & 1) != 0) res = mul4(res, x); x = mul4(x, x); n >>= 1; } return res; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint inverse1(uint x) { long b = mod1, r = 1, u = 0, t = 0, x2 = x; while (b > 0) { var q = x2 / b; t = u; u = r - q * u; r = t; t = b; b = x2 - q * b; x2 = t; } return (uint)(r < 0 ? r + mod1 : r); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint inverse2(uint x) { long b = mod2, r = 1, u = 0, t = 0, x2 = x; while (b > 0) { var q = x2 / b; t = u; u = r - q * u; r = t; t = b; b = x2 - q * b; x2 = t; } return (uint)(r < 0 ? r + mod2 : r); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint inverse3(uint x) { long b = mod3, r = 1, u = 0, t = 0, x2 = x; while (b > 0) { var q = x2 / b; t = u; u = r - q * u; r = t; t = b; b = x2 - q * b; x2 = t; } return (uint)(r < 0 ? r + mod3 : r); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint inverse4(uint x) { long b = mod4, r = 1, u = 0, t = 0, x2 = x; while (b > 0) { var q = x2 / b; t = u; u = r - q * u; r = t; t = b; b = x2 - q * b; x2 = t; } return (uint)(r < 0 ? r + mod4 : r); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt1(ref uint[] a, bool rev = false) { var n = (uint)a.Length; if (n == 1) return; var b = new uint[n]; var s = pow1(root1, rev ? (mod1 - 1 - (mod1 - 1) / n) : (mod1 - 1) / n); var kp = Enumerable.Repeat((uint)1, (int)(n / 2 + 1)).ToArray(); uint i, j, k, l, r; for (i = 0; i < n / 2; ++i) kp[i + 1] = mul1(kp[i], s); for (i = 1, l = n / 2; i < n; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { for (k = 0, s = kp[i * j]; k < i; ++k) { var p = a[k + r]; var q = a[k + r + n / 2]; b[k + 2 * r] = add1(p, q); b[k + 2 * r + i] = mul1(sub1(p, q), s); } } var t = a; a = b; b = t; } if (rev) { s = inverse1(n); for (i = 0; i < n; ++i) a[i] = mul1(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt2(ref uint[] a, bool rev = false) { var n = (uint)a.Length; if (n == 1) return; var b = new uint[n]; var s = pow2(root2, rev ? (mod2 - 1 - (mod2 - 1) / n) : (mod2 - 1) / n); var kp = Enumerable.Repeat((uint)1, (int)(n / 2 + 1)).ToArray(); uint i, j, k, l, r; for (i = 0; i < n / 2; ++i) kp[i + 1] = mul2(kp[i], s); for (i = 1, l = n / 2; i < n; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { for (k = 0, s = kp[i * j]; k < i; ++k) { var p = a[k + r]; var q = a[k + r + n / 2]; b[k + 2 * r] = add2(p, q); b[k + 2 * r + i] = mul2(sub2(p, q), s); } } var t = a; a = b; b = t; } if (rev) { s = inverse2(n); for (i = 0; i < n; ++i) a[i] = mul2(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt3(ref uint[] a, bool rev = false) { var n = (uint)a.Length; if (n == 1) return; var b = new uint[n]; var s = pow3(root3, rev ? (mod3 - 1 - (mod3 - 1) / n) : (mod3 - 1) / n); var kp = Enumerable.Repeat((uint)1, (int)(n / 2 + 1)).ToArray(); uint i, j, k, l, r; for (i = 0; i < n / 2; ++i) kp[i + 1] = mul3(kp[i], s); for (i = 1, l = n / 2; i < n; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { for (k = 0, s = kp[i * j]; k < i; ++k) { var p = a[k + r]; var q = a[k + r + n / 2]; b[k + 2 * r] = add3(p, q); b[k + 2 * r + i] = mul3(sub3(p, q), s); } } var t = a; a = b; b = t; } if (rev) { s = inverse3(n); for (i = 0; i < n; ++i) a[i] = mul3(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt4(ref uint[] a, bool rev = false)
        {
            var n = (uint)a.Length;
            if (n == 1) return;
            var b = new uint[n];
            var s = pow4(root4, rev ? (mod4 - 1 - (mod4 - 1) / n) : (mod4 - 1) / n);
            var kp = Enumerable.Repeat((uint)1, (int)(n / 2 + 1)).ToArray();
            uint i, j, k, l, r;
            for (i = 0; i < n / 2; ++i) kp[i + 1] = mul4(kp[i], s);
            for (i = 1, l = n / 2; i < n; i <<= 1, l >>= 1)
            {
                for (j = 0, r = 0; j < l; ++j, r += i)
                {
                    for (k = 0, s = kp[i * j]; k < i; ++k)
                    {
                        var p = a[k + r]; var q = a[k + r + n / 2];
                        b[k + 2 * r] = add4(p, q);
                        b[k + 2 * r + i] = mul4(sub4(p, q), s);
                    }
                }
                var t = a; a = b; b = t;
            }
            if (rev)
            {
                s = inverse4(n);
                for (i = 0; i < n; ++i) a[i] = mul4(a[i], s);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply1(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; var na = new uint[t]; var nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt1(ref na); ntt1(ref nb); for (var i = 0; i < t; ++i) na[i] = mul1(na[i], nb[i]); ntt1(ref na, true); return na.Take(n).ToArray(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply2(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; var na = new uint[t]; var nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt2(ref na); ntt2(ref nb); for (var i = 0; i < t; ++i) na[i] = mul2(na[i], nb[i]); ntt2(ref na, true); return na.Take(n).ToArray(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply3(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; var na = new uint[t]; var nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt3(ref na); ntt3(ref nb); for (var i = 0; i < t; ++i) na[i] = mul3(na[i], nb[i]); ntt3(ref na, true); return na.Take(n).ToArray(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b)
        {
            var n = a.Length + b.Length - 1;
            var t = 1;
            while (t < n) t <<= 1;
            var na = new uint[t];
            var nb = new uint[t];
            for (var i = 0; i < a.Length; ++i) na[i] = (uint)a[i];
            for (var i = 0; i < b.Length; ++i) nb[i] = (uint)b[i];
            ntt4(ref na);
            ntt4(ref nb);
            for (var i = 0; i < t; ++i) na[i] = mul4(na[i], nb[i]);
            ntt4(ref na, true);
            return na.Take(n).Select(e => (long)e).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b, long mod)
        {
            if (mod == 998244353) return Multiply(a, b);
            var uinta = a.Select(e => (uint)e).ToArray();
            var uintb = b.Select(e => (uint)e).ToArray();
            var x = Multiply1(uinta, uintb);
            var y = Multiply2(uinta, uintb);
            var z = Multiply3(uinta, uintb);
            var m1_inv_m2 = inverse2(mod1);
            var m12_inv_m3 = inverse3(mul3(mod1, mod2));
            var m12_mod = (long)mod1 * mod2 % mod;
            var ret = new long[x.Length];
            for (var i = 0; i < x.Length; ++i)
            {
                var v1 = mul2(sub2(add2(y[i], mod2), x[i]), m1_inv_m2);
                var v2 = mul3(sub3(sub3(add3(z[i], mod3), x[i]), mul3(mod1, v1)), m12_inv_m3);
                ret[i] = (x[i] + ((long)mod1 * v1 % mod) + ((long)m12_mod * v2 % mod)) % mod;
            }
            return ret;
        }
    }
    ////end
}