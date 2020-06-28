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
    partial struct LIB_Mod : IEquatable<LIB_Mod>, IEquatable<long>
    {
        static public long _mod = 1000000007; long v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Mod(long x)
        {
            if (x < _mod && x >= 0) v = x;
            else if ((v = x % _mod) < 0) v += _mod;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator LIB_Mod(long x) => new LIB_Mod(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public implicit operator long(LIB_Mod x) => x.v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(LIB_Mod x) { if ((v += x.v) >= _mod) v -= _mod; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sub(LIB_Mod x) { if ((v -= x.v) < 0) v += _mod; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mul(LIB_Mod x) => v = (v * x.v) % _mod;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Div(LIB_Mod x) => v = (v * Inverse(x.v)) % _mod;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod operator +(LIB_Mod x, LIB_Mod y) { var t = x.v + y.v; return t >= _mod ? new LIB_Mod { v = t - _mod } : new LIB_Mod { v = t }; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod operator -(LIB_Mod x, LIB_Mod y) { var t = x.v - y.v; return t < 0 ? new LIB_Mod { v = t + _mod } : new LIB_Mod { v = t }; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod operator *(LIB_Mod x, LIB_Mod y) => x.v * y.v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod operator /(LIB_Mod x, LIB_Mod y) => x.v * Inverse(y.v);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator ==(LIB_Mod x, LIB_Mod y) => x.v == y.v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public bool operator !=(LIB_Mod x, LIB_Mod y) => x.v != y.v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long Inverse(long x)
        {
            long b = _mod, r = 1, u = 0, t = 0;
            while (b > 0)
            {
                var q = x / b;
                t = u;
                u = r - q * u;
                r = t;
                t = b;
                b = x - q * b;
                x = t;
            }
            return r < 0 ? r + _mod : r;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(LIB_Mod x) => v == x.v;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(long x) => v == x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) => x == null ? false : Equals((LIB_Mod)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => v.GetHashCode();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => v.ToString();
        static List<LIB_Mod> _fact = new List<LIB_Mod>() { 1 };
        static long _factm = _mod;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void B(long n)
        {
            if (_factm != _mod) _fact = new List<LIB_Mod>() { 1 };
            if (n >= _fact.Count)
                for (int i = _fact.Count; i <= n; ++i)
                    _fact.Add(_fact[i - 1] * i);
            _factm = _mod;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod Comb(long n, long k)
        {
            B(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod CombOK(long n, long k)
        {
            LIB_Mod ret = 1;
            for (var i = 0; i < k; i++) ret *= n - i;
            for (var i = 1; i <= k; i++) ret /= i;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod Perm(long n, long k)
        {
            B(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[(int)n] / _fact[(int)(n - k)];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod Pow(LIB_Mod x, long y)
        {
            LIB_Mod a = 1;
            while (y != 0)
            {
                if ((y & 1) == 1) a.Mul(x);
                x.Mul(x);
                y >>= 1;
            }
            return a;
        }
    }
    ////end
}