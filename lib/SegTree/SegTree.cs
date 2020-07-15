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
    class LIB_SegTree<T>
    {
        int n, sz;
        T ti;
        Func<T, T, T> f;
        T[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree(long _n, T _ti, Func<T, T, T> _f)
        {
            n = 1; sz = (int)_n;
            while (n < _n) n <<= 1;
            ti = _ti;
            f = _f;
            dat = Enumerable.Repeat(ti, n << 1).ToArray();
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree(IEnumerable<T> l, T _ti, Func<T, T, T> _f) : this(l.Count(), _ti, _f)
        {
            var idx = 0;
            foreach (var item in l) dat[n + idx++] = item;
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long i, T v)
        {
            dat[i += n] = v;
            while ((i >>= 1) > 0) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r)
        {
            if (l == r) return ti;
            if (r < l) throw new Exception();
            var vl = ti;
            var vr = ti;
            for (long li = n + l, ri = n + r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, dat[li++]);
                if ((ri & 1) == 1) vr = f(dat[--ri], vr);
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
            get { return dat[idx + n]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Update(idx, value); }
        }
    }
    ////end
}