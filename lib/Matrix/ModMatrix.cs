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
    class LIB_ModMatrix
    {
        const uint _mod = 998244353;
        ulong[,] dat;
        int n;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ModMatrix(long n)
        {
            this.n = (int)n;
            dat = new ulong[n, n];
        }
        public long this[long row, long col]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (long)dat[row, col]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { dat[row, col] = (ulong)(((value % _mod) + _mod) % _mod); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_ModMatrix operator *(LIB_ModMatrix x, LIB_ModMatrix y)
        {
            var ret = new LIB_ModMatrix(x.n);
            for (var i = 0; i < x.n; ++i)
            {
                for (var j = 0; j < x.n; ++j)
                {
                    for (var k = 0; k < x.n; ++k)
                    {
                        ret.dat[i, j] += x.dat[i, k] * y.dat[k, j] % _mod;
                    }
                    ret.dat[i, j] %= _mod;
                }
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_ModMatrix operator *(long x, LIB_ModMatrix y) => y * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_ModMatrix operator *(LIB_ModMatrix x, long y)
        {
            var trim = y % _mod;
            if (trim < 0) trim += _mod;
            var ret = new LIB_ModMatrix(x.n);
            for (var i = 0; i < x.n; ++i)
            {
                for (var j = 0; j < x.n; ++j)
                {
                    ret.dat[i, j] = x.dat[i, j] * (ulong)trim % _mod;
                }
            }
            return ret;
        }
        public LIB_ModMatrix Pow(long y)
        {
            var a = new ulong[n, n];
            var b = new ulong[n, n];
            var c = new ulong[n, n];
            ref var refa = ref a[0, 0];
            ref var refb = ref b[0, 0];
            ref var retref = ref c[0, 0];
            for (var i = 0; i < n; ++i) Unsafe.Add(ref retref, i * n + i) = 1;
            var size = (uint)(Unsafe.SizeOf<ulong>() * n * n);
            Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref refa), ref Unsafe.As<ulong, byte>(ref dat[0, 0]), size);
            while (y > 0)
            {
                if ((y & 1) != 0)
                {
                    Unsafe.InitBlock(ref Unsafe.As<ulong, byte>(ref refb), 0, size);
                    for (var i = 0; i < n; ++i)
                    {
                        for (var j = 0; j < n; ++j)
                        {
                            ref var reftmp = ref Unsafe.Add(ref refb, i * n + j);
                            for (var k = 0; k < n; ++k)
                            {
                                reftmp += Unsafe.Add(ref retref, i * n + k) * Unsafe.Add(ref refa, k * n + j) % _mod;
                            }
                            reftmp %= _mod;
                        }
                    }
                    ref var t = ref retref;
                    retref = ref refb;
                    refb = ref t;
                }
                Unsafe.InitBlock(ref Unsafe.As<ulong, byte>(ref refb), 0, size);
                for (var i = 0; i < n; ++i)
                {
                    for (var j = 0; j < n; ++j)
                    {
                        ref var reftmp = ref Unsafe.Add(ref refb, i * n + j);
                        for (var k = 0; k < n; ++k)
                        {
                            reftmp += Unsafe.Add(ref refa, i * n + k) * Unsafe.Add(ref refa, k * n + j) % _mod;
                        }
                        reftmp %= _mod;
                    }
                }
                {
                    ref var t = ref refa;
                    refa = ref refb;
                    refb = ref t;
                }

                y >>= 1;
            }
            var ret = new LIB_ModMatrix(n);
            Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref ret.dat[0, 0]), ref Unsafe.As<ulong, byte>(ref retref), size);
            return ret;
        }
        /// <summary>
        /// 固有多項式
        /// det(x I - a)
        /// O(n^3)
        /// </summary>
        public long[] CharacteristicPoly()
        {
            var a = new ulong[n][];
            ref var datref = ref dat[0, 0];
            for (var i = 0; i < n; ++i)
            {
                a[i] = new ulong[n];
                for (var j = 0; j < n; ++j)
                {
                    a[i][j] = _mod - (Unsafe.Add(ref datref, i * n + j) == 0 ? _mod : Unsafe.Add(ref datref, i * n + j));
                }
            }
            for (var j = 0; j < n - 2; ++j)
            {
                for (var i = j + 1; i < n; ++i)
                {
                    if (a[i][j] != 0)
                    {
                        var t = a[j + 1]; a[j + 1] = a[i]; a[i] = t;
                        for (var ii = 0; ii < n; ++ii)
                        {
                            var t2 = a[ii][j + 1]; a[ii][j + 1] = a[ii][i]; a[ii][i] = t2;
                        }
                        break;
                    }
                }
                if (a[j + 1][j] != 0)
                {
                    ref var aj1ref = ref a[j + 1][0];
                    var s = (ulong)LIB_Mod998244353.Inverse((long)a[j + 1][j]);
                    for (var i = j + 2; i < n; ++i)
                    {
                        ref var airef = ref a[i][0];
                        var t = s * a[i][j] % _mod;
                        for (var jj = j; jj < n; ++jj) Unsafe.Add(ref airef, jj) = (Unsafe.Add(ref airef, jj) + _mod - t * Unsafe.Add(ref aj1ref, jj) % _mod) % _mod;
                        for (var ii = 0; ii < n; ++ii) a[ii][j + 1] = (a[ii][j + 1] + t * a[ii][i]) % _mod;
                    }
                }
            }
            var fss = new long[(n + 1) * (n + 2) / 2];
            var offlist = new int[n + 1];
            fss[0] = 1;
            var off = 0;
            for (var i = 0; i < n; ++i)
            {
                ref var fssiref = ref fss[off];
                offlist[i + 1] = off += i + 1;
                ref var fssi1ref = ref fss[off];
                var aii = (long)a[i][i];
                for (var k = 0; k <= i; ++k) Unsafe.Add(ref fssi1ref, k + 1) = Unsafe.Add(ref fssiref, k);
                for (var k = 0; k <= i; ++k) Unsafe.Add(ref fssi1ref, k) = (Unsafe.Add(ref fssi1ref, k) + aii * Unsafe.Add(ref fssiref, k)) % _mod;
                var prod = 1L;
                for (var j = i - 1; j >= 0; --j)
                {
                    ref var fssjref = ref fss[offlist[j]];
                    prod = prod * -(long)a[j + 1][j] % _mod;
                    var t = prod * (long)a[j][i] % _mod;
                    for (var k = 0; k <= j; ++k) Unsafe.Add(ref fssi1ref, k) = (_mod + Unsafe.Add(ref fssi1ref, k) + t * Unsafe.Add(ref fssjref, k) % _mod) % _mod;
                }
            }
            var ret = new long[n + 1];
            Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref ret[0]), ref Unsafe.As<long, byte>(ref fss[offlist[n]]), (uint)(Unsafe.SizeOf<long>() * (n + 1)));
            return ret;
        }
        /// <summary>
        /// 行列式
        /// det(a[0] + x a[1] + ... + x^m a[m])
        /// O((m n)^3)
        /// </summary>
        static public long[] DeterminantPoly(params LIB_ModMatrix[] a)
        {
            var m = a.Length - 1;
            var n = a[0].n;
            ref var mdatref = ref a[m].dat[0, 0];
            var prod = 1L;
            var off = 0;
            for (var h = 0; h < n; ++h)
            {
                while (true)
                {
                    if (Unsafe.Add(ref mdatref, h * n + h) != 0) break;
                    for (var j = h + 1; j < n; ++j)
                    {
                        if (Unsafe.Add(ref mdatref, h * n + j) != 0)
                        {
                            prod *= -1;
                            for (var d = 0; d <= m; ++d)
                            {
                                ref var datref = ref a[d].dat[0, 0];
                                for (var i = 0; i < n; ++i)
                                {
                                    var t = Unsafe.Add(ref datref, i * n + h); Unsafe.Add(ref datref, i * n + h) = Unsafe.Add(ref datref, i * n + j); Unsafe.Add(ref datref, i * n + j) = t;
                                }
                            }
                            break;
                        }
                    }
                    if (Unsafe.Add(ref mdatref, h * n + h) != 0) break;
                    if (++off > m * n) return new long[m * n + 1];
                    for (var d = m; --d >= 0;)
                    {
                        for (var j = 0; j < n; ++j)
                        {
                            a[d + 1].dat[h, j] = a[d].dat[h, j];
                        }
                    }
                    for (var j = 0; j < n; ++j)
                    {
                        a[0].dat[h, j] = 0;
                    }
                    for (var i = 0; i < h; ++i)
                    {
                        var t = Unsafe.Add(ref mdatref, h * n + i);
                        for (var d = 0; d <= m; ++d)
                        {
                            ref var datref = ref a[d].dat[0, 0];
                            for (var j = 0; j < n; ++j)
                            {
                                Unsafe.Add(ref datref, h * n + j) = (Unsafe.Add(ref datref, h * n + j) + _mod - t * Unsafe.Add(ref datref, i * n + j) % _mod) % _mod;
                            }
                        }
                    }
                }
                prod = prod * (long)Unsafe.Add(ref mdatref, h * n + h) % _mod;
                var s = (ulong)LIB_Mod998244353.Inverse((long)Unsafe.Add(ref mdatref, h * n + h));
                for (var d = 0; d <= m; ++d)
                {
                    ref var datref = ref a[d].dat[0, 0];
                    for (var j = 0; j < n; ++j)
                    {
                        Unsafe.Add(ref datref, h * n + j) = Unsafe.Add(ref datref, h * n + j) * s % _mod;
                    }
                }
                for (var i = 0; i < n; ++i)
                {
                    if (h != i)
                    {
                        var t = Unsafe.Add(ref mdatref, i * n + h);
                        for (var d = 0; d <= m; ++d)
                        {
                            ref var datref = ref a[d].dat[0, 0];
                            for (var j = 0; j < n; ++j)
                            {
                                Unsafe.Add(ref datref, i * n + j) = (Unsafe.Add(ref datref, i * n + j) + _mod - t * Unsafe.Add(ref datref, h * n + j) % _mod) % _mod;
                            }
                        }
                    }
                }
            }
            var fs = new[] { 1L };
            if (m * n > 0)
            {
                var b = new LIB_ModMatrix(m * n);
                ref var bdat = ref b.dat[0, 0];
                for (var i = 0; i < (m - 1) * n; ++i)
                {
                    Unsafe.Add(ref bdat, i * n + n + i) = 1;
                }
                var geta = m * n * n - n * n;
                for (var d = 0; d < m; ++d)
                {
                    ref var datref = ref a[d].dat[0, 0];
                    for (var i = 0; i < n; ++i)
                    {
                        for (var j = 0; j < n; ++j)
                        {
                            Unsafe.Add(ref bdat, geta + (i + d) * n + j) = Unsafe.Add(ref datref, i * n + j) == 0 ? 0 : _mod - Unsafe.Add(ref datref, i * n + j);
                        }
                    }
                }
                fs = b.CharacteristicPoly();
            }
            var gs = new long[m * n + 1];
            for (var i = 0; off + i <= m * n; ++i)
            {
                gs[i] = (_mod + prod * fs[off + i] % _mod) % _mod;
            }
            return gs;
        }
    }
    ////end
}