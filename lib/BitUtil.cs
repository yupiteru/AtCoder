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
    class LIB_BitUtil
    {
        static readonly ulong[] bitMask;
        static LIB_BitUtil()
        {
            bitMask = new ulong[64];
            bitMask[0] = 1;
            for (var i = 1; i < 64; i++) bitMask[i] = bitMask[i - 1] << 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<long> ScanOne(long value)
        {
            for (; value > 0; value &= value - 1) yield return value & -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long CountOne(long value)
        {
            value = (value & 0x5555555555555555) + ((value >> 1) & 0x5555555555555555);
            value = (value & 0x3333333333333333) + ((value >> 2) & 0x3333333333333333);
            value = (value & 0x0f0f0f0f0f0f0f0f) + ((value >> 4) & 0x0f0f0f0f0f0f0f0f);
            value = (value & 0x00ff00ff00ff00ff) + ((value >> 8) & 0x00ff00ff00ff00ff);
            value = (value & 0x0000ffff0000ffff) + ((value >> 16) & 0x0000ffff0000ffff);
            value = (value & 0x00000000ffffffff) + ((value >> 32) & 0x00000000ffffffff);
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool IsSet(ulong value, int idx) => (value & bitMask[idx]) != 0;
    }
    ////end
}