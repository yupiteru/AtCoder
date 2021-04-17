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
    class LIB_RecurrenceRelation
    {
        readonly int size;
        long[] init;
        long[] coeff;
        Func<long, long, long> mul;
        Func<long, long, long> add;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RecurrenceRelation(int[] init, int[] coeff, Func<long, long, long> mul, Func<long, long, long> add)
        {
            this.size = init.Length;
            this.init = init.Select(e => (long)e).ToArray();
            this.coeff = new long[size];
            for (var i = 0; i < size; ++i) this.coeff[i] = coeff[size - i - 1];
            this.mul = mul;
            this.add = add;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RecurrenceRelation(long[] init, long[] coeff, Func<long, long, long> mul, Func<long, long, long> add)
        {
            this.size = init.Length;
            this.init = init.ToArray();
            this.coeff = new long[size];
            for (var i = 0; i < size; ++i) this.coeff[i] = coeff[size - i - 1];
            this.mul = mul;
            this.add = add;
        }
        long[] Next(long[] c)
        {
            var ret = new long[size];
            ref var retref = ref ret[0];
            ref var coeffref = ref coeff[0];
            ref var cref = ref c[0];
            var lastc = Unsafe.Add(ref cref, size - 1);
            retref = mul(lastc, coeffref);
            for (var i = 1; i < size; ++i) Unsafe.Add(ref retref, i) = add(Unsafe.Add(ref cref, i - 1), mul(lastc, Unsafe.Add(ref coeffref, i)));
            return ret;
        }
        long[] Double(long[] c)
        {
            var tmp = new long[size];
            var ret = new long[size];
            ref var tmpref = ref tmp[0];
            ref var retref = ref ret[0];
            ref var cref = ref c[0];
            Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmpref), ref Unsafe.As<long, byte>(ref cref), (uint)(size << 3));
            for (var j = 0; j < size; ++j)
            {
                for (var i = 0; i < size; ++i)
                {
                    Unsafe.Add(ref retref, i) = add(Unsafe.Add(ref retref, i), mul(Unsafe.Add(ref cref, j), Unsafe.Add(ref tmpref, i)));
                }
                tmp = Next(tmp);
                tmpref = ref tmp[0];
            }
            return ret;
        }
        public long a(long n)
        {
            if (n < size) return init[n];
            var ary = new long[size];
            var coe = new long[size];
            ref var initref = ref init[0];
            coe[1] = 1;
            var p = 62;
            while (((n >> --p) & 1) == 0) ;
            while (p-- > 0)
            {
                coe = Double(coe);
                if (((n >> p) & 1) == 1) coe = Next(coe);
            }
            ref var coeref = ref coe[0];
            var ans = 0L;
            for (var i = 0; i < size; ++i) ans = add(ans, mul(Unsafe.Add(ref coeref, i), Unsafe.Add(ref initref, i)));
            return ans;
        }
    }
    ////end
}