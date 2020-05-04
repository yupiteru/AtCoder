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
    class LIB_SegTree2D<T>
    {
        int xsz;
        int ysz;
        T ti;
        Func<T, T, T> f;
        T[,] dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SegTree2D(long _xsz, long _ysz, T _ti, Func<T, T, T> _f)
        {
            xsz = 1; ysz = 1;
            while (xsz < _xsz) xsz <<= 1;
            while (ysz < _ysz) ysz <<= 1;
            ti = _ti;
            f = _f;
            dat = new T[xsz << 1, ysz << 1];
            for (var i = 0; i < xsz; ++i) for (var j = 0; j < ysz; ++j) dat[i, j] = ti;
            for (var j = 0; j < xsz; ++j) for (var i = ysz - 1; i > 0; i--) dat[j, i] = f(dat[j, (i << 1) | 0], dat[j, (i << 1) | 1]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long x, long y, T v)
        {
            dat[x += xsz, y += ysz] = v;
            Action<long, long> up = (xp, yp) => { while ((yp >>= 1) > 0) dat[xp, yp] = f(dat[xp, (yp << 1) | 0], dat[xp, (yp << 1) | 1]); };
            up(x, y);
            while ((x >>= 1) > 0)
            {
                dat[x, y] = f(dat[(x << 1) | 0, y], dat[(x << 1) | 1, y]);
                up(x, y);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long xl, long xr, long yl, long yr)
        {
            if (xl == xr || yl == yr) return ti;
            if (xr < xl || yr < yl) throw new Exception();
            var xvl = ti;
            var xvr = ti;
            for (long xli = xsz + xl, xri = xsz + xr; xli < xri; xli >>= 1, xri >>= 1)
            {
                if ((xli & 1) == 1)
                {
                    var yvl = ti;
                    var yvr = ti;
                    for (long yli = ysz + yl, yri = ysz + yr; yli < yri; yli >>= 1, yri >>= 1)
                    {
                        if ((yli & 1) == 1) yvl = f(yvl, dat[xli, yli++]);
                        if ((yri & 1) == 1) yvr = f(dat[xli, --yri], yvr);
                    }
                    xvl = f(xvl, f(yvl, yvr));
                    ++xli;
                }
                if ((xri & 1) == 1)
                {
                    --xri;
                    var yvl = ti;
                    var yvr = ti;
                    for (long yli = ysz + yl, yri = ysz + yr; yli < yri; yli >>= 1, yri >>= 1)
                    {
                        if ((yli & 1) == 1) yvl = f(yvl, dat[xri, yli++]);
                        if ((yri & 1) == 1) yvr = f(dat[xri, --yri], yvr);
                    }
                    xvr = f(f(yvl, yvr), xvr);
                }
            }
            return f(xvl, xvr);
        }
        public T this[long x, long y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return dat[x + xsz, y + ysz]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Update(x, y, value); }
        }
    }
    ////end
}