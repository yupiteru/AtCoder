
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
    class LIB_RollingHash
    {
        const ulong MASK30 = (1UL << 30) - 1;
        const ulong MASK31 = (1UL << 31) - 1;
        const ulong MOD = (1UL << 61) - 1;
        const ulong POSITIVIZER = MOD * ((1UL << 3) - 1);
        static readonly uint baseNum;
        static readonly List<ulong> basePow;
        List<ulong> hash;
        static LIB_RollingHash()
        {
            baseNum = (uint)new Random().Next(10000000, int.MaxValue);
            basePow = new ulong[] { 1 }.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator long(LIB_RollingHash x) => (long)x.hash.Last();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RollingHash(string s = "") : this(s.Select(e => (long)e)) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RollingHash(IEnumerable<long> ary) { hash = new ulong[1].ToList(); foreach (var item in ary) Add(item); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(long val)
        {
            hash.Add(Mod(Mul(hash.Last(), baseNum) + (ulong)val));
            basePow.Add(Mod(Mul(basePow.Last(), baseNum)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetHash(int l, int r) => (long)Mod(hash[r] + POSITIVIZER - Mul(hash[l], basePow[r - l]));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong Mul(ulong l, ulong r)
        {
            var lu = l >> 31;
            var ld = l & MASK31;
            var ru = r >> 31;
            var rd = r & MASK31;
            var middleBit = ld * ru + lu * rd;
            return ((lu * ru) << 1) + ld * rd + ((middleBit & MASK30) << 31) + (middleBit >> 30);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong Mul(ulong l, uint r)
        {
            var rd = r & MASK31;
            var middleBit = (l >> 31) * rd;
            return (l & MASK31) * rd + ((middleBit & MASK30) << 31) + (middleBit >> 30);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong Mod(ulong val)
        {
            val = (val & MOD) + (val >> 61);
            return val < MOD ? val : val - MOD;
        }
    }
    ////end
}