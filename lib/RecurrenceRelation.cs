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
    class LIB_RecurrenceRelation<T>
    {
        readonly int size;
        readonly int typeSize;
        T addE;
        T mulE;
        T[] init;
        T[] coeff;
        Func<T, T, T> mul;
        Func<T, T, T> add;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RecurrenceRelation(T mulE, T addE, T[] init, T[] coeff, Func<T, T, T> mul, Func<T, T, T> add)
        {
            typeSize = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            this.addE = addE;
            this.mulE = mulE;
            this.size = init.Length;
            this.init = init.ToArray();
            this.coeff = new T[size];
            for (var i = 0; i < size; ++i) this.coeff[i] = coeff[size - i - 1];
            this.mul = mul;
            this.add = add;
        }
        T[] Next(T[] c)
        {
            var ret = new T[size];
            ref var retref = ref ret[0];
            ref var coeffref = ref coeff[0];
            ref var cref = ref c[0];
            var lastc = Unsafe.Add(ref cref, size - 1);
            retref = mul(lastc, coeffref);
            for (var i = 1; i < size; ++i) Unsafe.Add(ref retref, i) = add(Unsafe.Add(ref cref, i - 1), mul(lastc, Unsafe.Add(ref coeffref, i)));
            return ret;
        }
        T[] Double(T[] c)
        {
            var tmp = new T[size];
            var ret = new T[size];
            ref var tmpref = ref tmp[0];
            ref var retref = ref ret[0];
            ref var cref = ref c[0];
            Unsafe.CopyBlock(ref Unsafe.As<T, byte>(ref tmpref), ref Unsafe.As<T, byte>(ref cref), (uint)(size * typeSize));
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
        public T a(long n)
        {
            if (n < size) return init[n];
            var ary = new T[size];
            var coe = new T[size];
            ref var initref = ref init[0];
            coe[1 % size] = mulE;
            var p = 62;
            while (((n >> --p) & 1) == 0) ;
            while (p-- > 0)
            {
                coe = Double(coe);
                if (((n >> p) & 1) == 1) coe = Next(coe);
            }
            ref var coeref = ref coe[0];
            var ans = addE;
            for (var i = 0; i < size; ++i) ans = add(ans, mul(Unsafe.Add(ref coeref, i), Unsafe.Add(ref initref, i)));
            return ans;
        }
    }
    ////end
}