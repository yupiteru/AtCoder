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
    class LIB_FactorizedNumber : LIB_Dict<long, long>, IEquatable<LIB_FactorizedNumber>
    {
        // LIB_Dict wo tukaimasu
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FactorizedNumber(long x)
        {
            foreach (var item in LIB_Math.Factors(x).GroupBy(e => e).ToDictionary(e => e.Key, e => e.LongCount())) this[item.Key] = item.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_FactorizedNumber(LIB_FactorizedNumber x)
        {
            foreach (var item in x) this[item.Key] = item.Value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_FactorizedNumber(long x) => new LIB_FactorizedNumber(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public explicit operator long(LIB_FactorizedNumber x) => x.Aggregate(1L, (a, e) => a * LIB_Math.Pow(e.Key, e.Value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_Mod1000000007(LIB_FactorizedNumber x) => x.Aggregate((LIB_Mod1000000007)1, (a, e) => a * LIB_Mod1000000007.Pow(e.Key, e.Value));
        static public implicit operator LIB_Mod998244353(LIB_FactorizedNumber x) => x.Aggregate((LIB_Mod998244353)1, (a, e) => a * LIB_Mod998244353.Pow(e.Key, e.Value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mul(LIB_FactorizedNumber x) { foreach (var item in x) this[item.Key] += item.Value; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Div(LIB_FactorizedNumber x) { foreach (var item in x) this[item.Key] -= item.Value; }
        static public bool operator ==(LIB_FactorizedNumber x, LIB_FactorizedNumber y) => x.All(e => e.Value == y[e.Key]) && y.All(e => e.Value == x[e.Key]);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator !=(LIB_FactorizedNumber x, LIB_FactorizedNumber y) => !(x == y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_FactorizedNumber x) => this == x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_FactorizedNumber)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.Aggregate(0, (a, x) => a ^ x.Key.GetHashCode() ^ x.Value.GetHashCode());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pow(long y) { foreach (var item in Keys) this[item] *= y; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GCD(LIB_FactorizedNumber y)
        {
            var removeKeys = new HashSet<long>(Keys);
            foreach (var item in y)
            {
                if (removeKeys.Contains(item.Key))
                {
                    removeKeys.Remove(item.Key);
                    this[item.Key] = Min(item.Value, this[item.Key]);
                }
            }
            foreach (var item in removeKeys) Remove(item);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LCM(LIB_FactorizedNumber y)
        {
            foreach (var item in y) this[item.Key] = Max(this[item.Key], item.Value);
        }
    }
    ////end
}