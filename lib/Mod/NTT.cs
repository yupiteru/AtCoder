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
        static void ntt1(ref Span<uint> a, bool rev = false) { if (a.Length == 1) return; var halfn = a.Length >> 1; Span<uint> b = new uint[a.Length]; var s = pow1(root1, rev ? (mod1 - 1 - (mod1 - 1) / (uint)a.Length) : (mod1 - 1) / (uint)a.Length); int i, j, k, l, r; var kp = new uint[halfn + 1]; kp[0] = 1; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod1); var modV = new System.Numerics.Vector<uint>(mods); for (i = 0; i < kp.Length - 1; ++i) kp[i + 1] = mul1(kp[i], s); for (i = 1, l = halfn; i < a.Length; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = kp[i * j]; var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } for (k = 0; k < ten; ++k) b[k + rshifti] = mul1(b[k + rshifti], s); for (; k < i; ++k) { var kr = k + r; var krhalfn = kr + halfn; b[kr + r] = (a[kr] + a[krhalfn]) % mod1; b[kr + r + i] = mul1(a[kr] + mod1 - a[krhalfn], s); } } var t = a; a = b; b = t; } if (rev) { s = inverse1((uint)a.Length); for (i = 0; i < a.Length; ++i) a[i] = mul1(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt2(ref Span<uint> a, bool rev = false) { if (a.Length == 1) return; var halfn = a.Length >> 1; Span<uint> b = new uint[a.Length]; var s = pow2(root2, rev ? (mod2 - 1 - (mod2 - 1) / (uint)a.Length) : (mod2 - 1) / (uint)a.Length); int i, j, k, l, r; var kp = new uint[halfn + 1]; kp[0] = 1; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod2); var modV = new System.Numerics.Vector<uint>(mods); for (i = 0; i < kp.Length - 1; ++i) kp[i + 1] = mul2(kp[i], s); for (i = 1, l = halfn; i < a.Length; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = kp[i * j]; var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } for (k = 0; k < ten; ++k) b[k + rshifti] = mul2(b[k + rshifti], s); for (; k < i; ++k) { var kr = k + r; var krhalfn = kr + halfn; b[kr + r] = (a[kr] + a[krhalfn]) % mod2; b[kr + r + i] = mul2(a[kr] + mod2 - a[krhalfn], s); } } var t = a; a = b; b = t; } if (rev) { s = inverse2((uint)a.Length); for (i = 0; i < a.Length; ++i) a[i] = mul2(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt3(ref Span<uint> a, bool rev = false) { if (a.Length == 1) return; var halfn = a.Length >> 1; Span<uint> b = new uint[a.Length]; var s = pow3(root3, rev ? (mod3 - 1 - (mod3 - 1) / (uint)a.Length) : (mod3 - 1) / (uint)a.Length); int i, j, k, l, r; var kp = new uint[halfn + 1]; kp[0] = 1; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod3); var modV = new System.Numerics.Vector<uint>(mods); for (i = 0; i < kp.Length - 1; ++i) kp[i + 1] = mul3(kp[i], s); for (i = 1, l = halfn; i < a.Length; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = kp[i * j]; var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } for (k = 0; k < ten; ++k) b[k + rshifti] = mul3(b[k + rshifti], s); for (; k < i; ++k) { var kr = k + r; var krhalfn = kr + halfn; b[kr + r] = (a[kr] + a[krhalfn]) % mod3; b[kr + r + i] = mul3(a[kr] + mod3 - a[krhalfn], s); } } var t = a; a = b; b = t; } if (rev) { s = inverse3((uint)a.Length); for (i = 0; i < a.Length; ++i) a[i] = mul3(a[i], s); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt4(ref Span<uint> a, bool rev = false)
        {
            if (a.Length == 1) return;
            var halfn = a.Length >> 1;
            Span<uint> b = new uint[a.Length];
            var s = pow4(root4, rev ? (mod4 - 1 - (mod4 - 1) / (uint)a.Length) : (mod4 - 1) / (uint)a.Length);
            int i, j, k, l, r;
            var kp = new uint[halfn + 1];
            kp[0] = 1;
            var regLength = System.Numerics.Vector<uint>.Count;
            Span<uint> mods = stackalloc uint[regLength];
            mods.Fill(mod4);
            var modV = new System.Numerics.Vector<uint>(mods);
            for (i = 0; i < kp.Length - 1; ++i) kp[i + 1] = mul4(kp[i], s);
            for (i = 1, l = halfn; i < a.Length; i <<= 1, l >>= 1)
            {
                for (j = 0, r = 0; j < l; ++j, r += i)
                {
                    s = kp[i * j];
                    var ten = i - regLength;
                    var rshift = (r << 1);
                    var rshifti = rshift + i;
                    for (k = 0; k < ten; k += regLength)
                    {
                        var u = new System.Numerics.Vector<uint>(a.Slice(k + r));
                        var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn));
                        var add = u + v;
                        var sub = modV + u - v;
                        var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV);
                        add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add);
                        add.CopyTo(b.Slice(k + rshift));
                        sub.CopyTo(b.Slice(k + rshifti));
                    }
                    for (k = 0; k < ten; ++k) b[k + rshifti] = mul4(b[k + rshifti], s);
                    for (; k < i; ++k)
                    {
                        var kr = k + r;
                        var krhalfn = kr + halfn;
                        b[kr + r] = (a[kr] + a[krhalfn]) % mod4;
                        b[kr + r + i] = mul4(a[kr] + mod4 - a[krhalfn], s);
                    }
                }
                var t = a; a = b; b = t;
            }
            if (rev)
            {
                s = inverse4((uint)a.Length);
                for (i = 0; i < a.Length; ++i) a[i] = mul4(a[i], s);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply1(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt1(ref na); ntt1(ref nb); for (var i = 0; i < t; ++i) na[i] = mul1(na[i], nb[i]); ntt1(ref na, true); var ret = new uint[n]; for (var i = 0; i < n; ++i) ret[i] = na[i]; return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply2(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt2(ref na); ntt2(ref nb); for (var i = 0; i < t; ++i) na[i] = mul2(na[i], nb[i]); ntt2(ref na, true); var ret = new uint[n]; for (var i = 0; i < n; ++i) ret[i] = na[i]; return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply3(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; for (var i = 0; i < a.Length; ++i) na[i] = a[i]; for (var i = 0; i < b.Length; ++i) nb[i] = b[i]; ntt3(ref na); ntt3(ref nb); for (var i = 0; i < t; ++i) na[i] = mul3(na[i], nb[i]); ntt3(ref na, true); var ret = new uint[n]; for (var i = 0; i < n; ++i) ret[i] = na[i]; return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b)
        {
            var n = a.Length + b.Length - 1;
            var t = 1;
            while (t < n) t <<= 1;
            Span<uint> na = new uint[t];
            Span<uint> nb = new uint[t];
            for (var i = 0; i < a.Length; ++i) na[i] = (uint)a[i];
            for (var i = 0; i < b.Length; ++i) nb[i] = (uint)b[i];
            ntt4(ref na);
            ntt4(ref nb);
            for (var i = 0; i < t; ++i) na[i] = mul4(na[i], nb[i]);
            ntt4(ref na, true);
            var ret = new long[n];
            for (var i = 0; i < n; ++i) ret[i] = na[i];
            return ret;
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
                var v1 = mul2(y[i] + (mod2 << 1) - x[i], m1_inv_m2);
                var v2 = mul3(z[i] + (mod3 << 1) - x[i] + mod3 - mul3(mod1, v1), m12_inv_m3);
                ret[i] = (x[i] + ((long)mod1 * v1 % mod) + (m12_mod * v2 % mod)) % mod;
            }
            return ret;
        }
    }
    ////end
}