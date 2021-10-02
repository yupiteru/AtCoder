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
    class LIB_DualSegTree<T> where T : IEquatable<T>
    {
        int n, height;
        T ti;
        Func<T, T, T> f;
        T[] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DualSegTree(long _n, T _ti, Func<T, T, T> _f)
        {
            n = 1; height = 0;
            while (n < _n) { n <<= 1; ++height; }
            ti = _ti;
            f = _f;
            dat = Enumerable.Repeat(ti, n << 1).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DualSegTree(IEnumerable<T> l, T _ti, Func<T, T, T> _f) : this(l.Count(), _ti, _f)
        {
            var idx = 0;
            foreach (var item in l) dat[n + idx++] = item;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Eval(long i)
        {
            if (dat[i].Equals(ti)) return;
            dat[(i << 1) | 0] = f(dat[(i << 1) | 0], dat[i]);
            dat[(i << 1) | 1] = f(dat[(i << 1) | 1], dat[i]);
            dat[i] = ti;
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
        public void Update(long l, long r, T v)
        {
            if (l == r) return;
            if (r < l) throw new Exception();
            Thrust(l += n, r += n - 1);
            for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1)
            {
                if ((li & 1) == 1) { dat[li] = f(dat[li], v); ++li; }
                if ((ri & 1) == 1) { --ri; dat[ri] = f(dat[ri], v); }
            }
        }
        public class LazySegTreeOperator
        {
            T rangeOperator;
            public T value => rangeOperator;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public LazySegTreeOperator operator *(LazySegTreeOperator lhs, T rhs)
            {
                lhs.rangeOperator = rhs;
                return lhs;
            }
        }
        public LazySegTreeOperator this[long l, long r]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LazySegTreeOperator();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Update(l, r + 1, value.value);
        }
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { Thrust(idx += n); return dat[idx]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Thrust(idx += n); dat[idx] = value; }
        }
    }
    ////end
}