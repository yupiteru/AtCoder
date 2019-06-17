using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;

namespace Program
{
    public static class ABC112D
    {
        static public void Solve()
        {
            var N = NextLong;
            var M = NextLong;
            foreach (var item in Divisor(M).OrderByDescending(e => e))
            {
                if (N * item <= M)
                {
                    Console.WriteLine(item);
                    break;
                }
            }
        }

        static public void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
                Console.SetOut(sw);
            }
            Solve();
            Console.Out.Flush();
        }
        static Random rand = new Random();
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static int NextInt => int.Parse(Console_.NextString());
        static long NextLong => long.Parse(Console_.NextString());
        static double NextDouble => double.Parse(Console_.NextString());
        static string NextString => Console_.NextString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<int> NextIntList(int N) => Enumerable.Repeat(0, N).Select(_ => NextInt).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<long> NextLongList(int N) => Enumerable.Repeat(0, N).Select(_ => NextLong).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<double> NextDoubleList(int N) => Enumerable.Repeat(0, N).Select(_ => NextDouble).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<string> NextStringList(int N) => Enumerable.Repeat(0, N).Select(_ => NextString).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => x.OrderBy(_ => rand.Next());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> Repeat<T>(T v, int n) => Enumerable.Repeat<T>(v, n);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> Sort<T>(List<T> l) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp); return tmp.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> Sort<T>(List<T> l, Comparison<T> comp) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, comp); return tmp.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> RevSort<T>(List<T> l) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => y.CompareTo(x)); return tmp.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> RevSort<T>(List<T> l, Comparison<T> comp) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => comp(y, x)); return tmp.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Prime(long x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (long)(Math.Sqrt(x) / 2);
            for (long i = 1; i <= max; ++i)
            {
                if (table[i]) continue;
                var add = 2 * i + 1;
                yield return add;
                for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true;
            }
            for (long i = max + 1; i <= halfx; ++i) if (!table[i] && 2 * i + 1 <= x) yield return 2 * i + 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i <= max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long GCD(long a, long b)
        {
            while (b > 0) { var tmp = b; b = a % b; a = tmp; }
            return a;
        }
        class PQ<T> where T : IComparable
        {
            private List<T> h;
            private Comparison<T> c;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, Comparison<T> c, bool asc = true) { h = new List<T>(cap); this.c = asc ? c : (x, y) => c(y, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(Comparison<T> c, bool asc = true) { h = new List<T>(); this.c = asc ? c : (x, y) => c(y, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(T v)
            {
                var i = h.Count;
                h.Add(v);
                while (i > 0)
                {
                    var ni = (i - 1) / 2;
                    if (c(v, h[ni]) >= 0) break;
                    h[i] = h[ni]; i = ni;
                }
                h[i] = v;
            }
            public T Peek => h[0];
            public int Count => h.Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Pop()
            {
                var r = h[0];
                var v = h[h.Count - 1];
                h.RemoveAt(h.Count - 1);
                if (h.Count == 0) return r;
                var i = 0;
                while (i * 2 + 1 < h.Count)
                {
                    var i1 = i * 2 + 1;
                    var i2 = i * 2 + 2;
                    if (i2 < h.Count && c(h[i1], h[i2]) > 0) i1 = i2;
                    if (c(v, h[i1]) <= 0) break;
                    h[i] = h[i1]; i = i1;
                }
                h[i] = v;
                return r;
            }
        }
        class PQ<TKey, TValue> where TKey : IComparable
        {
            private PQ<Tuple<TKey, TValue>> q;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, Comparison<TKey> c, bool asc = true) { q = new PQ<Tuple<TKey, TValue>>(cap, (x, y) => c(x.Item1, y.Item1), asc); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(Comparison<TKey> c, bool asc = true) { q = new PQ<Tuple<TKey, TValue>>((x, y) => c(x.Item1, y.Item1), asc); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(TKey k, TValue v) => q.Push(Tuple.Create(k, v));
            public Tuple<TKey, TValue> Peek => q.Peek;
            public int Count => q.Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<TKey, TValue> Pop() => q.Pop();
        }
        class Mod
        {
            static public long _mod = 1000000007;
            private long _val = 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mod() { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mod(long x) { if (x < _mod && x >= 0) _val = x; else if ((_val = x % _mod) < 0) _val += _mod; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator Mod(long x) => new Mod(x);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator long(Mod x) => x._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator +(Mod x, Mod y) { var t = x._val + y._val; return new Mod() { _val = t < _mod ? t : t - _mod }; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator +(Mod x, long y) => x._val + y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator +(long x, Mod y) => x + y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator -(Mod x, Mod y) { var t = x._val - y._val; return new Mod() { _val = t < _mod ? t : t - _mod }; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator -(Mod x, long y) => x._val - y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator -(long x, Mod y) => x - y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator /(Mod x, Mod y) => x._val * Inverse(y._val);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator ==(Mod x, Mod y) => x._val == y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator ==(Mod x, long y) => x._val == y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator ==(long x, Mod y) => x == y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator !=(Mod x, Mod y) => x._val != y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator !=(Mod x, long y) => x._val != y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator !=(long x, Mod y) => x != y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Pow(Mod x, long y)
            {
                Mod a = 1;
                while (y != 0)
                {
                    if ((y & 1) == 1) a *= x;
                    x *= x; y >>= 1;
                }
                return a;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public long Inverse(long x)
            {
                long b = _mod, r = 1, u = 0, t = 0;
                while (b > 0)
                {
                    var q = x / b;
                    t = u; u = r - q * u; r = t;
                    t = b; b = x - q * b; x = t;
                }
                return r < 0 ? r + _mod : r;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            override public bool Equals(object obj) => obj == null ? false : _val == ((Mod)obj)._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Mod obj) => obj == null ? false : _val == obj._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode() => _val.GetHashCode();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override string ToString() => _val.ToString();
            static private List<Mod> _fact = new List<Mod>();
            static private List<Mod> _ifact = new List<Mod>();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static private void Build(int n)
            {
                if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i)
                        if (i == 0L) { _fact.Add(1); _ifact.Add(1); }
                        else { _fact.Add(_fact[i - 1] * i); _ifact.Add(_ifact[i - 1] * Mod.Pow(i, _mod - 2)); }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Comb(int n, int k)
            {
                Build(n);
                if (n == 0 && k == 0) return 1;
                if (n < k || n < 0) return 0;
                return _ifact[n - k] * _ifact[k] * _fact[n];
            }
        }
        class Mat<T>
        {
            private T[,] m;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mat(T[,] v) { m = (T[,])v.Clone(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator Mat<T>(T[,] v) => new Mat<T>(v);
            public T this[int r, int c] { get { return m[r, c]; } set { m[r, c] = value; } }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator +(Mat<T> a, T x)
            {
                var tm = (T[,])a.m.Clone();
                for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)x;
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator +(Mat<T> a, Mat<T> b)
            {
                var tm = (T[,])a.m.Clone();
                for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)b[r, c];
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator -(Mat<T> a, T x)
            {
                var tm = (T[,])a.m.Clone();
                for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)x;
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator -(Mat<T> a, Mat<T> b)
            {
                var tm = (T[,])a.m.Clone();
                for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)b[r, c];
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator *(Mat<T> a, T x)
            {
                var tm = (T[,])a.m.Clone();
                for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] *= (dynamic)x;
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator *(Mat<T> a, Mat<T> b)
            {
                var nr = a.m.GetLength(0); var nc = b.m.GetLength(1);
                var tm = new T[nr, nc];
                for (int i = 0; i < nr; ++i) for (int j = 0; j < nc; ++j) tm[i, j] = (dynamic)0;
                for (int r = 0; r < nr; ++r) for (int c = 0; c < nc; ++c) for (int i = 0; i < a.m.GetLength(1); ++i) tm[r, c] += a[r, i] * (dynamic)b[i, c];
                return new Mat<T>(tm);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> Pow(Mat<T> x, long y)
            {
                var n = x.m.GetLength(0);
                var t = (Mat<T>)new T[n, n];
                for (int i = 0; i < n; ++i) for (int j = 0; j < n; ++j) t[i, j] = (dynamic)(i == j ? 1 : 0);
                while (y != 0)
                {
                    if ((y & 1) == 1) t *= x;
                    x *= x; y >>= 1;
                }
                return t;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Mat<T> Pow<T>(Mat<T> x, long y) => Mat<T>.Pow(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Mod Pow(Mod x, long y) => Mod.Pow(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long Pow(long x, long y)
        {
            long a = 1;
            while (y != 0)
            {
                if ((y & 1) == 1) a *= x;
                x *= x; y >>= 1;
            }
            return a;
        }
        static List<long> _fact = new List<long>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void _Build(int n)
        {
            if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i)
                    if (i == 0L) _fact.Add(1);
                    else _fact.Add(_fact[i - 1] * i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long Comb(int n, int k)
        {
            _Build(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[n] / _fact[k] / _fact[n - k];
        }
    }
}
