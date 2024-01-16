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
        static public LazySegTreeBeats_RangeChmaxChminAddRangeSum CreateRangeChmaxChminAddRangeSum(IEnumerable<long> init) => new LazySegTreeBeats_RangeChmaxChminAddRangeSum(init);
    }
    class LazySegTreeBeats_RangeChmaxChminAddRangeSum : LIB_LazySegTree<(long min, long max, long min2, long max2, long sum, int count, int minCount, int maxCount), (long chmax, long chmin, long bias)>
    {
        const long INF = long.MaxValue >> 2;
        const long NEGINF = long.MinValue >> 2;
        static readonly (long min, long max, long min2, long max2, long sum, int count, int minCount, int maxCount) zero = (INF, NEGINF, INF, NEGINF, 0, 0, 0, 0);
        public LazySegTreeBeats_RangeChmaxChminAddRangeSum(IEnumerable<long> init) : base(init.Select(e => (e, e, INF, NEGINF, e, 1, 1, 1)), zero, (NEGINF, INF, 0),
            (x, y) =>
            {
                var min = Min(x.min, y.min);
                var max = Max(x.max, y.max);
                var min2 = x.min == y.min ? Min(x.min2, y.min2) : x.min2 <= y.min ? x.min2 : y.min2 <= x.min ? y.min2 : Max(x.min, y.min);
                var max2 = x.max == y.max ? Max(x.max2, y.max2) : x.max2 >= y.max ? x.max2 : y.max2 >= x.max ? y.max2 : Min(x.max, y.max);
                var sum = x.sum + y.sum;
                var count = x.count + y.count;
                var minCount = (x.min <= y.min ? x.minCount : 0) + (y.min <= x.min ? y.minCount : 0);
                var maxCount = (x.max >= y.max ? x.maxCount : 0) + (y.max >= x.max ? y.maxCount : 0);
                return (min, max, min2, max2, sum, count, minCount, maxCount);
            },
            (x, y, c) =>
            {
                if (x.count == 0) return (false, zero);
                if (x.min == x.max || y.chmax == y.chmin || y.chmax >= x.max || y.chmin <= x.min)
                {
                    var num = Min(Max(x.min, y.chmax), y.chmin) + y.bias;
                    return (false, (num, num, INF, NEGINF, num * x.count, x.count, x.count, x.count));
                }
                if (x.min2 == x.max)
                {
                    var min = Max(x.min, y.chmax) + y.bias;
                    var max = Min(x.max, y.chmin) + y.bias;
                    var sum = min * x.minCount + max * x.maxCount;
                    return (false, (min, max, max, min, sum, x.count, x.minCount, x.maxCount));
                }
                if (y.chmax < x.min2 && y.chmin > x.max2)
                {
                    var nextMin = Max(x.min, y.chmax);
                    var nextMax = Min(x.max, y.chmin);
                    var sum = x.sum + (nextMin - x.min) * x.minCount - (x.max - nextMax) * x.maxCount + y.bias * x.count;
                    var min = nextMin + y.bias;
                    var max = nextMax + y.bias;
                    var min2 = x.min2 + y.bias;
                    var max2 = x.max2 + y.bias;
                    return (false, (min, max, min2, max2, sum, x.count, x.minCount, x.maxCount));
                }
                return (true, zero);
            },
            (x, y) =>
            {
                var chmax = Max(Min(x.chmax + x.bias, y.chmin), y.chmax) - x.bias;
                var chmin = Min(Max(x.chmin + x.bias, y.chmax), y.chmin) - x.bias;
                var bias = x.bias + y.bias;
                return (chmax, chmin, bias);
            })
        {
        }

        public void UpdateChmax(long left, long right, long v) => Update(left, right, (v, INF, 0));
        public void UpdateChmin(long left, long right, long v) => Update(left, right, (NEGINF, v, 0));
        public void UpdateAdd(long left, long right, long v) => Update(left, right, (NEGINF, INF, v));
    }
    class LIB_LazySegTree<T, E> where E : IEquatable<E>
    {
        int n, height, sz;
        int[] rangeSz;
        bool isBeats;
        T ti;
        E ei;
        Func<T, T, T> f;
        Func<T, E, int, T> g;
        Func<T, E, int, (bool, T)> gBeats;
        Func<E, E, E> h;
        T[] dat;
        E[] laz;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(long _n, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, int, T> _g, Func<E, E, E> _h)
        {
            isBeats = false;
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
        public LIB_LazySegTree(long _n, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, int, (bool, T)> _gBeats, Func<E, E, E> _h)
        {
            isBeats = true;
            n = 1; height = 0; sz = (int)_n;
            while (n < _n) { n <<= 1; ++height; }
            ti = _ti;
            ei = _ei;
            f = _f;
            gBeats = _gBeats;
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
        public LIB_LazySegTree(IEnumerable<T> l, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, int, (bool, T)> _gBeats, Func<E, E, E> _h) : this(l.Count(), _ti, _ei, _f, _gBeats, _h)
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
            if (isBeats)
            {
                var res = gBeats(dati, v, rangeSz[i]);
                dati = res.Item2;
                if (i < n)
                {
                    ref E lazi = ref Unsafe.Add(ref lazref, i);
                    lazi = h(lazi, v);
                    if (res.Item1)
                    {
                        Down(ref datref, ref lazref, i);
                        Up(ref datref, i);
                    }
                }
            }
            else
            {
                dati = g(dati, v, rangeSz[i]);
                if (i < n)
                {
                    ref E lazi = ref Unsafe.Add(ref lazref, i);
                    lazi = h(lazi, v);
                }
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