using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Program
{
    public class ABC114A
    {
        static public void Solve()
        {
            Console.WriteLine(new[] { 3, 5, 7 }.Contains(NextInt) ? "YES" : "NO");
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
        static List<int> NextIntList(int N) => Enumerable.Repeat(0, N).Select(_ => NextInt).ToList();
        static List<long> NextLongList(int N) => Enumerable.Repeat(0, N).Select(_ => NextLong).ToList();
        static List<double> NextDoubleList(int N) => Enumerable.Repeat(0, N).Select(_ => NextDouble).ToList();
        static List<string> NextStringList(int N) => Enumerable.Repeat(0, N).Select(_ => NextString).ToList();
        static List<T> Sort<T>(List<T> l) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp); return tmp.ToList();
        }
        static List<T> Sort<T>(List<T> l, Comparison<T> comp) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, comp); return tmp.ToList();
        }
        static List<T> RevSort<T>(List<T> l) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => y.CompareTo(x)); return tmp.ToList();
        }
        static List<T> RevSort<T>(List<T> l, Comparison<T> comp) where T : IComparable
        {
            var tmp = l.ToArray(); Array.Sort(tmp, (x, y) => comp(y, x)); return tmp.ToList();
        }
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
            for (long i = max + 1; i <= halfx; ++i) if (!table[i]) yield return 2 * i + 1;
        }
        static IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i < max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        static long GCD(long a, long b)
        {
            while (b > 0) { var tmp = b; b = a % b; a = tmp; }
            return a;
        }
        class PQ<T> where T : IComparable
        {
            private List<T> h;
            private Comparison<T> c;
            public PQ(int cap, Comparison<T> c, bool asc = true) { h = new List<T>(cap); this.c = asc ? c : (x, y) => c(y, x); }
            public PQ(Comparison<T> c, bool asc = true) { h = new List<T>(); this.c = asc ? c : (x, y) => c(y, x); }
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
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
            public PQ(int cap, Comparison<TKey> c, bool asc = true) { q = new PQ<Tuple<TKey, TValue>>(cap, (x, y) => c(x.Item1, y.Item1), asc); }
            public PQ(Comparison<TKey> c, bool asc = true) { q = new PQ<Tuple<TKey, TValue>>((x, y) => c(x.Item1, y.Item1), asc); }
            public PQ(int cap, bool asc = true) : this(cap, (x, y) => x.CompareTo(y), asc) { }
            public PQ(bool asc = true) : this((x, y) => x.CompareTo(y), asc) { }
            public void Push(TKey k, TValue v) => q.Push(Tuple.Create(k, v));
            public Tuple<TKey, TValue> Peek => q.Peek;
            public int Count => q.Count;
            public Tuple<TKey, TValue> Pop() => q.Pop();
        }
        class Mod
        {
            static public long _mod = 1000000007;
            private long _val = 0;
            public Mod(long x) { _val = x; }
            static public implicit operator Mod(long x) => new Mod(x);
            static public explicit operator long(Mod x) => x._val;
            static public Mod operator +(Mod x) => x._val;
            static public Mod operator -(Mod x) => -x._val;
            static public Mod operator ++(Mod x) => x._val + 1;
            static public Mod operator --(Mod x) => x._val - 1;
            static public Mod operator +(Mod x, Mod y) => (x._val + y._val) % _mod;
            static public Mod operator -(Mod x, Mod y) => ((((x._val - y._val) % _mod) + _mod) % _mod);
            static public Mod operator *(Mod x, Mod y) => (x._val * y._val) % _mod;
            static public Mod operator /(Mod x, Mod y) => (x._val * Inverse(y._val)) % _mod;
            static public bool operator ==(Mod x, Mod y) => x._val == y._val;
            static public bool operator !=(Mod x, Mod y) => x._val != y._val;
            static public bool operator <(Mod x, Mod y) => x._val < y._val;
            static public bool operator >(Mod x, Mod y) => x._val > y._val;
            static public bool operator <=(Mod x, Mod y) => x._val <= y._val;
            static public bool operator >=(Mod x, Mod y) => x._val >= y._val;
            static public Mod Pow(Mod x, long y)
            {
                Mod a = 1;
                while (y != 0)
                {
                    if ((y & 1) == 1) a *= x;
                    x *= x;
                    y >>= 1;
                }
                return a;
            }
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
            override public bool Equals(object obj) => obj == null ? false : _val == ((Mod)obj)._val;
            public bool Equals(Mod obj) => obj == null ? false : _val == obj._val;
            public override int GetHashCode() => _val.GetHashCode();
            public override string ToString() => _val.ToString();
            static private List<Mod> _fact = new List<Mod>();
            static private List<Mod> _ifact = new List<Mod>();
            static private void Build(int n)
            {
                if (n >= _fact.Count)
                    for (int i = _fact.Count; i <= n; ++i)
                        if (i == 0L) { _fact.Add(1); _ifact.Add(1); }
                        else { _fact.Add(_fact[i - 1] * i); _ifact.Add(_ifact[i - 1] * Mod.Pow(i, _mod - 2)); }
            }
            static public Mod Comb(int n, int k)
            {
                Build(n);
                if (n == 0 && k == 0) return 1;
                if (n < k || n < 0) return 0;
                return _ifact[n - k] * _ifact[k] * _fact[n];
            }
        }
        static private List<long> _fact = new List<long>();
        static private void Build(int n)
        {
            if (n >= _fact.Count)
                for (int i = _fact.Count; i <= n; ++i)
                    if (i == 0L) _fact.Add(1);
                    else _fact.Add(_fact[i - 1] * i);
        }
        static public long Comb(int n, int k)
        {
            Build(n);
            if (n == 0 && k == 0) return 1;
            if (n < k || n < 0) return 0;
            return _fact[n] / _fact[k] / _fact[n - k];
        }
    }
}
