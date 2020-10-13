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
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Update(long i, T v)
        {
            ref T datref = ref dat[0];
            Unsafe.Add(ref datref, (int)(i += n)) = v;
            while ((i >>= 1) > 0) Unsafe.Add(ref datref, (int)i) = f(Unsafe.Add(ref datref, (int)i << 1), Unsafe.Add(ref datref, (int)(i << 1) | 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public T Query(long l, long r)
        {
            if (l == r) return ti;
            var vl = ti;
            var vr = ti;
            ref T datref = ref dat[0];
            for (long li = n + l, ri = n + r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, Unsafe.Add(ref datref, (int)(li++)));
                if ((ri & 1) == 1) vr = f(Unsafe.Add(ref datref, (int)(--ri)), vr);
            }
            return f(vl, vr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public long FindToRight(long st, Func<T, bool> check)
        {
            if (st == sz) return sz;
            T acc = ti;
            st += n;
            ref T datref = ref dat[0];
            do
            {
                while ((st & 1) == 0) st >>= 1;
                var ch = f(acc, Unsafe.Add(ref datref, (int)st));
                if (check(ch))
                {
                    while (st < n)
                    {
                        ch = f(acc, Unsafe.Add(ref datref, (int)(st <<= 1)));
                        if (!check(ch))
                        {
                            acc = ch;
                            ++st;
                        }
                    }
                    return Min(st - n, sz);
                }
                acc = ch;
                ++st;
            } while ((st & -st) != st);
            return sz;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public long FindToLeft(long st, Func<T, bool> check)
        {
            if (st < 0) return -1;
            ++st;
            T acc = ti;
            st += n;
            ref T datref = ref dat[0];
            do
            {
                --st;
                while (st > 1 && (st & 1) == 1) st >>= 1;
                var ch = f(Unsafe.Add(ref datref, (int)st), acc);
                if (check(ch))
                {
                    while (st < n)
                    {
                        ch = f(Unsafe.Add(ref datref, (int)(st = (st << 1) | 1)), acc);
                        if (!check(ch))
                        {
                            acc = ch;
                            --st;
                        }
                    }
                    return st - n;
                }
                acc = ch;
            } while ((st & -st) != st);
            return -1;
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