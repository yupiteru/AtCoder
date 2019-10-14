using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
namespace Program
{
    public static class TDPCD
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var D = NN;
            var dfactors = Factors(D).GroupBy(e => e).ToDictionary(e => e.Key, e => e.Count());
            if (dfactors.Any(e => e.Key > 5))
            {
                Console.WriteLine(0);
                return;
            }
            var twoCnt = 0;
            var threeCnt = 0;
            var fiveCnt = 0;
            if (dfactors.ContainsKey(2)) twoCnt = dfactors[2];
            if (dfactors.ContainsKey(3)) threeCnt = dfactors[3];
            if (dfactors.ContainsKey(5)) fiveCnt = dfactors[5];
            var dp = new double[N + 1, twoCnt + 1, threeCnt + 1, fiveCnt + 1];
            dp[0, 0, 0, 0] = 1;
            for (var i = 0; i < N; i++)
            {
                for (var two = 0; two <= twoCnt; two++)
                {
                    for (var three = 0; three <= threeCnt; three++)
                    {
                        for (var five = 0; five <= fiveCnt; five++)
                        {
                            var prob = dp[i, two, three, five] / 6.0;
                            dp[i + 1, two, three, five] += prob;
                            dp[i + 1, Min(two + 1, twoCnt), Min(three, threeCnt), Min(five, fiveCnt)] += prob;
                            dp[i + 1, Min(two, twoCnt), Min(three + 1, threeCnt), Min(five, fiveCnt)] += prob;
                            dp[i + 1, Min(two + 2, twoCnt), Min(three, threeCnt), Min(five, fiveCnt)] += prob;
                            dp[i + 1, Min(two, twoCnt), Min(three, threeCnt), Min(five + 1, fiveCnt)] += prob;
                            dp[i + 1, Min(two + 1, twoCnt), Min(three + 1, threeCnt), Min(five, fiveCnt)] += prob;
                        }
                    }
                }
            }
            Console.WriteLine(dp[N, twoCnt, threeCnt, fiveCnt]);
        }
        static public void Main(string[] args) { if (args.Length == 0) { var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false }; Console.SetOut(sw); } var t = new Thread(Solve, 134217728); t.Start(); t.Join(); Console.Out.Flush(); }
        static class Console_
        {
            static Queue<string> param = new Queue<string>();
            public static string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        }
        static long NN => long.Parse(Console_.NextString());
        static double ND => double.Parse(Console_.NextString());
        static string NS => Console_.NextString();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        static IEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => x.OrderBy(_ => xorshift);
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        static void RevSort<T>(T[] l) where T : IComparable { Array.Sort(l, (x, y) => y.CompareTo(x)); }
        static void RevSort<T>(T[] l, Comparison<T> comp) where T : IComparable { Array.Sort(l, (x, y) => comp(y, x)); }
        static IEnumerable<long> Primes(long x) { if (x < 2) yield break; yield return 2; var halfx = x / 2; var table = new bool[halfx + 1]; var max = (long)(Math.Sqrt(x) / 2); for (long i = 1; i <= max; ++i) { if (table[i]) continue; var add = 2 * i + 1; yield return add; for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true; } for (long i = max + 1; i <= halfx; ++i) if (!table[i] && 2 * i + 1 <= x) yield return 2 * i + 1; }
        static IEnumerable<long> Factors(long x) { if (x < 2) yield break; while (x % 2 == 0) { x /= 2; yield return 2; } var max = (long)Math.Sqrt(x); for (long i = 3; i <= max; i += 2) while (x % i == 0) { x /= i; yield return i; } if (x != 1) yield return x; }
        static IEnumerable<long> Divisor(long x) { if (x < 1) yield break; var max = (long)Math.Sqrt(x); for (long i = 1; i <= max; ++i) { if (x % i != 0) continue; yield return i; if (i != x / i) yield return x / i; } }
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
        static long GCD(long a, long b) { while (b > 0) { var tmp = b; b = a % b; a = tmp; } return a; }
        static long LCM(long a, long b) => a * b / GCD(a, b);
        static Mat<T> Pow<T>(Mat<T> x, long y) => Mat<T>.Pow(x, y);
        static Mod Pow(Mod x, long y) { Mod a = 1; while (y != 0) { if ((y & 1) == 1) a.Mul(x); x.Mul(x); y >>= 1; } return a; }
        static long Pow(long x, long y) { long a = 1; while (y != 0) { if ((y & 1) == 1) a *= x; x *= x; y >>= 1; } return a; }
        static List<long> _fact = new List<long>() { 1 };
        static void _B(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
        static long Comb(long n, long k) { _B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
        static long Perm(long n, long k) { _B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)]; }
        static Func<TR> Lambda<TR>(Func<Func<TR>, TR> f) { Func<TR> t = () => default(TR); return t = () => f(t); }
        static Func<T1, TR> Lambda<T1, TR>(Func<T1, Func<T1, TR>, TR> f) { Func<T1, TR> t = x1 => default(TR); return t = x1 => f(x1, t); }
        static Func<T1, T2, TR> Lambda<T1, T2, TR>(Func<T1, T2, Func<T1, T2, TR>, TR> f) { Func<T1, T2, TR> t = (x1, x2) => default(TR); return t = (x1, x2) => f(x1, x2, t); }
        static Func<T1, T2, T3, TR> Lambda<T1, T2, T3, TR>(Func<T1, T2, T3, Func<T1, T2, T3, TR>, TR> f) { Func<T1, T2, T3, TR> t = (x1, x2, x3) => default(TR); return t = (x1, x2, x3) => f(x1, x2, x3, t); }
        static Func<T1, T2, T3, T4, TR> Lambda<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Func<T1, T2, T3, T4, TR>, TR> f) { Func<T1, T2, T3, T4, TR> t = (x1, x2, x3, x4) => default(TR); return t = (x1, x2, x3, x4) => f(x1, x2, x3, x4, t); }
        static List<T> LCS<T>(T[] s, T[] t) where T : IEquatable<T> { int sl = s.Length, tl = t.Length; var dp = new int[sl + 1, tl + 1]; for (var i = 0; i < sl; i++) for (var j = 0; j < tl; j++) dp[i + 1, j + 1] = s[i].Equals(t[j]) ? dp[i, j] + 1 : Max(dp[i + 1, j], dp[i, j + 1]); { var r = new List<T>(); int i = sl, j = tl; while (i > 0 && j > 0) if (s[--i].Equals(t[--j])) r.Add(s[i]); else if (dp[i, j + 1] > dp[i + 1, j]) ++j; else ++i; r.Reverse(); return r; } }
        static long LIS<T>(T[] array, bool strict) { var l = new List<T>(); foreach (var e in array) { var i = l.BinarySearch(e); if (i < 0) i = ~i; else if (!strict) ++i; if (i == l.Count) l.Add(e); else l[i] = e; } return l.Count; }
        class PQ<T> where T : IComparable
        {
            List<T> h; Comparison<T> c; public T Peek => h[0]; public int Count => h.Count;
            public PQ(int cap, Comparison<T> c, bool asc = true) { h = new List<T>(cap); this.c = asc ? c : (x, y) => c(y, x); }
            public PQ(Comparison<T> c, bool asc = true) { h = new List<T>(); this.c = asc ? c : (x, y) => c(y, x); }
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            public void Push(T v) { var i = h.Count; h.Add(v); while (i > 0) { var ni = (i - 1) / 2; if (c(v, h[ni]) >= 0) break; h[i] = h[ni]; i = ni; } h[i] = v; }
            public T Pop() { var r = h[0]; var v = h[h.Count - 1]; h.RemoveAt(h.Count - 1); if (h.Count == 0) return r; var i = 0; while (i * 2 + 1 < h.Count) { var i1 = i * 2 + 1; var i2 = i * 2 + 2; if (i2 < h.Count && c(h[i1], h[i2]) > 0) i1 = i2; if (c(v, h[i1]) <= 0) break; h[i] = h[i1]; i = i1; } h[i] = v; return r; }
        }
        class PQ<TK, TV> where TK : IComparable
        {
            PQ<Tuple<TK, TV>> q; public Tuple<TK, TV> Peek => q.Peek; public int Count => q.Count;
            public PQ(int cap, Comparison<TK> c, bool asc = true) { q = new PQ<Tuple<TK, TV>>(cap, (x, y) => c(x.Item1, y.Item1), asc); }
            public PQ(Comparison<TK> c, bool asc = true) { q = new PQ<Tuple<TK, TV>>((x, y) => c(x.Item1, y.Item1), asc); }
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            public void Push(TK k, TV v) => q.Push(Tuple.Create(k, v));
            public Tuple<TK, TV> Pop() => q.Pop();
        }
        public class UF
        {
            long[] d;
            public UF(long s) { d = Repeat(-1L, s).ToArray(); }
            public bool Unite(long x, long y) { x = Root(x); y = Root(y); if (x != y) { if (d[y] < d[x]) { var t = y; y = x; x = t; } d[x] += d[y]; d[y] = x; } return x != y; }
            public bool IsSame(long x, long y) => Root(x) == Root(y);
            public long Root(long x) => d[x] < 0 ? x : d[x] = Root(d[x]);
            public long Count(long x) => -d[Root(x)];
        }
        struct Mod : IEquatable<Mod>, IEquatable<long>
        {
            static public long _mod = 1000000007; long v;
            public Mod(long x) { if (x < _mod && x >= 0) v = x; else if ((v = x % _mod) < 0) v += _mod; }
            static public implicit operator Mod(long x) => new Mod(x);
            static public implicit operator long(Mod x) => x.v;
            public void Add(Mod x) { if ((v += x.v) >= _mod) v -= _mod; }
            public void Sub(Mod x) { if ((v -= x.v) < 0) v += _mod; }
            public void Mul(Mod x) => v = (v * x.v) % _mod;
            public void Div(Mod x) => v = (v * Inverse(x.v)) % _mod;
            static public Mod operator +(Mod x, Mod y) { var t = x.v + y.v; return t >= _mod ? new Mod { v = t - _mod } : new Mod { v = t }; }
            static public Mod operator -(Mod x, Mod y) { var t = x.v - y.v; return t < 0 ? new Mod { v = t + _mod } : new Mod { v = t }; }
            static public Mod operator *(Mod x, Mod y) => x.v * y.v;
            static public Mod operator /(Mod x, Mod y) => x.v * Inverse(y.v);
            static public bool operator ==(Mod x, Mod y) => x.v == y.v;
            static public bool operator !=(Mod x, Mod y) => x.v != y.v;
            static public long Inverse(long x) { long b = _mod, r = 1, u = 0, t = 0; while (b > 0) { var q = x / b; t = u; u = r - q * u; r = t; t = b; b = x - q * b; x = t; } return r < 0 ? r + _mod : r; }
            public bool Equals(Mod x) => v == x.v;
            public bool Equals(long x) => v == x;
            public override bool Equals(object x) => x == null ? false : Equals((Mod)x);
            public override int GetHashCode() => v.GetHashCode();
            public override string ToString() => v.ToString();
            static List<Mod> _fact = new List<Mod>() { 1 };
            static void B(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
            static public Mod Comb(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
            static public Mod Perm(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)]; }
        }
        struct Mat<T>
        {
            T[,] m;
            public Mat(T[,] v) { m = (T[,])v.Clone(); }
            static public implicit operator Mat<T>(T[,] v) => new Mat<T>(v);
            public T this[int r, int c] { get { return m[r, c]; } set { m[r, c] = value; } }
            static public Mat<T> operator +(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)x; return tm; }
            static public Mat<T> operator +(Mat<T> a, Mat<T> b) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] += (dynamic)b[r, c]; return tm; }
            static public Mat<T> operator -(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)x; return tm; }
            static public Mat<T> operator -(Mat<T> a, Mat<T> b) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] -= (dynamic)b[r, c]; return tm; }
            static public Mat<T> operator *(Mat<T> a, T x) { var tm = (T[,])a.m.Clone(); for (int r = 0; r < tm.GetLength(0); ++r) for (int c = 0; c < tm.GetLength(1); ++c) tm[r, c] *= (dynamic)x; return tm; }
            static public Mat<T> operator *(Mat<T> a, Mat<T> b) { var nr = a.m.GetLength(0); var nc = b.m.GetLength(1); var tm = new T[nr, nc]; for (int i = 0; i < nr; ++i) for (int j = 0; j < nc; ++j) tm[i, j] = (dynamic)0; for (int r = 0; r < nr; ++r) for (int c = 0; c < nc; ++c) for (int i = 0; i < a.m.GetLength(1); ++i) tm[r, c] += a[r, i] * (dynamic)b[i, c]; return tm; }
            static public Mat<T> Pow(Mat<T> x, long y) { var n = x.m.GetLength(0); var t = (Mat<T>)new T[n, n]; for (int i = 0; i < n; ++i) for (int j = 0; j < n; ++j) t[i, j] = (dynamic)(i == j ? 1 : 0); while (y != 0) { if ((y & 1) == 1) t *= x; x *= x; y >>= 1; } return t; }
        }
        class Tree
        {
            long N; int l; List<long>[] p; int[] d; long[][] pr; long r; Tuple<long, long, int>[] e; Tuple<long, long>[] b; bool lca; bool euler; bool bfs;
            public Tree(List<long>[] p_, long r_) { N = p_.Length; p = p_; r = r_; lca = false; euler = false; }
            public Tuple<long, long>[] BFSRoot() { if (!bfs) { var nb = new List<Tuple<long, long>>(); var q = new Queue<long>(); var d = new bool[N]; nb.Add(Tuple.Create(r, -1L)); d[r] = true; q.Enqueue(r); while (q.Count > 0) { var w = q.Dequeue(); foreach (var i in p[w]) { if (d[i]) continue; d[i] = true; q.Enqueue(i); nb.Add(Tuple.Create(i, w)); } } b = nb.ToArray(); bfs = true; } return b; }
            public Tuple<long, long>[] BFSLeaf() => BFSRoot().Reverse().ToArray();
            public Tuple<long, long, int>[] Euler() { if (!euler) { var ne = new List<Tuple<long, long, int>>(); var s = new Stack<Tuple<long, long>>(); var d = new bool[N]; d[r] = true; s.Push(Tuple.Create(r, -1L)); while (s.Count > 0) { var w = s.Peek(); var ad = true; foreach (var i in p[w.Item1]) { if (d[i]) continue; d[i] = true; ad = false; s.Push(Tuple.Create(i, w.Item1)); } if (!ad || p[w.Item1].Count == 1) ne.Add(Tuple.Create(w.Item1, w.Item2, 1)); if (ad) { s.Pop(); ne.Add(Tuple.Create(w.Item1, w.Item2, -1)); } } e = ne.ToArray(); euler = true; } return e; }
            public long LCA(long u, long v) { if (!lca) { l = 0; while (N > (1 << l)) l++; d = new int[N]; pr = Repeat(0, l).Select(_ => new long[N]).ToArray(); d[r] = 0; pr[0][r] = -1; var q = new Stack<long>(); q.Push(r); while (q.Count > 0) { var w = q.Pop(); foreach (var i in p[w]) { if (i == pr[0][w]) continue; q.Push(i); d[i] = d[w] + 1; pr[0][i] = w; } } for (var k = 0; k + 1 < l; k++) for (var w = 0; w < N; w++) if (pr[k][w] < 0) pr[k + 1][w] = -1; else pr[k + 1][w] = pr[k][pr[k][w]]; lca = true; } if (d[u] > d[v]) { var t = u; u = v; v = t; } for (var k = 0; k < l; k++) if ((((d[v] - d[u]) >> k) & 1) != 0) v = pr[k][v]; if (u == v) return u; for (var k = l - 1; k >= 0; k--) if (pr[k][u] != pr[k][v]) { u = pr[k][u]; v = pr[k][v]; } return pr[0][u]; }
        }
        class Graph
        {
            int n; List<Tuple<int, int, long>> pathList; Dictionary<int, long>[] vtxPath; long INF = (long.MaxValue >> 1) - 1;
            public Graph(long _n)
            {
                n = (int)_n;
                pathList = new List<Tuple<int, int, long>>();
                vtxPath = Repeat(0, n).Select(_ => new Dictionary<int, long>()).ToArray();
            }
            public void AddPath(long a, long b, long c)
            {
                pathList.Add(Tuple.Create((int)a, (int)b, c));
                vtxPath[a][(int)b] = vtxPath[a].ContainsKey((int)b) ? Min(vtxPath[a][(int)b], c) : c;
            }
            public long[,] WarshallFloyd()
            {
                var ret = new long[n, n];
                for (var i = 0; i < n; i++)
                    for (var j = 0; j < n; j++)
                        ret[i, j] = vtxPath[i].ContainsKey(j) ? vtxPath[i][j] : INF;
                for (var k = 0; k < n; k++)
                    for (var i = 0; i < n; i++)
                        for (var j = 0; j < n; j++)
                            ret[i, j] = Min(ret[i, j], ret[i, k] + ret[k, j]);
                return ret;
            }
            public Tuple<long[], int?[], bool[]> BellmanFord(long s)
            {
                var dist = Repeat(INF, n).ToArray();
                var pred = new int?[n];
                var neg = new bool[n];
                dist[s] = 0;
                for (var i = 1; i < n; i++)
                    foreach (var path in pathList)
                        if (dist[path.Item2] > (dist[path.Item1] == INF ? INF : dist[path.Item1] + path.Item3))
                        {
                            dist[path.Item2] = dist[path.Item1] + path.Item3;
                            pred[path.Item2] = path.Item1;
                        }
                for (var i = 0; i < n; i++)
                    foreach (var path in pathList)
                        if (dist[path.Item2] > (dist[path.Item1] == INF ? INF : dist[path.Item1] + path.Item3) || neg[path.Item1])
                        {
                            dist[path.Item2] = dist[path.Item1] + path.Item3;
                            neg[path.Item2] = true;
                        }
                return Tuple.Create(dist, pred, neg);
            }
            public Tuple<long[], int?[]> Dijkstra(long s)
            {
                var dist = Repeat(long.MaxValue >> 2, n).ToArray();
                var pred = new int?[n];
                dist[s] = 0;
                var q = new PQ<long, int>();
                q.Push(0, (int)s);
                while (q.Count > 0)
                {
                    var u = q.Pop().Item2;
                    foreach (var path in vtxPath[u])
                    {
                        var v = path.Key;
                        var alt = dist[u] + path.Value;
                        if (dist[v] > alt)
                        {
                            dist[v] = alt;
                            pred[v] = u;
                            q.Push(alt, v);
                        }
                    }
                }
                return Tuple.Create(dist, pred);
            }
        }
        class BT<T> where T : IComparable
        {
            class Node { public Node l; public Node r; public T v; public bool b; public int c; }
            Comparison<T> c; Node r; bool ch; T lm;
            public BT(Comparison<T> _c) { c = _c; }
            public BT() : this((x, y) => x.CompareTo(y)) { }
            bool R(Node n) => n != null && !n.b;
            bool B(Node n) => n != null && n.b;
            long C(Node n) => n?.c ?? 0;
            Node RtL(Node n) { Node m = n.r, t = m.l; m.l = n; n.r = t; n.c -= m.c - (t?.c ?? 0); m.c += n.c - (t?.c ?? 0); return m; }
            Node RtR(Node n) { Node m = n.l, t = m.r; m.r = n; n.l = t; n.c -= m.c - (t?.c ?? 0); m.c += n.c - (t?.c ?? 0); return m; }
            Node RtLR(Node n) { n.l = RtL(n.l); return RtR(n); }
            Node RtRL(Node n) { n.r = RtR(n.r); return RtL(n); }
            public void Add(T x) { r = A(r, x); r.b = true; }
            Node A(Node n, T x) { if (n == null) { ch = true; return new Node() { v = x, c = 1 }; } if (c(x, n.v) < 0) n.l = A(n.l, x); else n.r = A(n.r, x); n.c++; return Bl(n); }
            Node Bl(Node n) { if (!ch) return n; if (!B(n)) return n; if (R(n.l) && R(n.l.l)) { n = RtR(n); n.l.b = true; } else if (R(n.l) && R(n.l.r)) { n = RtLR(n); n.l.b = true; } else if (R(n.r) && R(n.r.l)) { n = RtRL(n); n.r.b = true; } else if (R(n.r) && R(n.r.r)) { n = RtL(n); n.r.b = true; } else ch = false; return n; }
            public void Remove(T x) { r = Rm(r, x); if (r != null) r.b = true; }
            Node Rm(Node n, T x) { if (n == null) { ch = false; return n; } n.c--; var r = c(x, n.v); if (r < 0) { n.l = Rm(n.l, x); return BlL(n); } if (r > 0) { n.r = Rm(n.r, x); return BlR(n); } if (n.l == null) { ch = n.b; return n.r; } n.l = RmM(n.l); n.v = lm; return BlL(n); }
            Node RmM(Node n) { n.c--; if (n.r != null) { n.r = RmM(n.r); return BlR(n); } lm = n.v; ch = n.b; return n.l; }
            Node BlL(Node n) { if (!ch) return n; if (B(n.r) && R(n.r.l)) { var b = n.b; n = RtRL(n); n.b = b; n.l.b = true; ch = false; } else if (B(n.r) && R(n.r.r)) { var b = n.b; n = RtL(n); n.b = b; n.r.b = true; n.l.b = true; ch = false; } else if (B(n.r)) { ch = n.b; n.b = true; n.r.b = false; } else { n = RtL(n); n.b = true; n.l.b = false; n.l = BlL(n.l); ch = false; } return n; }
            Node BlR(Node n) { if (!ch) return n; if (B(n.l) && R(n.l.r)) { var b = n.b; n = RtLR(n); n.b = b; n.r.b = true; ch = false; } else if (B(n.l) && R(n.l.l)) { var b = n.b; n = RtR(n); n.b = b; n.l.b = true; n.r.b = true; ch = false; } else if (B(n.l)) { ch = n.b; n.b = true; n.l.b = false; } else { n = RtR(n); n.b = true; n.r.b = false; n.r = BlR(n.r); ch = false; } return n; }
            public T this[long i] { get { return At(r, i); } }
            T At(Node n, long i) { if (n == null) return default(T); if (n.l == null) if (i == 0) return n.v; else return At(n.r, i - 1); if (n.l.c == i) return n.v; if (n.l.c > i) return At(n.l, i); return At(n.r, i - n.l.c - 1); }
            public bool Have(T x) { var t = FindUpper(x); return t < C(r) && At(r, t).CompareTo(x) == 0; }
            public long FindUpper(T x, bool allowSame = true) => allowSame ? FL(r, x) + 1 : FU(r, x);
            long FU(Node n, T x) { if (n == null) return 0; var r = c(x, n.v); if (r < 0) return FU(n.l, x); return C(n.l) + 1 + FU(n.r, x); }
            public long FindLower(T x, bool allowSame = true) { var t = FL(r, x); if (allowSame) return t + 1 < C(r) && At(r, t + 1).CompareTo(x) == 0 ? t + 1 : t < 0 ? C(r) : t; return t < 0 ? C(r) : t; }
            long FL(Node n, T x) { if (n == null) return -1; var r = c(x, n.v); if (r > 0) return C(n.l) + 1 + FL(n.r, x); return FL(n.l, x); }
            public T Min() { Node n = r, p = null; while (n != null) { p = n; n = n.l; } return p == null ? default(T) : p.v; }
            public T Max() { Node n = r, p = null; while (n != null) { p = n; n = n.r; } return p == null ? default(T) : p.v; }
            public bool Any() => r != null;
            public long Count() => C(r);
            public IEnumerable<T> List() => L(r);
            IEnumerable<T> L(Node n) { if (n == null) yield break; foreach (var i in L(n.l)) yield return i; yield return n.v; foreach (var i in L(n.r)) yield return i; }
        }
        class Dict<K, V> : Dictionary<K, V>
        {
            Func<K, V> d;
            public Dict(Func<K, V> _d) { d = _d; }
            public Dict() : this(_ => default(V)) { }
            new public V this[K i] { get { V v; return TryGetValue(i, out v) ? v : base[i] = d(i); } set { base[i] = value; } }
        }
        class Deque<T>
        {
            T[] b; int o, c;
            public int Count;
            public T this[int i] { get { return b[gi(i)]; } set { b[gi(i)] = value; } }
            public Deque(int cap = 16) { b = new T[c = cap]; }
            int gi(int i) { if (i >= c) throw new Exception(); var r = o + i; return r >= c ? r - c : r; }
            public void PushFront(T x) { if (Count == c) e(); if (--o < 0) o += b.Length; b[o] = x; ++Count; }
            public T PopFront() { if (Count-- == 0) throw new Exception(); var r = b[o++]; if (o >= c) o -= c; return r; }
            public T Front => b[o];
            public void PushBack(T x) { if (Count == c) e(); var i = o + Count++; b[i >= c ? i - c : i] = x; }
            public T PopBack() { if (Count == 0) throw new Exception(); return b[gi(--Count)]; }
            public T Back => b[gi(Count - 1)];
            void e() { T[] nb = new T[c << 1]; if (o > c - Count) { var l = b.Length - o; Array.Copy(b, o, nb, 0, l); Array.Copy(b, 0, nb, l, Count - l); } else Array.Copy(b, o, nb, 0, Count); b = nb; o = 0; c <<= 1; }
            public void Insert(int i, T x) { if (i > Count) throw new Exception(); this.PushFront(x); for (int j = 0; j < i; j++) this[j] = this[j + 1]; this[i] = x; }
            public T RemoveAt(int i) { if (i < 0 || i >= Count) throw new Exception(); var r = this[i]; for (int j = i; j > 0; j--) this[j] = this[j - 1]; this.PopFront(); return r; }
        }
        class SegTree<T>
        {
            int n; T ti; Func<T, T, T> f; T[] dat;
            public SegTree(long _n, T _ti, Func<T, T, T> _f) { n = 1; while (n < _n) n <<= 1; ti = _ti; f = _f; dat = Repeat(ti, n << 1).ToArray(); for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]); }
            public SegTree(List<T> l, T _ti, Func<T, T, T> _f) : this(l.Count, _ti, _f) { for (var i = 0; i < l.Count; i++) dat[n + i] = l[i]; for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]); }
            public void Update(long i, T v) { dat[i += n] = v; while ((i >>= 1) > 0) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]); }
            public T Query(long l, long r) { var vl = ti; var vr = ti; for (long li = n + l, ri = n + r; li < ri; li >>= 1, ri >>= 1) { if ((li & 1) == 1) vl = f(vl, dat[li++]); if ((ri & 1) == 1) vr = f(dat[--ri], vr); } return f(vl, vr); }
            public T this[long idx] { get { return dat[idx + n]; } set { Update(idx, value); } }
        }
        class LazySegTree<T, E>
        {
            int n, height; T ti; E ei; Func<T, T, T> f; Func<T, E, T> g; Func<E, E, E> h; T[] dat; E[] laz;
            public LazySegTree(long _n, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, T> _g, Func<E, E, E> _h) { n = 1; height = 0; while (n < _n) { n <<= 1; ++height; } ti = _ti; ei = _ei; f = _f; g = _g; h = _h; dat = Repeat(ti, n << 1).ToArray(); laz = Repeat(ei, n << 1).ToArray(); for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]); }
            public LazySegTree(List<T> l, T _ti, E _ei, Func<T, T, T> _f, Func<T, E, T> _g, Func<E, E, E> _h) : this(l.Count, _ti, _ei, _f, _g, _h) { for (var i = 0; i < l.Count; i++) dat[n + i] = l[i]; for (var i = n - 1; i > 0; i--) dat[i] = f(dat[(i << 1) | 0], dat[(i << 1) | 1]); }
            T Reflect(long i) => laz[i].Equals(ei) ? dat[i] : g(dat[i], laz[i]);
            void Eval(long i) { if (laz[i].Equals(ei)) return; laz[(i << 1) | 0] = h(laz[(i << 1) | 0], laz[i]); laz[(i << 1) | 1] = h(laz[(i << 1) | 1], laz[i]); dat[i] = Reflect(i); laz[i] = ei; }
            void Thrust(long i) { for (var j = height; j > 0; j--) Eval(i >> j); }
            void Recalc(long i) { while ((i >>= 1) > 0) dat[i] = f(Reflect((i << 1) | 0), Reflect((i << 1) | 1)); }
            public void Update(long l, long r, E v) { Thrust(l += n); Thrust(r += n - 1); for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1) { if ((li & 1) == 1) { laz[li] = h(laz[li], v); ++li; } if ((ri & 1) == 1) { --ri; laz[ri] = h(laz[ri], v); } } Recalc(l); Recalc(r); }
            public T Query(long l, long r) { Thrust(l += n); Thrust(r += n - 1); var vl = ti; var vr = ti; for (long li = l, ri = r + 1; li < ri; li >>= 1, ri >>= 1) { if ((li & 1) == 1) vl = f(vl, Reflect(li++)); if ((ri & 1) == 1) vr = f(Reflect(--ri), vr); } return f(vl, vr); }
            public T this[long idx] { get { Thrust(idx += n); return dat[idx] = Reflect(idx); } set { Thrust(idx += n); dat[idx] = value; laz[idx] = ei; Recalc(idx); } }
        }
    }
}
