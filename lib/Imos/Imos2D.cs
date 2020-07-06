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
    class LIB_Imos2D
    {
        long[,] ary;
        long[,] build;
        long h, w;
        bool b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Imos2D(long h, long w)
        {
            this.h = h;
            this.w = w;
            ary = new long[h + 1, w + 1];
            b = false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(long h1, long w1, long h2, long w2, long v)
        {
            b = false;
            if (h1 < 0 || w1 < 0 || h2 >= h || w2 >= w) return;
            if (h2 <= h1 || w2 <= w1) return;
            ary[Max(0, h1), Max(0, w1)] += v;
            ary[Min(h, h2), Max(0, w1)] -= v;
            ary[Max(0, h1), Min(w, w2)] -= v;
            ary[Min(h, h2), Min(w, w2)] += v;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build()
        {
            b = true;
            build = new long[h + 1, w + 1];
            for (var i = 0; i <= h; i++) for (var j = 0; j <= w; j++) build[i, j] = ary[i, j];
            for (var i = 0; i <= h; i++) for (var j = 1; j <= w; j++) build[i, j] += build[i, j - 1];
            for (var i = 0; i <= w; i++) for (var j = 1; j <= h; j++) build[j, i] += build[j - 1, i];
        }
        public long this[long h, long w]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { if (!b) throw new Exception(); return build[h, w]; }
            private set { }
        }
    }
    ////end
}