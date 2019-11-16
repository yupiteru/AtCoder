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
        int n;
        T ti;
        Func<T, T, T> f;
        T[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree(long _n, T _ti, Func<T, T, T> _f)
        {
            n = 1;
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
            var vl = ti;
            var vr = ti;
            for (long li = n + l, ri = n + r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, dat[li++]);
                if ((ri & 1) == 1) vr = f(dat[--ri], vr);
            }
            return f(vl, vr);
        }
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return dat[idx + n]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Update(idx, value); }
        }
    }
    class LIB_SegTree
    {
        int n;
        long ti;
        Func<long, long, long> f;
        long[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree(long _n, long _ti, Func<long, long, long> _f)
        {
            n = 1;
            while (n < _n) n <<= 1;
            ti = _ti;
            f = _f;
            dat = Enumerable.Repeat(ti, n << 1).ToArray();
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree(IEnumerable<long> l, long _ti, Func<long, long, long> _f) : this(l.Count(), _ti, _f)
        {
            var idx = 0;
            foreach (var item in l) dat[n + idx++] = item;
            for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long i, long v)
        {
            dat[i += n] = v;
            while ((i >>= 1) > 0) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Query(long l, long r)
        {
            var vl = ti;
            var vr = ti;
            for (long li = n + l, ri = n + r; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) vl = f(vl, dat[li++]);
                if ((ri & 1) == 1) vr = f(dat[--ri], vr);
            }
            return f(vl, vr);
        }
        public long this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return dat[idx + n]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Update(idx, value); }
        }
    }
    ////end
}