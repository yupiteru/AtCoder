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
    class LIB_LazySegTree<T, E>
    {
        int n, height;
        T ti;
        E ei;
        Func<T, T, T> f;
        Func<T, E, T> g;
        Func<E, E, E> h;
        T[] dat;
        E[] laz;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(long _n, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, T> _g, Func<E, E, E> _h)
        {
            n = 1; height = 0;
            while (n < _n) { n <<= 1; ++height; }
            ti = _ti;
            ei = _ei;
            f = _f;
            g = _g;
            h = _h;
            dat = Enumerable.Repeat(ti, n << 1).ToArray();
            laz = Enumerable.Repeat(ei, n << 1).ToArray();
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_LazySegTree(IEnumerable<T> l, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, T> _g, Func<E, E, E> _h) : this(l.Count(), _ti, _ei, _f, _g, _h)
        {
            var idx = 0;
            foreach (var item in l) dat[n + idx++] = item;
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Reflect(long i) => laz[i].Equals(ei) ? dat[i] : g(dat[i], laz[i]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Eval(long i)
        {
            if (laz[i].Equals(ei)) return;
            laz[(i << 1) | 0] = h(laz[(i << 1) | 0], laz[i]);
            laz[(i << 1) | 1] = h(laz[(i << 1) | 1], laz[i]);
            dat[i] = Reflect(i);
            laz[i] = ei;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Thrust(long i)
        {
            for (var j = height; j > 0; j--) Eval(i >> j);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Thrust(long l, long r)
        {
            if (l == r) { Thrust(l); return; }
            var xor = l ^ r;
            var i = height;
            for (; (xor >> i) != 0; --i) Eval(l >> i);
            for (; i != 0; --i) { Eval(l >> i); Eval(r >> i); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Recalc(long i)
        {
            while ((i >>= 1) > 0) dat[i] = f(Reflect((i << 1) | 0), Reflect((i << 1) | 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Recalc(long l, long r)
        {
            var xor = l ^ r;
            while (xor > 1)
            {
                xor >>= 1; l >>= 1; r >>= 1;
                dat[l] = f(Reflect((l << 1) | 0), Reflect((l << 1) | 1));
                dat[r] = f(Reflect((r << 1) | 0), Reflect((r << 1) | 1));
            }
            while (l > 1)
            {
                l >>= 1;
                dat[l] = f(Reflect((l << 1) | 0), Reflect((l << 1) | 1));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, E v)
        {
            Thrust(l += n, r += n - 1);
            for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) { laz[li] = h(laz[li], v); ++li; }
                if ((ri & 1) == 1) { --ri; laz[ri] = h(laz[ri], v); }
            }
            Recalc(l, r);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r)
        {
            Thrust(l += n); Thrust(r += n - 1);
            var vl = ti; var vr = ti;
            for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, Reflect(li++));
                if ((ri & 1) == 1) vr = f(Reflect(--ri), vr);
            }
            return f(vl, vr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int FindToRight(int st, Func<T, bool> check, ref T acc, int k, int l, int r)
        {
            if (l + 1 == r)
            {
                acc = f(acc, Reflect(k));
                return check(acc) ? k - n : -1;
            }
            Eval(k);
            int m = (l + r) >> 1;
            if (m <= st) return FindToRight(st, check, ref acc, (k << 1) | 1, m, r);
            if (st <= l && !check(f(acc, dat[k])))
            {
                acc = f(acc, dat[k]);
                return -1;
            }
            int vl = FindToRight(st, check, ref acc, (k << 1) | 0, l, m);
            if (vl != -1) return vl;
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
                acc = f(Reflect(k), acc);
                return check(acc) ? k - n : -1;
            }
            Eval(k);
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
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { Thrust(idx += n); return Reflect(idx); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Thrust(idx += n); dat[idx] = value; laz[idx] = ei; Recalc(idx); }
        }
    }
    ////end
}