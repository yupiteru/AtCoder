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
    class LIB_LazySegTree
    {
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeMin(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MaxValue, long.MinValue + 100, Math.Min, (x, y, c) => y, (x, y) => y);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeMin(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MaxValue, 0, Math.Min, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeMax(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MinValue, long.MaxValue - 100, Math.Max, (x, y, c) => y, (x, y) => y);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeMax(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, long.MinValue, 0, Math.Max, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_LazySegTree<long, long> CreateRangeUpdateRangeSum(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, 0, long.MaxValue, (x, y) => x + y, (x, y, c) => y * c, (x, y) => y);
        static public LIB_LazySegTree<long, long> CreateRangeAddRangeSum(IEnumerable<long> init) => new LIB_LazySegTree<long, long>(init, 0, 0, (x, y) => x + y, (x, y, c) => x + y * c, (x, y) => x + y);
    }
    class LIB_LazySegTree<T, E> where E : IEquatable<E>
    {
        int n, height, sz;
        int[] rangeSz;
        T ti;
        E ei;
        Func<T, T, T> f;
        Func<T, E, int, T> g;
        Func<E, E, E> h;
        T[] dat;
        E[] laz;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(long _n, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, int, T> _g, Func<E, E, E> _h)
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
            ref T datref = ref dat[0];
            ref int rangeSzref = ref rangeSz[0];
            for (var i = 0; i < sz; i++) Unsafe.Add(ref rangeSzref, n + i) = 1;
            for (var i = n - 1; i > 0; i--)
            {
                Unsafe.Add(ref rangeSzref, i) = Unsafe.Add(ref rangeSzref, i << 1) + Unsafe.Add(ref rangeSzref, (i << 1) | 1);
                Unsafe.Add(ref datref, i) = f(Unsafe.Add(ref datref, i << 1), Unsafe.Add(ref datref, (i << 1) | 1));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(IEnumerable<T> l, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, int, T> _g, Func<E, E, E> _h) : this(l.Count(), _ti, _ei, _f, _g, _h)
        {
            var idx = 0;
            ref T datref = ref dat[0];
            foreach (var item in l) Unsafe.Add(ref datref, n + idx++) = item;
            for (var i = n - 1; i > 0; i--) Unsafe.Add(ref datref, i) = f(Unsafe.Add(ref datref, i << 1), Unsafe.Add(ref datref, (i << 1) | 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Apply(int i, ref T datref, ref E lazref, ref E v)
        {
            if (v.Equals(ei)) return;
            ref T dati = ref Unsafe.Add(ref datref, i);
            dati = g(dati, v, rangeSz[i]);
            if (i < n)
            {
                ref E lazi = ref Unsafe.Add(ref lazref, i);
                lazi = h(lazi, v);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Up(ref T datref, int i) => Unsafe.Add(ref datref, i) = f(Unsafe.Add(ref datref, i << 1), Unsafe.Add(ref datref, (i << 1) | 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Down(ref T datref, ref E lazref, int i)
        {
            ref E lazi = ref Unsafe.Add(ref lazref, i);
            if (lazi.Equals(ei)) return;
            var cl = i << 1;
            var cr = cl + 1;
            Apply(cl, ref datref, ref lazref, ref lazi);
            Apply(cr, ref datref, ref lazref, ref lazi);
            lazi = ei;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Thrust(int i)
        {
            ref T datref = ref dat[0];
            ref E lazref = ref laz[0];
            for (var j = height; j > 0; j--) Down(ref datref, ref lazref, i >> j);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Recalc(int i)
        {
            ref T datref = ref dat[0];
            while ((i >>= 1) > 0) Up(ref datref, i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long left, long right, E v)
        {
            var l = (int)left;
            var r = (int)right;
            if (l == r) return;
            ref T datref = ref dat[0];
            ref E lazref = ref laz[0];
            l += n;
            r += n;
            for (var i = height; i >= 1; --i)
            {
                if (((l >> i) << i) != l) Down(ref datref, ref lazref, l >> i);
                if (((r >> i) << i) != r) Down(ref datref, ref lazref, (r - 1) >> i);
            }
            for (int li = l, ri = r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) { Apply(li, ref datref, ref lazref, ref v); ++li; }
                if ((ri & 1) == 1) { --ri; Apply(ri, ref datref, ref lazref, ref v); }
            }
            for (var i = 1; i <= height; ++i)
            {
                if (((l >> i) << i) != l) Up(ref datref, l >> i);
                if (((r >> i) << i) != r) Up(ref datref, (r - 1) >> i);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long left, long right)
        {
            var l = (int)left;
            var r = (int)right;
            if (l == r) return ti;
            ref T datref = ref dat[0];
            ref E lazref = ref laz[0];
            l += n;
            r += n;
            for (var i = height; i >= 1; --i)
            {
                if (((l >> i) << i) != l) Down(ref datref, ref lazref, l >> i);
                if (((r >> i) << i) != r) Down(ref datref, ref lazref, (r - 1) >> i);
            }
            var vl = ti; var vr = ti;
            for (int li = l, ri = r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, Unsafe.Add(ref datref, li++));
                if ((ri & 1) == 1) vr = f(Unsafe.Add(ref datref, --ri), vr);
            }
            return f(vl, vr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindToRight(int st, Func<T, bool> check, ref T acc, int k, int l, int r)
        {
            if (l + 1 == r)
            {
                acc = f(acc, dat[k]);
                return check(acc) ? k - n : sz;
            }
            Down(ref dat[0], ref laz[0], k);
            int m = (l + r) >> 1;
            if (m <= st) return FindToRight(st, check, ref acc, (k << 1) | 1, m, r);
            if (st <= l && !check(f(acc, dat[k])))
            {
                acc = f(acc, dat[k]);
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
                acc = f(dat[k], acc);
                return check(acc) ? k - n : -1;
            }
            Down(ref dat[0], ref laz[0], k);
            int m = (l + r) >> 1;
            if (m > st) return FindToLeft(st, check, ref acc, (k << 1) | 0, l, m);
            if (st >= r - 1 && !check(f(dat[k], acc)))
            {
                acc = f(dat[k], acc);
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
        public class LazySegTreeOperator
        {
            E rangeOperator;
            T rangeValue;
            public E value => rangeOperator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator T(LazySegTreeOperator x) => x.rangeValue;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public LazySegTreeOperator operator *(LazySegTreeOperator lhs, E rhs)
            {
                lhs.rangeOperator = rhs;
                return lhs;
            }
            public LazySegTreeOperator(T rangeValue)
            {
                this.rangeValue = rangeValue;
            }
        }
        public LazySegTreeOperator this[long l, long r]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LazySegTreeOperator(Query(l, r + 1));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Update(l, r + 1, value.value);
        }
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { Thrust((int)(idx += n)); return dat[idx]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Thrust((int)(idx += n)); dat[idx] = value; Recalc((int)idx); }
        }
    }
    ////end
}