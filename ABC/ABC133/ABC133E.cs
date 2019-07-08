using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;

namespace Program
{
    public static class ABC133E
    {
        static public void Solve()
        {

            var N = NN;
            var K = NN;
            var ab = Repeat(0, N - 1).Select(_ => new { a = NN - 1, b = NN - 1 }).ToArray();
            var path = Repeat(0, N).Select(_ => new List<long>()).ToArray();
            foreach (var item in ab)
            {
                path[item.a].Add(item.b);
                path[item.b].Add(item.a);
            }
            var tree = new Tree(path, 0);

            var pat = new Mod[N];
            foreach (var item in tree.FromLeaf())
            {
                var rootAdj = item.Item2 == -1 ? 1 : 0;
                var childCount = path[item.Item1].Count - 1 + rootAdj;
                pat[item.Item1] = childCount == 0 ? 1 : Mod.Perm(K - 2 + rootAdj, childCount);
                foreach (var child in path[item.Item1])
                {
                    if (child == item.Item2) continue;
                    pat[item.Item1] *= pat[child];
                }
            }

            Console.WriteLine(pat[0] * K);
        }

        static public void Main(string[] args) { if (args.Length == 0) { var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false }; Console.SetOut(sw); } Solve(); Console.Out.Flush(); }
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
        class PQ<T> where T : IComparable
        {
            private List<T> h; private Comparison<T> c; public T Peek => h[0]; public int Count => h.Count;
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
            private PQ<Tuple<TK, TV>> q; public Tuple<TK, TV> Peek => q.Peek; public int Count => q.Count;
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
            private long[] d;
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
            static public long _mod = 1000000007; private long _val;
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
            static private void B(long n) { if (n >= _fact.Count) for (int i = _fact.Count; i <= n; ++i) _fact.Add(_fact[i - 1] * i); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Comb(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)] / _fact[(int)k]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Mod Perm(long n, long k) { B(n); if (n == 0 && k == 0) return 1; if (n < k || n < 0) return 0; return _fact[(int)n] / _fact[(int)(n - k)]; }
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
        class Tree
        {
            private long N; private int l; private List<long>[] p; private int[] d; private long[][] pr; private long r; private Tuple<long, long, int>[] e; private Tuple<long, long>[] b; private bool lca; private bool euler; private bool bfs;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tree(List<long>[] p_, long r_) { N = p_.Length; p = p_; r = r_; lca = false; euler = false; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long>[] FromRoot() { if (!bfs) { var nb = new List<Tuple<long, long>>(); var q = new Queue<long>(); var d = new bool[N]; nb.Add(Tuple.Create(r, -1L)); d[r] = true; q.Enqueue(r); while (q.Count > 0) { var w = q.Dequeue(); foreach (var i in p[w]) { if (d[i]) continue; d[i] = true; q.Enqueue(i); nb.Add(Tuple.Create(i, w)); } } b = nb.ToArray(); bfs = true; } return b; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long>[] FromLeaf() => FromRoot().Reverse().ToArray();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tuple<long, long, int>[] Euler() { if (!euler) { var ne = new List<Tuple<long, long, int>>(); var s = new Stack<Tuple<long, long>>(); var d = new bool[N]; d[r] = true; s.Push(Tuple.Create(r, -1L)); while (s.Count > 0) { var w = s.Peek(); var ad = true; foreach (var i in p[w.Item1]) { if (d[i]) continue; d[i] = true; ad = false; s.Push(Tuple.Create(i, w.Item1)); } if (!ad || p[w.Item1].Count == 1) ne.Add(Tuple.Create(w.Item1, w.Item2, 1)); if (ad) { s.Pop(); ne.Add(Tuple.Create(w.Item1, w.Item2, -1)); } } e = ne.ToArray(); euler = true; } return e; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long LCA(long u, long v) { if (!lca) { l = 0; while (N > (1 << l)) l++; d = new int[N]; pr = Repeat(0, l).Select(_ => new long[N]).ToArray(); d[r] = 0; pr[0][r] = -1; var q = new Stack<long>(); q.Push(r); while (q.Count > 0) { var w = q.Pop(); foreach (var i in p[w]) { if (i == pr[0][w]) continue; q.Push(i); d[i] = d[w] + 1; pr[0][i] = w; } } for (var k = 0; k + 1 < l; k++) for (var w = 0; w < N; w++) if (pr[k][w] < 0) pr[k + 1][w] = -1; else pr[k + 1][w] = pr[k][pr[k][w]]; lca = true; } if (d[u] > d[v]) { var t = u; u = v; v = t; } for (var k = 0; k < l; k++) if ((((d[v] - d[u]) >> k) & 1) != 0) v = pr[k][v]; if (u == v) return u; for (var k = l - 1; k >= 0; k--) if (pr[k][u] != pr[k][v]) { u = pr[k][u]; v = pr[k][v]; } return pr[0][u]; }
        }
        class BT<T> where T : IComparable
        {
            private class Node
            {
                public Node left;
                public Node right;
                public T val;
                public bool isBlack;
            }
            private Comparison<T> c;
            private Node root;
            private bool change;
            public BT(Comparison<T> _c) { c = _c; }
            public BT() : this((x, y) => x.CompareTo(y)) { }
            private bool Red(Node n) => n != null && !n.isBlack;
            private bool Black(Node n) => n != null && n.isBlack;
            private Node RotateL(Node n)
            {
                Node m = n.right, t = m.left;
                m.left = n; n.right = t;
                return m;
            }
            private Node RotateR(Node n)
            {
                Node m = n.left, t = m.right;
                m.right = n; n.left = t;
                return m;
            }
            private Node RotateLR(Node n)
            {
                n.left = RotateL(n.left);
                return RotateR(n);
            }
            private Node RotateRL(Node n)
            {
                n.right = RotateR(n.right);
                return RotateL(n);
            }
            public void Add(T x)
            {
                root = Add(root, x);
                root.isBlack = true;
            }
            private Node Add(Node n, T x)
            {
                if (n == null)
                {
                    change = true;
                    return new Node() { val = x };
                }
                var r = c(x, n.val);
                if (r < 0)
                {
                    n.left = Add(n.left, x);
                    return Balance(n);
                }
                if (r > 0)
                {
                    n.right = Add(n.right, x);
                    return Balance(n);
                }
                change = false;
                return n;
            }

            private Node Balance(Node n)
            {
                if (!change) return n;
                if (!Black(n)) return n;
                if (Red(n.left) && Red(n.left.left))
                {
                    n = RotateR(n);
                    n.left.isBlack = true;
                }
                else if (Red(n.left) && Red(n.left.right))
                {
                    n = RotateLR(n);
                    n.left.isBlack = true;
                }
                else if (Red(n.right) && Red(n.right.left))
                {
                    n = RotateRL(n);
                    n.right.isBlack = true;
                }
                else if (Red(n.right) && Red(n.right.right))
                {
                    n = RotateL(n);
                    n.right.isBlack = true;
                }
                else
                {
                    change = false;
                }
                return n;
            }

            public void Remove(T x)
            {
                root = Remove(root, x);
                if (root != null) root.isBlack = true;
            }
            private Node Remove(Node n, T x)
            {
                if (n == null)
                {
                    change = false;
                    return n;
                }
                var r = c(x, n.val);
                if (r < 0)
                {
                    n.left = Remove(n.left, x);
                    return BalanceL(n);
                }
                if (r > 0)
                {
                    n.right = Remove(n.right, x);
                    return BalanceR(n);
                }
                if (n.left == null)
                {
                    change = n.isBlack;
                    return n.right;
                }
                n.left = RemoveMax(n.left);
                n.val = lmax;
                return BalanceL(n);
            }
            private T lmax;
            private Node RemoveMax(Node n)
            {
                if (n.right != null)
                {
                    n.right = RemoveMax(n.right);
                    return BalanceR(n);
                }
                lmax = n.val;
                change = n.isBlack;
                return n.left;
            }
            private Node BalanceL(Node n)
            {
                if (!change) return n;
                if (Black(n.right) && Red(n.right.left))
                {
                    var b = n.isBlack;
                    n = RotateRL(n);
                    n.isBlack = b;
                    n.left.isBlack = true;
                    change = false;
                }
                else if (Black(n.right) && Red(n.right.right))
                {
                    var b = n.isBlack;
                    n = RotateL(n);
                    n.isBlack = b;
                    n.right.isBlack = true;
                    n.left.isBlack = true;
                    change = false;
                }
                else if (Black(n.right))
                {
                    change = n.isBlack;
                    n.isBlack = true;
                    n.right.isBlack = false;
                }
                else
                {
                    n = RotateL(n);
                    n.isBlack = true;
                    n.left.isBlack = false;
                    n.left = BalanceL(n.left);
                    change = false;
                }
                return n;
            }
            private Node BalanceR(Node n)
            {
                if (!change) return n;
                if (Black(n.left) && Red(n.left.right))
                {
                    var b = n.isBlack;
                    n = RotateLR(n);
                    n.isBlack = b;
                    n.right.isBlack = true;
                    change = false;
                }
                else if (Black(n.left) && Red(n.left.left))
                {
                    var b = n.isBlack;
                    n = RotateR(n);
                    n.isBlack = b;
                    n.left.isBlack = true;
                    n.right.isBlack = true;
                    change = false;
                }
                else if (Black(n.left))
                {
                    change = n.isBlack;
                    n.isBlack = true;
                    n.left.isBlack = false;
                }
                else
                {
                    n = RotateR(n);
                    n.isBlack = true;
                    n.right.isBlack = false;
                    n.right = BalanceR(n.right);
                    change = false;
                }
                return n;
            }
            public bool Have(T x)
            {
                var t = FindUpper(x);
                return t.Item1 && t.Item2.CompareTo(x) == 0;
            }
            public Tuple<bool, T> FindUpper(T x)
            {
                var v = FindUpper(root, x);
                if (v == null) return Tuple.Create(false, default(T));
                return v;
            }
            private Tuple<bool, T> FindUpper(Node n, T x)
            {
                if (n == null) return null;
                var r = c(x, n.val);
                if (r < 0)
                {
                    var v = FindUpper(n.left, x);
                    if (v == null) return Tuple.Create(true, n.val);
                    return v;
                }
                else if (r > 0) return FindUpper(n.right, x);
                else return Tuple.Create(true, n.val);
            }
            public Tuple<bool, T> FindLower(T x)
            {
                var v = FindLower(root, x);
                if (v == null) return Tuple.Create(false, default(T));
                return v;
            }
            private Tuple<bool, T> FindLower(Node n, T x)
            {
                if (n == null) return null;
                var r = c(x, n.val);
                if (r < 0) return FindLower(n.left, x);
                else if (r > 0)
                {
                    var v = FindLower(n.right, x);
                    if (v == null) return Tuple.Create(true, n.val);
                    return v;
                }
                else return Tuple.Create(true, n.val);
            }
            public T Min()
            {
                Node n = root, p = null;
                while (n != null) { p = n; n = n.left; }
                return p == null ? default(T) : p.val;
            }
            public T Max()
            {
                Node n = root, p = null;
                while (n != null) { p = n; n = n.right; }
                return p == null ? default(T) : p.val;
            }
            public bool Any() => root != null;
            public int CountSlow() => List(root).Count();
            public IEnumerable<T> List() => List(root);
            private IEnumerable<T> List(Node n)
            {
                if (n == null) yield break;
                foreach (var i in List(n.left)) yield return i;
                yield return n.val;
                foreach (var i in List(n.right)) yield return i;
            }
        }
    }
}
