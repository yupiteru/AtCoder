using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;

namespace Program
{
    public static class ABC132B
    {
        static public void Solve()
        {
            var n = NN;
            var pList = NNList(n);
            var ans = 0;
            for (var i = 1; i < n - 1; i++)
            {
                var tmp = new[] { pList[i - 1], pList[i], pList[i + 1] };
                tmp = Sort(tmp.ToList()).ToArray();
                if (tmp[1] == pList[i])
                {
                    ans++;
                }
            }
            Console.WriteLine(ans);

        }

        static public void Main(string[] args)
        {
            if (args.Length == 0) { var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false }; Console.SetOut(sw); }
            Solve();
            Console.Out.Flush();
        }
        static Random rand = new Random();
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        }
        static long NN => long.Parse(Console_.NextString());
        static double ND => double.Parse(Console_.NextString());
        static string NS => Console_.NextString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<long> NNList(long N) => Repeat(0, N).Select(_ => NN).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<double> NDList(long N) => Repeat(0, N).Select(_ => ND).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<string> NSList(long N) => Repeat(0, N).Select(_ => NS).ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => x.OrderBy(_ => rand.Next());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> Sort<T>(List<T> l) where T : IComparable { var tmp = l.ToArray(); Array.Sort(tmp); return tmp.ToList(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> Sort<T>(List<T> l, Comparison<T> comp) where T : IComparable { var tmp = l.ToArray(); Array.Sort(tmp, comp); return tmp.ToList(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> RevSort<T>(List<T> l) where T : IComparable { var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => y.CompareTo(x)); return tmp.ToList(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static List<T> RevSort<T>(List<T> l, Comparison<T> comp) where T : IComparable { var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => comp(y, x)); return tmp.ToList(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Primes(long x) { if (x < 2) yield break; yield return 2; var halfx = x / 2; var table = new bool[halfx + 1]; var max = (long)(Math.Sqrt(x) / 2); for (long i = 1; i <= max; ++i) { if (table[i]) continue; var add = 2 * i + 1; yield return add; for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true; } for (long i = max + 1; i <= halfx; ++i) if (!table[i] && 2 * i + 1 <= x) yield return 2 * i + 1; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Factors(long x) { if (x < 2) yield break; while (x % 2 == 0) { x /= 2; yield return 2; } var max = (long)Math.Sqrt(x); for (long i = 3; i <= max; i += 2) while (x % i == 0) { x /= i; yield return i; } if (x != 1) yield return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Divisor(long x) { if (x < 1) yield break; var max = (long)Math.Sqrt(x); for (long i = 1; i <= max; ++i) { if (x % i != 0) continue; yield return i; if (i != x / i) yield return x / i; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long GCD(long a, long b) { while (b > 0) { var tmp = b; b = a % b; a = tmp; } return a; }
        static long LCM(long a, long b) => a * b / GCD(a, b);
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
            public void Push(T v) { var i = h.Count; h.Add(v); while (i > 0) { var ni = (i - 1) / 2; if (c(v, h[ni]) >= 0) break; h[i] = h[ni]; i = ni; } h[i] = v; }
            public T Peek => h[0];
            public int Count => h.Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Pop() { var r = h[0]; var v = h[h.Count - 1]; h.RemoveAt(h.Count - 1); if (h.Count == 0) return r; var i = 0; while (i * 2 + 1 < h.Count) { var i1 = i * 2 + 1; var i2 = i * 2 + 2; if (i2 < h.Count && c(h[i1], h[i2]) > 0) i1 = i2; if (c(v, h[i1]) <= 0) break; h[i] = h[i1]; i = i1; } h[i] = v; return r; }
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
        struct Mod : IEquatable<object>
        {
            static public long _mod = 1000000007;
            private long _val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mod(long x) { if (x < _mod && x >= 0) _val = x; else if ((_val = x % _mod) < 0) _val += _mod; }
            static public implicit operator Mod(long x) => new Mod(x);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator long(Mod x) => x._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator +(Mod x, Mod y) => x._val + y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator -(Mod x, Mod y) => x._val - y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator *(Mod x, Mod y) => x._val * y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator /(Mod x, Mod y) => x._val * Inverse(y._val);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator ==(Mod x, Mod y) => x._val == y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public bool operator !=(Mod x, Mod y) => x._val != y._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public long Inverse(long x) { long b = _mod, r = 1, u = 0, t = 0; while (b > 0) { var q = x / b; t = u; u = r - q * u; r = t; t = b; b = x - q * b; x = t; } return r < 0 ? r + _mod : r; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IEquatable<object>.Equals(object obj) => obj == null ? false : Equals((Mod)obj);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj) => obj == null ? false : Equals((Mod)obj);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Mod obj) => obj == null ? false : _val == obj._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode() => _val.GetHashCode();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override string ToString() => _val.ToString();
            static private List<Mod> _fact = new List<Mod>() { 1 };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static private void Build(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Comb(long n, long k) { Build(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
        }
        struct Mat<T>
        {
            private T[,] m;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mat(T[,] v) { m = (T[,])v.Clone(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator Mat<T>(T[,] v) => new Mat<T>(v);
            public T this[int r, int c] { get { return m[r, c]; } set { m[r, c] = value; } }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator +(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)x; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator +(Mat<T> a, Mat<T> b) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)b[r, c]; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator -(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)x; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator -(Mat<T> a, Mat<T> b) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)b[r, c]; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator *(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] *= (dynamic)x; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> operator *(Mat<T> a, Mat<T> b) { var nr = a.m.GetLength(0); var nc = b.m.GetLength(1); var tm = new T[nr, nc]; for (int i = 0; i < nr; ++i) for (int j = 0; j < nc; ++j) tm[i, j] = (dynamic)0; for (int r = 0; r < nr; ++r) for (int c = 0; c < nc; ++c) for (int i = 0; i < a.m.GetLength(1); ++i) tm[r, c] += a[r, i] * (dynamic)b[i, c]; return tm; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mat<T> Pow(Mat<T> x, long y) { var n = x.m.GetLength(0); var t = (Mat<T>)new T[n, n]; for (int i = 0; i < n; ++i) for (int j = 0; j < n; ++j) t[i, j] = (dynamic)(i == j ? 1 : 0); while (y != 0) { if ((y & 1) == 1) t *= x; x *= x; y >>= 1; } return t; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Mat<T> Pow<T>(Mat<T> x, long y) => Mat<T>.Pow(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Pow<T>(T x, long y) { T a = (dynamic)1; while (y != 0) { if ((y & 1) == 1) a *= (dynamic)x; x *= (dynamic)x; y >>= 1; } return a; }
        static List<long> _fact = new List<long>() { 1 };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void _Build(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long Comb(long n, long k) { _Build(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
    }
}
