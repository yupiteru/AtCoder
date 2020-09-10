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
    class LIB_LazySegTree<T, E> where E : IEquatable<E>
    {
        public delegate T F(ref T x, ref T y);
        public delegate T G(ref T x, ref E y, ref int c);
        public delegate E H(ref E x, ref E y);
        int n, height, sz;
        int[] rangeSz;
        T ti;
        E ei;
        F f;
        G g;
        H h;
        T[] dat;
        E[] laz;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(long _n, T _ti, E _ei, F _f, G _g, H _h)
        {
            n = 1; height = 0; sz = (int)_n;
            while (n < _n) { n <<= 1; ++height; }
            ti = _ti;
            ei = _ei;
            f = _f;
            g = _g;
            h = _h;
            dat = Enumerable.Repeat(ti, n << 1).ToArray();
            laz = Enumerable.Repeat(ei, n).ToArray();
            rangeSz = new int[n << 1];
            for (var i = 0; i < sz; i++) rangeSz[n + i] = 1;
            for (var i = n - 1; i > 0; i--)
            {
                rangeSz[i] = rangeSz[(i << 1) | 0] + rangeSz[(i << 1) | 1];
                dat[i] = f(ref dat[(i << 1) | 0], ref dat[(i << 1) | 1]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(IEnumerable<T> l, T _ti, E _ei, F _f, G _g, H _h) : this(l.Count(), _ti, _ei, _f, _g, _h)
        {
            var idx = 0;
            foreach (var item in l) dat[n + idx++] = item;
            for (var i = n - 1; i > 0; i--) dat[i] = f(ref dat[(i << 1) | 0], ref dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Apply(long i, ref E v)
        {
            if (v.Equals(ei)) return;
            dat[i] = g(ref dat[i], ref v, ref rangeSz[i]);
            if (i < n) laz[i] = h(ref laz[i], ref v);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Up(long i) => dat[i] = f(ref dat[i << 1], ref dat[(i << 1) | 1]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Down(long i)
        {
            if (laz[i].Equals(ei)) return;
            var cl = i << 1;
            var cr = cl + 1;
            Apply(cl, ref laz[i]);
            Apply(cr, ref laz[i]);
            laz[i] = ei;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Thrust(long i)
        {
            for (var j = height; j > 0; j--) Down(i >> j);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Recalc(long i)
        {
            while ((i >>= 1) > 0) Up(i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, E v)
        {
            if (l == r) return;
            l += n;
            r += n;
            for (var i = height; i >= 1; --i)
            {
                if (((l >> i) << i) != l) Down(l >> i);
                if (((r >> i) << i) != r) Down((r - 1) >> i);
            }
            for (long li = l, ri = r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) { Apply(li, ref v); ++li; }
                if ((ri & 1) == 1) { --ri; Apply(ri, ref v); }
            }
            for (var i = 1; i <= height; ++i)
            {
                if (((l >> i) << i) != l) Up(l >> i);
                if (((r >> i) << i) != r) Up((r - 1) >> i);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r)
        {
            if (l == r) return ti;
            l += n;
            r += n;
            for (var i = height; i >= 1; --i)
            {
                if (((l >> i) << i) != l) Down(l >> i);
                if (((r >> i) << i) != r) Down((r - 1) >> i);
            }
            var vl = ti; var vr = ti;
            for (long li = l, ri = r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(ref vl, ref dat[li++]);
                if ((ri & 1) == 1) vr = f(ref dat[--ri], ref vr);
            }
            return f(ref vl, ref vr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindToRight(int st, Func<T, bool> check, ref T acc, int k, int l, int r)
        {
            if (l + 1 == r)
            {
                acc = f(ref acc, ref dat[k]);
                return check(acc) ? k - n : sz;
            }
            Down(k);
            int m = (l + r) >> 1;
            if (m <= st) return FindToRight(st, check, ref acc, (k << 1) | 1, m, r);
            if (st <= l && !check(f(ref acc, ref dat[k])))
            {
                acc = f(ref acc, ref dat[k]);
                return sz;
            }
            int vl = FindToRight(st, check, ref acc, (k << 1) | 0, l, m);
            if (vl != sz) return vl;
            return FindToRight(st, check, ref acc, (k << 1) | 1, m, r);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindToRight(int st, Func<T, bool> check)
        {
            T acc = ti;
            return FindToRight(st, check, ref acc, 1, 0, n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindToLeft(int st, Func<T, bool> check, ref T acc, int k, int l, int r)
        {
            if (l + 1 == r)
            {
                acc = f(ref dat[k], ref acc);
                return check(acc) ? k - n : -1;
            }
            Down(k);
            int m = (l + r) >> 1;
            if (m > st) return FindToLeft(st, check, ref acc, (k << 1) | 0, l, m);
            if (st >= r - 1 && !check(f(ref dat[k], ref acc)))
            {
                acc = f(ref dat[k], ref acc);
                return -1;
            }
            int vr = FindToLeft(st, check, ref acc, (k << 1) | 1, m, r);
            if (vr != -1) return vr;
            return FindToLeft(st, check, ref acc, (k << 1) | 0, l, m);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindToLeft(int st, Func<T, bool> check)
        {
            T acc = ti;
            return FindToLeft(st, check, ref acc, 1, 0, n);
        }
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { Thrust(idx += n); return dat[idx]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Thrust(idx += n); dat[idx] = value; Recalc(idx); }
        }
    }
    class LIB_LazySegTree
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f1(ref long x, ref long y) => Math.Min(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f2(ref long x, ref long y, ref int c) => y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f3(ref long x, ref long y) => y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f4(ref long x, ref long y, ref int c) => x + y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f5(ref long x, ref long y) => x + y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f6(ref long x, ref long y) => Math.Max(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f7(ref long x, ref long y, ref int c) => y * c;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private long f8(ref long x, ref long y, ref int c) => x + y * c;
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeMin(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MaxValue, long.MinValue + 100, f1, f2, f3);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeMin(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MaxValue, 0, f1, f4, f5);
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeMax(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MinValue, long.MaxValue - 100, f6, f2, f3);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeMax(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MinValue, 0, f6, f4, f5);
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeSum(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, 0, long.MaxValue, f5, f7, f3);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeSum(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, 0, 0, f5, f8, f5);
    }
    ////end
}