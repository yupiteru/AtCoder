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
    class LIB_DualSegTree2D<T> where T : IEquatable<T>
    {
        int xsz;
        int ysz;
        int xheight;
        int yheight;
        T ti;
        Func<T, T, T> f;
        T[,] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DualSegTree2D(long _xsz, long _ysz, T _ti, Func<T, T, T> _f)
        {
            xsz = ysz = 1;
            xheight = yheight = 0;
            while (xsz < _xsz) { xsz <<= 1; ++xheight; }
            while (ysz < _ysz) { ysz <<= 1; ++yheight; }
            ti = _ti;
            f = _f;
            dat = new T[xsz << 1, ysz << 1];
            for (var i = 0; i < xsz; ++i) for (var j = 0; j < ysz; ++j) dat[i, j] = ti;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long xl, long xr, long yl, long yr, T v)
        {
            if (xl == xr || yl == yr) return;
            if (xr < xl || yr < yl) throw new Exception();
            for (long xli = xsz + xl, xri = xsz + xr; xli < xri; xli >>= 1, xri >>= 1)
            {
                if ((xli & 1) == 1)
                {
                    for (long yli = ysz + yl, yri = ysz + yr; yli < yri; yli >>= 1, yri >>= 1)
                    {
                        if ((yli & 1) == 1) { dat[xli, yli] = f(dat[xli, yli], v); ++yli; }
                        if ((yri & 1) == 1) { --yri; dat[xli, yri] = f(dat[xli, yri], v); }
                    }
                    ++xli;
                }
                if ((xri & 1) == 1)
                {
                    --xri;
                    for (long yli = ysz + yl, yri = ysz + yr; yli < yri; yli >>= 1, yri >>= 1)
                    {
                        if ((yli & 1) == 1) { dat[xri, yli] = f(dat[xri, yli], v); ++yli; }
                        if ((yri & 1) == 1) { --yri; dat[xri, yri] = f(dat[xri, yri], v); }
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EvalY(long x, long y)
        {
            if (dat[x, y].Equals(ti)) return;
            dat[x, (y << 1) | 0] = f(dat[x, (y << 1) | 0], dat[x, y]);
            dat[x, (y << 1) | 1] = f(dat[x, (y << 1) | 1], dat[x, y]);
            dat[x, y] = ti;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EvalX(long x, long y)
        {
            if (dat[x, y].Equals(ti)) return;
            dat[(x << 1) | 0, y] = f(dat[(x << 1) | 0, y], dat[x, y]);
            dat[(x << 1) | 1, y] = f(dat[(x << 1) | 1, y], dat[x, y]);
            dat[x, y] = ti;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Thrust(long x, long y)
        {
            for (var xj = xheight; xj > 0; xj--)
            {
                for (var yj = yheight; yj > 0; yj--) EvalY(x >> xj, y >> yj);
                EvalX(x >> xj, y);
            }
            for (var yj = yheight; yj > 0; yj--) EvalY(x, y >> yj);
        }
        public T this[long x, long y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { Thrust(x += xsz, y += ysz); return dat[x, y]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Thrust(x += xsz, y += ysz); dat[x, y] = value; }
        }
    }
    ////end
}