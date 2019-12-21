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
            ary = new ulong[(n - 1) / 64 + 1];
        }
        public long Count => n;
        public long CountOne => ary.Sum(e => LIB_BitUtil.CountOne(e));
        public bool this[int idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return LIB_BitUtil.IsSet(ary[idx / 64], idx % 64); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value) ary[idx / 64] |= LIB_BitUtil.BitMask[idx % 64];
                else ary[idx / 64] &= ~LIB_BitUtil.BitMask[idx % 64];
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator &(LIB_Bitset x, LIB_Bitset y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i] & y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator |(LIB_Bitset x, LIB_Bitset y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i] | y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator ^(LIB_Bitset x, LIB_Bitset y)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = x.ary[i] ^ y.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator ~(LIB_Bitset x)
        {
            var ret = new LIB_Bitset(x.n);
            for (var i = 0; i < ret.ary.Length; i++) ret.ary[i] = ~x.ary[i];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator <<(LIB_Bitset x, int num)
        {
            var ret = new LIB_Bitset(x.n);
            var moveCnt = num / 64;
            var moveBit = num % 64;
            for (var i = ret.ary.Length - 1; i >= moveCnt; i--)
            {
                ret.ary[i] = x.ary[i - moveCnt] << moveBit;
                if (moveBit > 0 && i > moveCnt) ret.ary[i] |= x.ary[i - moveCnt - 1] >> (64 - moveBit);
            }
            ret.ary[ret.ary.Length - 1] &= ceil[ret.n % 64];
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Bitset operator >>(LIB_Bitset x, int num)
        {
            var ret = new LIB_Bitset(x.n);
            var moveCnt = num / 64;
            var moveBit = num % 64;
            var aryMax = ret.ary.Length - moveCnt;
            for (var i = 0; i < aryMax; i++)
            {
                ret.ary[i] = x.ary[i + moveCnt] >> moveBit;
                if (moveBit > 0 && i < aryMax - 1) ret.ary[i] |= x.ary[i + moveCnt + 1] << (64 - moveBit);
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator ==(LIB_Bitset x, LIB_Bitset y) => x.n == y.n && Enumerable.Range(0, x.ary.Length).All(e => x.ary[e] == y.ary[e]);
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