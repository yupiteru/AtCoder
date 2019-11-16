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
        void Recalc(long i)
        {
            while ((i >>= 1) > 0) dat[i] = f(Reflect((i << 1) | 0), Reflect((i << 1) | 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, E v)
        {
            Thrust(l += n); Thrust(r += n - 1);
            for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) { laz[li] = h(laz[li], v); ++li; }
                if ((ri & 1) == 1) { --ri; laz[ri] = h(laz[ri], v); }
            }
            Recalc(l); Recalc(r);
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