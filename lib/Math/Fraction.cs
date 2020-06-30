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
    struct LIB_Fraction : IEquatable<LIB_Fraction>, IEquatable<long>, IComparable<LIB_Fraction>, IComparable<long>
    {
        public long Numerator { get; private set; }
        public long Denominator { get; private set; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Fraction(long nume = 0, long deno = 1)
        {
            Numerator = nume;
            Denominator = deno;
            Yakubun();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Yakubun()
        {
            if (Denominator < 0) { Numerator *= -1; Denominator *= -1; }
            var gcd = LIB_Math.GCD(Abs(Numerator), Denominator);
            if (gcd == 0) { Numerator = 0; Denominator = 1; }
            else { Numerator /= gcd; Denominator /= gcd; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_Fraction(long x) => new LIB_Fraction(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public explicit operator double(LIB_Fraction x) => (double)x.Numerator / x.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(LIB_Fraction x)
        {
            Numerator = Numerator * x.Denominator + x.Numerator * Denominator;
            Denominator *= x.Denominator;
            Yakubun();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sub(LIB_Fraction x)
        {
            Numerator = Numerator * x.Denominator - x.Numerator * Denominator;
            Denominator *= x.Denominator;
            Yakubun();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mul(LIB_Fraction x)
        {
            Numerator *= x.Numerator;
            Denominator *= x.Denominator;
            Yakubun();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Div(LIB_Fraction x)
        {
            Numerator *= x.Denominator;
            Denominator *= x.Numerator;
            Yakubun();
        }
        public LIB_Fraction Abs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LIB_Fraction(Abs(Numerator), Denominator);
            private set { }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Fraction operator +(LIB_Fraction x, LIB_Fraction y) { x.Add(y); return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Fraction operator -(LIB_Fraction x, LIB_Fraction y) { x.Sub(y); return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Fraction operator *(LIB_Fraction x, LIB_Fraction y) { x.Mul(y); return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Fraction operator /(LIB_Fraction x, LIB_Fraction y) { x.Div(y); return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator <(LIB_Fraction x, LIB_Fraction y) => x.Numerator * y.Denominator < y.Numerator * x.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator >(LIB_Fraction x, LIB_Fraction y) => x.Numerator * y.Denominator > y.Numerator * x.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator <=(LIB_Fraction x, LIB_Fraction y) => x.Numerator * y.Denominator <= y.Numerator * x.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator >=(LIB_Fraction x, LIB_Fraction y) => x.Numerator * y.Denominator >= y.Numerator * x.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator ==(LIB_Fraction x, LIB_Fraction y) => x.Numerator == y.Numerator && x.Denominator == y.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator !=(LIB_Fraction x, LIB_Fraction y) => x.Numerator != y.Numerator || x.Denominator != y.Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(LIB_Fraction x)
        {
            var left = Numerator * x.Denominator;
            var right = x.Numerator * Denominator;
            if (left < right) return -1;
            if (left > right) return 1;
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(long x)
        {
            var right = x * Denominator;
            if (Numerator < right) return -1;
            if (Numerator > right) return 1;
            return 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_Fraction x) => x.Numerator == Numerator && x.Denominator == Denominator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(long x) => Numerator == x && Denominator == 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_Fraction)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            var hash = 23L;
            hash = hash * 37 + Numerator;
            hash = hash * 37 + Denominator;
            return hash.GetHashCode();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";
    }
    ////end
}