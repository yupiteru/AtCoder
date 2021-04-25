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
    class LIB_Bitset : IEquatable<LIB_Bitset>
    {
        long n;
        ulong[] ary;
        static readonly ulong[] ceil;
        static LIB_Bitset()
        {
            ceil = new ulong[64];
            ceil[0] = 0xffffffffffffffff;
            ceil[1] = 1;
            for (var i = 2; i < 64; i++)
            {
                ceil[i] = (ceil[i - 1] << 1) | 1;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Bitset(long size)
        {
            if (size <= 0) throw new Exception();
            n = size;
            ary = new ulong[((n - 1) >> 6) + 1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Bitset(LIB_Bitset src)
        {
            n = src.n;
            ary = src.ary.ToArray();
        }
        public long Count => n;
        public long PopCount => ary.Sum(e => LIB_BitUtil.PopCount(e));
        public bool this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return LIB_BitUtil.IsSet(ary[idx >> 6], idx & 63); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value) ary[idx >> 6] |= LIB_BitUtil.BitMask[idx & 63];
                else ary[idx >> 6] &= ~LIB_BitUtil.BitMask[idx & 63];
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator &(LIB_Bitset x, LIB_Bitset y)
        {
            if (x.n < y.n) { var t = x; x = y; y = t; }
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            for (var i = 0; i < y.ary.Length; i++) ret.ary[i] &= y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator |(LIB_Bitset x, LIB_Bitset y)
        {
            if (x.n < y.n) { var t = x; x = y; y = t; }
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            for (var i = 0; i < y.ary.Length; i++) ret.ary[i] |= y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator ^(LIB_Bitset x, LIB_Bitset y)
        {
            if (x.n < y.n) { var t = x; x = y; y = t; }
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            for (var i = 0; i < y.ary.Length; i++) ret.ary[i] ^= y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator &(LIB_Bitset x, long y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            ret.ary[0] &= (ulong)y;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator |(LIB_Bitset x, long y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            ret.ary[0] |= (ulong)y;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator ^(LIB_Bitset x, long y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i];
            ret.ary[0] ^= (ulong)y;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flip()
        {
            for (var i = 0; i < ary.Length; i++) ary[i] = ~ary[i];
            ary[ary.Length - 1] &= ceil[n & 63];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator ~(LIB_Bitset x)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = ~x.ary[i];
            ret.ary[ret.ary.Length - 1] &= ceil[ret.n & 63];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftLeft(int num)
        {
            var moveCnt = num >> 6;
            var moveBit = num & 63;
            for (var i = ary.Length - 1; i >= moveCnt; i--)
            {
                ary[i] = ary[i - moveCnt] << moveBit;
                if (moveBit > 0 && i > moveCnt) ary[i] |= ary[i - moveCnt - 1] >> (64 - moveBit);
            }
            ary[ary.Length - 1] &= ceil[n & 63];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator <<(LIB_Bitset x, int num)
        {
            var ret = new LIB_Bitset(x.n);
            var moveCnt = num >> 6;
            var moveBit = num & 63;
            for (var i = ret.ary.Length - 1; i >= moveCnt; i--)
            {
                ret.ary[i] = x.ary[i - moveCnt] << moveBit;
                if (moveBit > 0 && i > moveCnt) ret.ary[i] |= x.ary[i - moveCnt - 1] >> (64 - moveBit);
            }
            ret.ary[ret.ary.Length - 1] &= ceil[ret.n & 63];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ShiftRight(int num)
        {
            var moveCnt = num >> 6;
            var moveBit = num & 63;
            var aryMax = ary.Length - moveCnt;
            for (var i = 0; i < aryMax; i++)
            {
                ary[i] = ary[i + moveCnt] >> moveBit;
                if (moveBit > 0 && i < aryMax - 1) ary[i] |= ary[i + moveCnt + 1] << (64 - moveBit);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator >>(LIB_Bitset x, int num)
        {
            var ret = new LIB_Bitset(x.n);
            var moveCnt = num >> 6;
            var moveBit = num & 63;
            var aryMax = ret.ary.Length - moveCnt;
            for (var i = 0; i < aryMax; i++)
            {
                ret.ary[i] = x.ary[i + moveCnt] >> moveBit;
                if (moveBit > 0 && i < aryMax - 1) ret.ary[i] |= x.ary[i + moveCnt + 1] << (64 - moveBit);
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator ==(LIB_Bitset x, LIB_Bitset y)
        {
            if (x.n < y.n) { var t = x; x = y; y = t; }
            var i = 0;
            for (; i < y.ary.Length; i++) if (x.ary[i] != y.ary[i]) return false;
            for (; i < x.ary.Length; i++) if (x.ary[i] != 0) return false;
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator !=(LIB_Bitset x, LIB_Bitset y) => !(x == y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_Bitset x) => x == this;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_Bitset)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            var t = ary.Aggregate((a, x) => a ^ x);
            return (int)(((t >> 32) ^ t) & 0x00000000ffffffff);
        }
    }
    ////end
}