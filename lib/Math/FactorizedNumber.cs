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
    class LIB_FactorizedNumber : IEquatable<LIB_FactorizedNumber>
    {
        LIB_Dict<long, long> dat;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FactorizedNumber(long x)
        {
            dat = new LIB_Dict<long, long>();
            foreach (var item in LIB_Math.Factors(x).GroupBy(e => e).ToDictionary(e => e.Key, e => e.LongCount())) dat[item.Key] = item.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FactorizedNumber(LIB_FactorizedNumber x)
        {
            dat = new LIB_Dict<long, long>();
            foreach (var item in x.dat) dat[item.Key] = item.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_FactorizedNumber(long x) => new LIB_FactorizedNumber(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public explicit operator long(LIB_FactorizedNumber x) => x.dat.Aggregate(1L, (a, e) => a * LIB_Math.Pow(e.Key, e.Value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_Mod(LIB_FactorizedNumber x) => x.dat.Aggregate((LIB_Mod)1, (a, e) => a * LIB_Mod.Pow(e.Key, e.Value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mul(LIB_FactorizedNumber x) { foreach (var item in x.dat) dat[item.Key] += item.Value; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Div(LIB_FactorizedNumber x) { foreach (var item in x.dat) dat[item.Key] -= item.Value; }
        static public bool operator ==(LIB_FactorizedNumber x, LIB_FactorizedNumber y) => x.dat.All(e => e.Value == y.dat[e.Key]) && y.dat.All(e => e.Value == x.dat[e.Key]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator !=(LIB_FactorizedNumber x, LIB_FactorizedNumber y) => !(x == y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_FactorizedNumber x) => this == x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_FactorizedNumber)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => dat.Aggregate(0, (a, x) => a ^ x.Key.GetHashCode() ^ x.Value.GetHashCode());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pow(long y) { foreach (var item in dat.Keys) dat[item] *= y; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GCD(LIB_FactorizedNumber y)
        {
            var removeKeys = new HashSet<long>(dat.Keys);
            foreach (var item in y.dat)
            {
                if (removeKeys.Contains(item.Key))
                {
                    removeKeys.Remove(item.Key);
                    dat[item.Key] = Min(item.Value, dat[item.Key]);
                }
            }
            foreach (var item in removeKeys) dat.Remove(item);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LCM(LIB_FactorizedNumber y)
        {
            foreach (var item in y.dat) dat[item.Key] = Max(dat[item.Key], item.Value);
        }
    }
    ////end
}