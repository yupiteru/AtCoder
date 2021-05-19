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
        public static readonly ulong[] BitMask;
        static LIB_BitUtil()
        {
            BitMask = new ulong[64];
            BitMask[0] = 1;
            for (var i = 1; i < 64; i++) BitMask[i] = BitMask[i - 1] << 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public IEnumerable<long> ScanOne(long value)
        {
            for (; value > 0; value &= value - 1) yield return value & -value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int MSB(long value)
        {
            return 64 - (int)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount((ulong)value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int LSB(long value)
        {
            return 64 - (int)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount((ulong)(value & -value));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long PopCount(ulong value)
        {
            value = (value & 0x5555555555555555) + ((value >> 1) & 0x5555555555555555);
            value = (value & 0x3333333333333333) + ((value >> 2) & 0x3333333333333333);
            value = (value & 0x0f0f0f0f0f0f0f0f) + ((value >> 4) & 0x0f0f0f0f0f0f0f0f);
            value = (value & 0x00ff00ff00ff00ff) + ((value >> 8) & 0x00ff00ff00ff00ff);
            value = (value & 0x0000ffff0000ffff) + ((value >> 16) & 0x0000ffff0000ffff);
            value = (value & 0x00000000ffffffff) + ((value >> 32) & 0x00000000ffffffff);
            return (long)value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long PopCount(long value) => PopCount((ulong)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool IsSet(ulong value, int idx) => (value & BitMask[idx]) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool IsSet(long value, int idx) => IsSet((ulong)value, idx);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public T[] BitZentansaku<T>(long bitlen, T e, Func<int, T, T> f)
        {
            var retlen = 1 << (int)bitlen;
            var ret = new T[retlen];
            var idx = 0;
            ret[0] = e;
            for (var i = 1; i < retlen; ++i)
            {
                if (i == (1 << (idx + 1))) ++idx;
                ret[i] = f(idx, ret[i & (~(1 << idx))]);
            }
            return ret;
        }
    }
    ////end
}