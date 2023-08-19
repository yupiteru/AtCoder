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
        const uint mod1 = 167772161;
        const uint root1 = 3;
        const uint mod2 = 469762049;
        const uint root2 = 3;
        const uint mod3 = 754974721;
        const uint root3 = 11;
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
        static void ntt1(ref Span<uint> a, bool rev = false) { var alen = a.Length; if (alen == 1) return; var halfn = alen >> 1; Span<uint> b = new uint[alen]; var s = pow1(root1, rev ? (mod1 - 1 - (mod1 - 1) / (uint)alen) : (mod1 - 1) / (uint)alen); int i, j, k, l, r; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod1); var modV = new System.Numerics.Vector<uint>(mods); var kp = new uint[halfn + 1]; ref uint kpref = ref kp[0]; kpref = 1; for (l = 0; l < halfn; ++l) Unsafe.Add(ref kpref, l + 1) = mul1(Unsafe.Add(ref kpref, l), s); for (i = 1; i < alen; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = Unsafe.Add(ref kpref, i * j); var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } ref uint aref = ref a[0]; ref uint bref = ref b[0]; for (k = 0; k < ten; ++k) { ref uint brefi = ref Unsafe.Add(ref bref, k + rshifti); brefi = mul1(brefi, s); } for (; k < i; ++k) { var kr = k + r; ref uint akr = ref Unsafe.Add(ref aref, kr); ref uint krhalfn = ref Unsafe.Add(ref aref, kr + halfn); Unsafe.Add(ref bref, kr + r) = (akr + krhalfn) % mod1; Unsafe.Add(ref bref, kr + r + i) = mul1(akr + mod1 - krhalfn, s); } } var t = a; a = b; b = t; } if (rev) { s = inverse1((uint)alen); ref uint aref = ref a[0]; for (i = 0; i < alen; ++i) { ref uint arefi = ref Unsafe.Add(ref aref, i); arefi = mul1(arefi, s); } } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt2(ref Span<uint> a, bool rev = false) { var alen = a.Length; if (alen == 1) return; var halfn = alen >> 1; Span<uint> b = new uint[alen]; var s = pow2(root2, rev ? (mod2 - 1 - (mod2 - 1) / (uint)alen) : (mod2 - 1) / (uint)alen); int i, j, k, l, r; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod2); var modV = new System.Numerics.Vector<uint>(mods); var kp = new uint[halfn + 1]; ref uint kpref = ref kp[0]; kpref = 1; for (l = 0; l < halfn; ++l) Unsafe.Add(ref kpref, l + 1) = mul2(Unsafe.Add(ref kpref, l), s); for (i = 1; i < alen; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = Unsafe.Add(ref kpref, i * j); var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } ref uint aref = ref a[0]; ref uint bref = ref b[0]; for (k = 0; k < ten; ++k) { ref uint brefi = ref Unsafe.Add(ref bref, k + rshifti); brefi = mul2(brefi, s); } for (; k < i; ++k) { var kr = k + r; ref uint akr = ref Unsafe.Add(ref aref, kr); ref uint krhalfn = ref Unsafe.Add(ref aref, kr + halfn); Unsafe.Add(ref bref, kr + r) = (akr + krhalfn) % mod2; Unsafe.Add(ref bref, kr + r + i) = mul2(akr + mod2 - krhalfn, s); } } var t = a; a = b; b = t; } if (rev) { s = inverse2((uint)alen); ref uint aref = ref a[0]; for (i = 0; i < alen; ++i) { ref uint arefi = ref Unsafe.Add(ref aref, i); arefi = mul2(arefi, s); } } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ntt3(ref Span<uint> a, bool rev = false) { var alen = a.Length; if (alen == 1) return; var halfn = alen >> 1; Span<uint> b = new uint[alen]; var s = pow3(root3, rev ? (mod3 - 1 - (mod3 - 1) / (uint)alen) : (mod3 - 1) / (uint)alen); int i, j, k, l, r; var regLength = System.Numerics.Vector<uint>.Count; Span<uint> mods = stackalloc uint[regLength]; mods.Fill(mod3); var modV = new System.Numerics.Vector<uint>(mods); var kp = new uint[halfn + 1]; ref uint kpref = ref kp[0]; kpref = 1; for (l = 0; l < halfn; ++l) Unsafe.Add(ref kpref, l + 1) = mul3(Unsafe.Add(ref kpref, l), s); for (i = 1; i < alen; i <<= 1, l >>= 1) { for (j = 0, r = 0; j < l; ++j, r += i) { s = Unsafe.Add(ref kpref, i * j); var ten = i - regLength; var rshift = (r << 1); var rshifti = rshift + i; for (k = 0; k < ten; k += regLength) { var u = new System.Numerics.Vector<uint>(a.Slice(k + r)); var v = new System.Numerics.Vector<uint>(a.Slice(k + r + halfn)); var add = u + v; var sub = modV + u - v; var ge = System.Numerics.Vector.GreaterThanOrEqual(add, modV); add = System.Numerics.Vector.ConditionalSelect(ge, add - modV, add); add.CopyTo(b.Slice(k + rshift)); sub.CopyTo(b.Slice(k + rshifti)); } ref uint aref = ref a[0]; ref uint bref = ref b[0]; for (k = 0; k < ten; ++k) { ref uint brefi = ref Unsafe.Add(ref bref, k + rshifti); brefi = mul3(brefi, s); } for (; k < i; ++k) { var kr = k + r; ref uint akr = ref Unsafe.Add(ref aref, kr); ref uint krhalfn = ref Unsafe.Add(ref aref, kr + halfn); Unsafe.Add(ref bref, kr + r) = (akr + krhalfn) % mod3; Unsafe.Add(ref bref, kr + r + i) = mul3(akr + mod3 - krhalfn, s); } } var t = a; a = b; b = t; } if (rev) { s = inverse3((uint)alen); ref uint aref = ref a[0]; for (i = 0; i < alen; ++i) { ref uint arefi = ref Unsafe.Add(ref aref, i); arefi = mul3(arefi, s); } } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public void ntt4(ref Span<uint> a, bool rev = false)
        {
            var alen = a.Length;
            if (alen == 1) return;
            var halfn = alen >> 1;
            Span<uint> b = new uint[alen];
            var s = pow4(root4, rev ? (mod4 - 1 - (mod4 - 1) / (uint)alen) : (mod4 - 1) / (uint)alen);
            int i, j, k, l, r;
            var regLength = System.Numerics.Vector<uint>.Count;
            Span<uint> mods = stackalloc uint[regLength];
            mods.Fill(mod4);
            var modV = new System.Numerics.Vector<uint>(mods);
            var kp = new uint[halfn + 1];
            ref uint kpref = ref kp[0];
            kpref = 1;
            for (l = 0; l < halfn; ++l) Unsafe.Add(ref kpref, l + 1) = mul4(Unsafe.Add(ref kpref, l), s);
            for (i = 1; i < alen; i <<= 1, l >>= 1)
            {
                for (j = 0, r = 0; j < l; ++j, r += i)
                {
                    s = Unsafe.Add(ref kpref, i * j);
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
                    ref uint aref = ref a[0];
                    ref uint bref = ref b[0];
                    for (k = 0; k < ten; ++k)
                    {
                        ref uint brefi = ref Unsafe.Add(ref bref, k + rshifti);
                        brefi = mul4(brefi, s);
                    }
                    for (; k < i; ++k)
                    {
                        var kr = k + r;
                        ref uint akr = ref Unsafe.Add(ref aref, kr);
                        ref uint krhalfn = ref Unsafe.Add(ref aref, kr + halfn);
                        Unsafe.Add(ref bref, kr + r) = (akr + krhalfn) % mod4;
                        Unsafe.Add(ref bref, kr + r + i) = mul4(akr + mod4 - krhalfn, s);
                    }
                }
                var t = a; a = b; b = t;
            }
            if (rev)
            {
                s = inverse4((uint)alen);
                ref uint aref = ref a[0];
                for (i = 0; i < alen; ++i)
                {
                    ref uint arefi = ref Unsafe.Add(ref aref, i);
                    arefi = mul4(arefi, s);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] NTT998244353(long[] ary, int len, bool isInverse = false)
        {
            var t = 1;
            while (t < len) t <<= 1;
            Span<uint> na = new uint[t];
            ref long aref = ref ary[0];
            ref uint naref = ref na[0];
            for (var i = 0; i < ary.Length; ++i) Unsafe.Add(ref naref, i) = (uint)Unsafe.Add(ref aref, i);
            ntt4(ref na, isInverse);
            naref = ref na[0];
            var ret = new long[t];
            ref var retref = ref ret[0];
            for (var i = 0; i < ret.Length; ++i) Unsafe.Add(ref retref, i) = Unsafe.Add(ref naref, i);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply1(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; ref uint naref = ref na[0]; ref uint nbref = ref nb[0]; Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref naref), ref Unsafe.As<uint, byte>(ref a[0]), (uint)(a.Length << 2)); Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref nbref), ref Unsafe.As<uint, byte>(ref b[0]), (uint)(b.Length << 2)); ntt1(ref na); ntt1(ref nb); naref = ref na[0]; nbref = ref nb[0]; for (var i = 0; i < t; ++i) { ref uint narefi = ref Unsafe.Add(ref naref, i); narefi = mul1(narefi, Unsafe.Add(ref nbref, i)); } ntt1(ref na, true); var ret = new uint[n]; naref = ref na[0]; ref uint retref = ref ret[0]; for (var i = 0; i < n; ++i) Unsafe.Add(ref retref, i) = Unsafe.Add(ref naref, i); return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply2(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; ref uint naref = ref na[0]; ref uint nbref = ref nb[0]; Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref naref), ref Unsafe.As<uint, byte>(ref a[0]), (uint)(a.Length << 2)); Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref nbref), ref Unsafe.As<uint, byte>(ref b[0]), (uint)(b.Length << 2)); ntt2(ref na); ntt2(ref nb); naref = ref na[0]; nbref = ref nb[0]; for (var i = 0; i < t; ++i) { ref uint narefi = ref Unsafe.Add(ref naref, i); narefi = mul2(narefi, Unsafe.Add(ref nbref, i)); } ntt2(ref na, true); var ret = new uint[n]; naref = ref na[0]; ref uint retref = ref ret[0]; for (var i = 0; i < n; ++i) Unsafe.Add(ref retref, i) = Unsafe.Add(ref naref, i); return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint[] Multiply3(uint[] a, uint[] b) { var n = a.Length + b.Length - 1; var t = 1; while (t < n) t <<= 1; Span<uint> na = new uint[t]; Span<uint> nb = new uint[t]; ref uint naref = ref na[0]; ref uint nbref = ref nb[0]; Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref naref), ref Unsafe.As<uint, byte>(ref a[0]), (uint)(a.Length << 2)); Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref nbref), ref Unsafe.As<uint, byte>(ref b[0]), (uint)(b.Length << 2)); ntt3(ref na); ntt3(ref nb); naref = ref na[0]; nbref = ref nb[0]; for (var i = 0; i < t; ++i) { ref uint narefi = ref Unsafe.Add(ref naref, i); narefi = mul3(narefi, Unsafe.Add(ref nbref, i)); } ntt3(ref na, true); var ret = new uint[n]; naref = ref na[0]; ref uint retref = ref ret[0]; for (var i = 0; i < n; ++i) Unsafe.Add(ref retref, i) = Unsafe.Add(ref naref, i); return ret; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long[] MultiplySparse(long[] a, long[] b, long mod = -1)
        {
            if (mod == -1)
            {
                var ret = new long[a.Length + b.Length - 1];
                var data = new List<(int idx, long val)>();
                var datb = new List<(int idx, long val)>();
                for (var i = 0; i < a.Length; ++i) if (a[i] != 0) data.Add((i, a[i]));
                for (var i = 0; i < b.Length; ++i) if (b[i] != 0) datb.Add((i, b[i]));
                if (data.Count * (long)datb.Count > a.Length + b.Length)
                {
                    return null;
                }
                foreach (var item in data)
                {
                    foreach (var item2 in datb)
                    {
                        ret[item.idx + item2.idx] += item.val * item2.val;
                    }
                }
                return ret;
            }
            else
            {
                var ret = new ulong[a.Length + b.Length - 1];
                var br = new LIB_BarrettReduction((ulong)mod);
                var data = new List<(int idx, ulong val)>();
                var datb = new List<(int idx, ulong val)>();
                for (var i = 0; i < a.Length; ++i) if (a[i] != 0)
                    {
                        var v = a[i] % mod;
                        if (v < 0) v += mod;
                        data.Add((i, (ulong)v));
                    }
                for (var i = 0; i < b.Length; ++i) if (b[i] != 0)
                    {
                        var v = b[i] % mod;
                        if (v < 0) v += mod;
                        datb.Add((i, (ulong)v));
                    }
                if (data.Count * (long)datb.Count > a.Length + b.Length)
                {
                    return null;
                }
                foreach (var item in data)
                {
                    foreach (var item2 in datb)
                    {
                        ret[item.idx + item2.idx] += br.Reduce(item.val * item2.val);
                    }
                }
                return ret.Select(e => (long)(br.Reduce(e))).ToArray();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b)
        {
            var sparse = MultiplySparse(a, b, mod4);
            if (sparse != null) return sparse;
            var n = a.Length + b.Length - 1;
            var t = 1;
            while (t < n) t <<= 1;
            Span<uint> na = new uint[t];
            Span<uint> nb = new uint[t];
            ref long aref = ref a[0];
            ref long bref = ref b[0];
            ref uint naref = ref na[0];
            ref uint nbref = ref nb[0];
            for (var i = 0; i < a.Length; ++i) Unsafe.Add(ref naref, i) = (uint)(Unsafe.Add(ref aref, i) % mod4);
            for (var i = 0; i < b.Length; ++i) Unsafe.Add(ref nbref, i) = (uint)(Unsafe.Add(ref bref, i) % mod4);
            ntt4(ref na);
            ntt4(ref nb);
            naref = ref na[0];
            nbref = ref nb[0];
            for (var i = 0; i < t; ++i)
            {
                ref uint narefi = ref Unsafe.Add(ref naref, i);
                narefi = mul4(narefi, Unsafe.Add(ref nbref, i));
            }
            ntt4(ref na, true);
            var ret = new long[n];
            naref = ref na[0];
            ref long retref = ref ret[0];
            for (var i = 0; i < n; ++i) Unsafe.Add(ref retref, i) = Unsafe.Add(ref naref, i);
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] Multiply(long[] a, long[] b, long mod)
        {
            var sparse = MultiplySparse(a, b, mod);
            if (sparse != null) return sparse;
            if (mod == 998244353) return Multiply(a, b);
            var x = Multiply1(a.Select(e => (uint)(e % mod1)).ToArray(), b.Select(e => (uint)(e % mod1)).ToArray());
            var y = Multiply2(a.Select(e => (uint)(e % mod2)).ToArray(), b.Select(e => (uint)(e % mod2)).ToArray());
            var z = Multiply3(a.Select(e => (uint)(e % mod3)).ToArray(), b.Select(e => (uint)(e % mod3)).ToArray());
            var m1_inv_m2 = inverse2(mod1);
            var m12_inv_m3 = inverse3(mul3(mod1, mod2));
            var m12_mod = (long)mod1 * mod2 % mod;
            var ret = new long[x.Length];
            ref long retref = ref ret[0];
            ref uint xref = ref x[0];
            ref uint yref = ref y[0];
            ref uint zref = ref z[0];
            for (var i = 0; i < x.Length; ++i)
            {
                var xv = Unsafe.Add(ref xref, i);
                var v1 = mul2(Unsafe.Add(ref yref, i) + (mod2 << 1) - xv, m1_inv_m2);
                var v2 = mul3(Unsafe.Add(ref zref, i) + (mod3 << 1) - xv + mod3 - mul3(mod1, v1), m12_inv_m3);
                Unsafe.Add(ref retref, i) = (xv + ((long)mod1 * v1 % mod) + (m12_mod * v2 % mod)) % mod;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] MultiplyLong(long[] a, long[] b)
        {
            var sparse = MultiplySparse(a, b);
            if (sparse != null) return sparse;
            unchecked
            {
                const ulong mod23 = (ulong)mod2 * mod3;
                const ulong mod13 = (ulong)mod1 * mod3;
                const ulong mod12 = (ulong)mod1 * mod2;
                const ulong mod123 = (ulong)mod1 * mod2 * mod3;
                ulong[] offset = new ulong[] { 0, 0, mod123, 2 * mod123, 3 * mod123 };

                const ulong i1 = 190329765;
                const ulong i2 = 58587104;
                const ulong i3 = 187290749;

                var c1 = Multiply1(a.Select(e => (uint)(e % mod1) + mod1).ToArray(), b.Select(e => (uint)(e % mod1) + mod1).ToArray());
                var c2 = Multiply2(a.Select(e => (uint)(e % mod2) + mod2).ToArray(), b.Select(e => (uint)(e % mod2) + mod2).ToArray());
                var c3 = Multiply3(a.Select(e => (uint)(e % mod3) + mod3).ToArray(), b.Select(e => (uint)(e % mod3) + mod3).ToArray());

                var c = new long[a.Length + b.Length - 1];
                for (var i = 0; i < c.Length; ++i)
                {
                    var x = 0UL;
                    x += c1[i] * i1 % mod1 * mod23;
                    x += c2[i] * i2 % mod2 * mod13;
                    x += c3[i] * i3 % mod3 * mod12;

                    var tmp = (long)x % mod1;
                    if (tmp < 0) tmp += mod1;
                    var diff = c1[i] - tmp;
                    if (diff < 0) diff += mod1;

                    c[i] = (long)(x - offset[diff % 5]);
                }
                return c;
            }
        }

        public class RelaxedConvolutionImpl
        {
            int N;
            int q;
            long[] a;
            long[] b;
            long[] c;
            List<uint[]> aList;
            List<uint[]> bList;

            public RelaxedConvolutionImpl(long N)
            {
                this.N = (int)N;
                q = 0;
                a = new long[N + 1];
                b = new long[N + 1];
                c = new long[N + 1];
                aList = new List<uint[]>();
                bList = new List<uint[]>();
            }

            public long Get(long x, long y)
            {
                a[q] = (LIB_Mod998244353)x;
                b[q] = (LIB_Mod998244353)y;
                c[q] = (c[q] + a[q] * b[0] + (q == 0 ? 0 : a[0] * b[q])) % mod4;

                ++q;
                if (q > N) return c[q - 1];
                var lg = -1;
                for (var d = 1; d <= q; d *= 2)
                {
                    ++lg;
                    if (q % (2 * d) != d) continue;
                    Span<uint> f = new uint[d * 2];
                    Span<uint> g = new uint[d * 2];
                    if (q == d)
                    {
                        for (var i = 0; i < d; ++i)
                        {
                            f[i] = (uint)a[i];
                            g[i] = (uint)b[i];
                        }
                        ntt4(ref f);
                        ntt4(ref g);
                        for (var i = 0; i < d * 2; ++i) f[i] = mul4(f[i], g[i]);
                    }
                    else
                    {
                        if (lg == aList.Count)
                        {
                            Span<uint> s2 = new uint[d * 2];
                            Span<uint> t2 = new uint[d * 2];
                            for (var i = 0; i < d * 2; ++i)
                            {
                                s2[i] = (uint)a[i];
                                t2[i] = (uint)b[i];
                            }
                            ntt4(ref s2);
                            ntt4(ref t2);
                            aList.Add(s2.ToArray());
                            bList.Add(t2.ToArray());
                        }

                        for (var i = 0; i < d; ++i)
                        {
                            f[i] = (uint)(a[q - d + i]);
                            g[i] = (uint)(b[q - d + i]);
                        }
                        Span<uint> s = aList[lg];
                        Span<uint> t = bList[lg];
                        ntt4(ref f);
                        ntt4(ref g);
                        for (var i = 0; i < d * 2; ++i) f[i] = (mul4(f[i], t[i]) + mul4(g[i], s[i])) % mod4;
                    }
                    ntt4(ref f, true);
                    for (var i = q; i < Min(q + d, N + 1); ++i) c[i] = (c[i] + f[d + i - q]) % mod4;
                }
                return c[q - 1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public RelaxedConvolutionImpl RelaxedConvolution(long N)
        {
            return new RelaxedConvolutionImpl(N);
        }
    }
    ////end
}