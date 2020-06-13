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
    class LIB_Imos
    {
        long[] ary;
        long[] build;
        bool b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Imos(long n)
        {
            ary = new long[n + 1];
            b = false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, long v)
        {
            b = false;
            if (l >= r) return;
            if (r <= 0) return;
            if (l >= ary.Length) return;
            ary[Max(0, l)] += v;
            ary[Min(ary.Length - 1, r)] -= v;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build()
        {
            b = true;
            build = new long[ary.Length];
            build[0] = ary[0];
            for (var i = 1; i < ary.Length; i++) build[i] = ary[i] + build[i - 1];
        }
        public long this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { if (!b) throw new Exception(); return build[idx]; }
            private set { }
        }
    }
    ////end
}