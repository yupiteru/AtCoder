using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;
using System.Threading;

namespace Program
{
    public static class DPF
    {
        static public void Solve()
        {
            var s = NS;
            var t = NS;
            var sl = s.Length;
            var tl = t.Length;

            var dp = new long[sl + 1, tl + 1];
            var dps = new long[sl + 1, tl + 1];
            var list = new Dict<long, Tuple<long, char>>();
            for (var i = 1; i <= sl; i++)
            {
                for (var j = 1; j <= tl; j++)
                {
                    dp[i, j] = dp[i - 1, j - 1];
                    dps[i, j] = dps[i - 1, j - 1];
                    if (s[i - 1] == t[j - 1])
                    {
                        dp[i, j] = dp[i, j] + 1;
                        list[i * 10000 + j] = Tuple.Create(dps[i, j], s[i - 1]);
                        dps[i, j] = i * 10000 + j;
                    }
                    if (dp[i, j] < dp[i - 1, j])
                    {
                        dp[i, j] = dp[i - 1, j];
                        dps[i, j] = dps[i - 1, j];
                    }
                    if (dp[i, j] < dp[i, j - 1])
                    {
                        dp[i, j] = dp[i, j - 1];
                        dps[i, j] = dps[i, j - 1];
                    }
                }
            }
            var ans = new List<char>();
            var start = dps[sl, tl];
            while (list[start] != null)
            {
                ans.Add(list[start].Item2);
                start = list[start].Item1;
            }
            ans.Reverse();
            Console.WriteLine(new string(ans.ToArray()));
        }

        static public void Main(string[] args) { if (args.Length == 0) { var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false }; Console.SetOut(sw); } var t = new Thread(Solve, 134217728); t.Start(); t.Join(); Console.Out.Flush(); }
        static Random rand = new Random();
        static class Console_
        {
            static Queue<string> param = new Queue<string>();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        }
        static long NN => long.Parse(Console_.NextString());
        static double ND => double.Parse(Console_.NextString());
        static string NS => Console_.NextString();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => x.OrderBy(_ => rand.Next());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void RevSort<T>(T[] l) where T : IComparable { Array.Sort(l, (x, y) => y.CompareTo(x)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void RevSort<T>(T[] l, Comparison<T> comp) where T : IComparable { Array.Sort(l, (x, y) => comp(y, x)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Primes(long x) { if (x < 2) yield break; yield return 2; var halfx = x / 2; var table = new bool[halfx + 1]; var max = (long)(Math.Sqrt(x) / 2); for (long i = 1; i <= max; ++i) { if (table[i]) continue; var add = 2 * i + 1; yield return add; for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true; } for (long i = max + 1; i <= halfx; ++i) if (!table[i] && 2 * i + 1 <= x) yield return 2 * i + 1; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Factors(long x) { if (x < 2) yield break; while (x % 2 == 0) { x /= 2; yield return 2; } var max = (long)Math.Sqrt(x); for (long i = 3; i <= max; i += 2) while (x % i == 0) { x /= i; yield return i; } if (x != 1) yield return x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<long> Divisor(long x) { if (x < 1) yield break; var max = (long)Math.Sqrt(x); for (long i = 1; i <= max; ++i) { if (x % i != 0) continue; yield return i; if (i != x / i) yield return x / i; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long GCD(long a, long b) { while (b > 0) { var tmp = b; b = a % b; a = tmp; } return a; }
        static long LCM(long a, long b) => a * b / GCD(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Mat<T> Pow<T>(Mat<T> x, long y) => Mat<T>.Pow(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Pow<T>(T x, long y) { T a = (dynamic)1; while (y != 0) { if ((y & 1) == 1) a *= (dynamic)x; x *= (dynamic)x; y >>= 1; } return a; }
        static List<long> _fact = new List<long>() { 1 };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void _B(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long Comb(long n, long k) { _B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long Perm(long n, long k) { _B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<TR> Lambda<TR>(Func<Func<TR>, TR> f) { Func<TR> t = () => default(TR); return t = () => f(t); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<T1, TR> Lambda<T1, TR>(Func<T1, Func<T1, TR>, TR> f) { Func<T1, TR> t = x1 => default(TR); return t = x1 => f(x1, t); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<T1, T2, TR> Lambda<T1, T2, TR>(Func<T1, T2, Func<T1, T2, TR>, TR> f) { Func<T1, T2, TR> t = (x1, x2) => default(TR); return t = (x1, x2) => f(x1, x2, t); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<T1, T2, T3, TR> Lambda<T1, T2, T3, TR>(Func<T1, T2, T3, Func<T1, T2, T3, TR>, TR> f) { Func<T1, T2, T3, TR> t = (x1, x2, x3) => default(TR); return t = (x1, x2, x3) => f(x1, x2, x3, t); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<T1, T2, T3, T4, TR> Lambda<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Func<T1, T2, T3, T4, TR>, TR> f) { Func<T1, T2, T3, T4, TR> t = (x1, x2, x3, x4) => default(TR); return t = (x1, x2, x3, x4) => f(x1, x2, x3, x4, t); }
        class PQ<T> where T : IComparable
        {
            List<T> h; Comparison<T> c; public T Peek => h[0]; public int Count => h.Count;
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Pop() { var r = h[0]; var v = h[h.Count - 1]; h.RemoveAt(h.Count - 1); if (h.Count == 0) return r; var i = 0; while (i * 2 + 1 < h.Count) { var i1 = i * 2 + 1; var i2 = i * 2 + 2; if (i2 < h.Count && c(h[i1], h[i2]) > 0) i1 = i2; if (c(v, h[i1]) <= 0) break; h[i] = h[i1]; i = i1; } h[i] = v; return r; }
        }
        class PQ<TK, TV> where TK : IComparable
        {
            PQ<Tuple<TK, TV>> q; public Tuple<TK, TV> Peek => q.Peek; public int Count => q.Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, Comparison<TK> c, bool asc = true) { q = new PQ<Tuple<TK, TV>>(cap, (x, y) => c(x.Item1, y.Item1), asc); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(Comparison<TK> c, bool asc = true) { q = new PQ<Tuple<TK, TV>>((x, y) => c(x.Item1, y.Item1), asc); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(TK k, TV v) => q.Push(Tuple.Create(k, v));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<TK, TV> Pop() => q.Pop();
        }
        public class UF
        {
            long[] d;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UF(long s) { d = Repeat(-1L, s).ToArray(); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Unite(long x, long y) { x = Root(x); y = Root(y); if (x != y) { if (d[y] < d[x]) { var t = y; y = x; x = t; } d[x] += d[y]; d[y] = x; } return x != y; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSame(long x, long y) => Root(x) == Root(y);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Root(long x) => d[x] < 0 ? x : d[x] = Root(d[x]);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Count(long x) => -d[Root(d[x])];
        }
        struct Mod : IEquatable<object>
        {
            static public long _mod = 1000000007; long _val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mod(long x) { if (x < _mod && x >= 0) _val = x; else if ((_val = x % _mod) < 0) _val += _mod; }
            static public implicit operator Mod(long x) => new Mod(x);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public implicit operator long(Mod x) => x._val;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator +(Mod x, Mod y) { var t = x._val + y._val; return t >= _mod ? new Mod { _val = t - _mod } : new Mod { _val = t }; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod operator -(Mod x, Mod y) { var t = x._val - y._val; return t < 0 ? new Mod { _val = t + _mod } : new Mod { _val = t }; }
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
            static List<Mod> _fact = new List<Mod>() { 1 };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void B(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Comb(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Perm(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)]; }
        }
        struct Mat<T>
        {
            T[,] m;
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
        class Tree
        {
            long N; int l; List<long>[] p; int[] d; long[][] pr; long r; Tuple<long, long, int>[] e; Tuple<long, long>[] b; bool lca; bool euler; bool bfs;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tree(List<long>[] p_, long r_) { N = p_.Length; p = p_; r = r_; lca = false; euler = false; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long>[] BFSRoot() { if (!bfs) { var nb = new List<Tuple<long, long>>(); var q = new Queue<long>(); var d = new bool[N]; nb.Add(Tuple.Create(r, -1L)); d[r] = true; q.Enqueue(r); while (q.Count > 0) { var w = q.Dequeue(); foreach (var i in p[w]) { if (d[i]) continue; d[i] = true; q.Enqueue(i); nb.Add(Tuple.Create(i, w)); } } b = nb.ToArray(); bfs = true; } return b; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long>[] BFSLeaf() => BFSRoot().Reverse().ToArray();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long, int>[] Euler() { if (!euler) { var ne = new List<Tuple<long, long, int>>(); var s = new Stack<Tuple<long, long>>(); var d = new bool[N]; d[r] = true; s.Push(Tuple.Create(r, -1L)); while (s.Count > 0) { var w = s.Peek(); var ad = true; foreach (var i in p[w.Item1]) { if (d[i]) continue; d[i] = true; ad = false; s.Push(Tuple.Create(i, w.Item1)); } if (!ad || p[w.Item1].Count == 1) ne.Add(Tuple.Create(w.Item1, w.Item2, 1)); if (ad) { s.Pop(); ne.Add(Tuple.Create(w.Item1, w.Item2, -1)); } } e = ne.ToArray(); euler = true; } return e; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long LCA(long u, long v) { if (!lca) { l = 0; while (N > (1 << l)) l++; d = new int[N]; pr = Repeat(0, l).Select(_ => new long[N]).ToArray(); d[r] = 0; pr[0][r] = -1; var q = new Stack<long>(); q.Push(r); while (q.Count > 0) { var w = q.Pop(); foreach (var i in p[w]) { if (i == pr[0][w]) continue; q.Push(i); d[i] = d[w] + 1; pr[0][i] = w; } } for (var k = 0; k + 1 < l; k++) for (var w = 0; w < N; w++) if (pr[k][w] < 0) pr[k + 1][w] = -1; else pr[k + 1][w] = pr[k][pr[k][w]]; lca = true; } if (d[u] > d[v]) { var t = u; u = v; v = t; } for (var k = 0; k < l; k++) if ((((d[v] - d[u]) >> k) & 1) != 0) v = pr[k][v]; if (u == v) return u; for (var k = l - 1; k >= 0; k--) if (pr[k][u] != pr[k][v]) { u = pr[k][u]; v = pr[k][v]; } return pr[0][u]; }
        }
        class BT<T> where T : IComparable
        {
            class Node { public Node l; public Node r; public T v; public bool b; }
            Comparison<T> c; Node r; bool ch; T lm;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BT(Comparison<T> _c) { c = _c; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BT() : this((x, y) => x.CompareTo(y)) { }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool R(Node n) => n != null && !n.b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool B(Node n) => n != null && n.b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node RtL(Node n) { Node m = n.r, t = m.l; m.l = n; n.r = t; return m; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node RtR(Node n) { Node m = n.l, t = m.r; m.r = n; n.l = t; return m; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node RtLR(Node n) { n.l = RtL(n.l); return RtR(n); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node RtRL(Node n) { n.r = RtR(n.r); return RtL(n); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T x) { r = A(r, x); r.b = true; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node A(Node n, T x) { if (n == null) { ch = true; return new Node() { v = x }; } if (c(x, n.v) < 0) n.l = A(n.l, x); else n.r = A(n.r, x); return Bl(n); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node Bl(Node n) { if (!ch) return n; if (!B(n)) return n; if (R(n.l) && R(n.l.l)) { n = RtR(n); n.l.b = true; } else if (R(n.l) && R(n.l.r)) { n = RtLR(n); n.l.b = true; } else if (R(n.r) && R(n.r.l)) { n = RtRL(n); n.r.b = true; } else if (R(n.r) && R(n.r.r)) { n = RtL(n); n.r.b = true; } else ch = false; return n; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(T x) { r = Rm(r, x); if (r != null) r.b = true; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node Rm(Node n, T x) { if (n == null) { ch = false; return n; } var r = c(x, n.v); if (r < 0) { n.l = Rm(n.l, x); return BlL(n); } if (r > 0) { n.r = Rm(n.r, x); return BlR(n); } if (n.l == null) { ch = n.b; return n.r; } n.l = RmM(n.l); n.v = lm; return BlL(n); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node RmM(Node n) { if (n.r != null) { n.r = RmM(n.r); return BlR(n); } lm = n.v; ch = n.b; return n.l; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node BlL(Node n) { if (!ch) return n; if (B(n.r) && R(n.r.l)) { var b = n.b; n = RtRL(n); n.b = b; n.l.b = true; ch = false; } else if (B(n.r) && R(n.r.r)) { var b = n.b; n = RtL(n); n.b = b; n.r.b = true; n.l.b = true; ch = false; } else if (B(n.r)) { ch = n.b; n.b = true; n.r.b = false; } else { n = RtL(n); n.b = true; n.l.b = false; n.l = BlL(n.l); ch = false; } return n; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Node BlR(Node n) { if (!ch) return n; if (B(n.l) && R(n.l.r)) { var b = n.b; n = RtLR(n); n.b = b; n.r.b = true; ch = false; } else if (B(n.l) && R(n.l.l)) { var b = n.b; n = RtR(n); n.b = b; n.l.b = true; n.r.b = true; ch = false; } else if (B(n.l)) { ch = n.b; n.b = true; n.l.b = false; } else { n = RtR(n); n.b = true; n.r.b = false; n.r = BlR(n.r); ch = false; } return n; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Have(T x) { var t = FindUpper(x); return t.Item1 && t.Item2.CompareTo(x) == 0; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<bool, T> FindUpper(T x, bool findSame = true) { var v = FU(r, x, findSame); return v == null ? Tuple.Create(false, default(T)) : v; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Tuple<bool, T> FU(Node n, T x, bool s) { if (n == null) return null; var r = c(x, n.v); if (r < 0) { var v = FU(n.l, x, s); return v == null ? Tuple.Create(true, n.v) : v; } if (r > 0 || !s && r == 0) return FU(n.r, x, s); return Tuple.Create(true, n.v); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<bool, T> FindLower(T x, bool findSame = true) { var v = FL(r, x, findSame); return v == null ? Tuple.Create(false, default(T)) : v; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Tuple<bool, T> FL(Node n, T x, bool s) { if (n == null) return null; var r = c(x, n.v); if (r < 0 || !s && r == 0) return FL(n.l, x, s); if (r > 0) { var v = FL(n.r, x, s); return v == null ? Tuple.Create(true, n.v) : v; } return Tuple.Create(true, n.v); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Min() { Node n = r, p = null; while (n != null) { p = n; n = n.l; } return p == null ? default(T) : p.v; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Max() { Node n = r, p = null; while (n != null) { p = n; n = n.r; } return p == null ? default(T) : p.v; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Any() => r != null;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CountSlow() => L(r).Count();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<T> List() => L(r);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            IEnumerable<T> L(Node n) { if (n == null) yield break; foreach (var i in L(n.l)) yield return i; yield return n.v; foreach (var i in L(n.r)) yield return i; }
        }
        class Dict<K, V> : Dictionary<K, V>
        {
            Func<K, V> d;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Dict(Func<K, V> _d) { d = _d; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Dict() : this(_ => default(V)) { }
            new public V this[K i] { get { V v; return TryGetValue(i, out v) ? v : base[i] = d(i); } set { base[i] = value; } }
        }
        class Deque<T>
        {
            T[] b; int o, c;
            public int Count;
            public T this[int i] { get { return b[gi(i)]; } set { b[gi(i)] = value; } }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Deque(int cap = 16) { b = new T[c = cap]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int gi(int i) { if (i >= c) throw new Exception(); var r = o + i; return r >= c ? r - c : r; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void PushFront(T x) { if (Count == c) e(); if (--o < 0) o += b.Length; b[o] = x; ++Count; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T PopFront() { if (Count-- == 0) throw new Exception(); var r = b[o++]; if (o >= c) o -= c; return r; }
            public T Front => b[o];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void PushBack(T x) { if (Count == c) e(); var i = o + Count++; b[i >= c ? i - c : i] = x; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T PopBack() { if (Count == 0) throw new Exception(); return b[gi(--Count)]; }
            public T Back => b[gi(Count - 1)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void e() { T[] nb = new T[c << 1]; if (o > c - Count) { var l = b.Length - o; Array.Copy(b, o, nb, 0, l); Array.Copy(b, 0, nb, l, Count - l); } else Array.Copy(b, o, nb, 0, Count); b = nb; o = 0; c <<= 1; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Insert(int i, T x) { if (i > Count) throw new Exception(); this.PushFront(x); for (int j = 0; j < i; j++) this[j] = this[j + 1]; this[i] = x; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T RemoveAt(int i) { if (i < 0 || i >= Count) throw new Exception(); var r = this[i]; for (int j = i; j > 0; j--) this[j] = this[j - 1]; this.PopFront(); return r; }
        }
    }
}
