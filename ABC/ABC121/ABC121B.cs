using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC121B
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var M = NextInt;
            var C = NextInt;
            var B = NextIntList(M);
            var Alist = new List<List<int>>(N);
            var ans = 0;
            for (int i = 0; i < N; ++i)
            {
                var tmp = C;
                for (int j = 0; j < M; ++j)
                {
                    tmp += NextInt * B[j];
                }
                if (tmp > 0) ++ans;
            }

            Console.WriteLine(ans);
        }

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
        static List<int> NextIntList(int N)
        {
            var ret = new List<int>(N);
            for (int i = 0; i < N; ++i) ret.Add(NextInt);
            return ret;
        }
        static long NextLong => long.Parse(Console_.NextString());
        static List<long> NextLongList(int N)
        {
            var ret = new List<long>(N);
            for (int i = 0; i < N; ++i) ret.Add(NextLong);
            return ret;
        }
        static string NextString => Console_.NextString();
        static void Sort<T>(List<T> l) where T : IComparable => l.Sort();
        static void RevSort<T>(List<T> l) where T : IComparable => l.Sort((x, y) => y.CompareTo(x));
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
            long tmpa = a, tmpb = b;
            while (tmpb > 0) { var tmp = tmpb; tmpb = tmpa % tmpb; tmpa = tmp; }
            return tmpa;
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
            public T Pop
            {
                get
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
                private set { }
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
            public Tuple<TKey, TValue> Pop => q.Pop;
        }
        static class Mod
        {
            static public long _mod = 1000000007;
            static public long Add(long x, long y) => (x + y) % _mod;
            static public long Sub(long x, long y) => (x - y) % _mod;
            static public long Multi(long x, long y) => (x * y) % _mod;
            static public long Div(long x, long y) => (x * Inverse(y)) % _mod;
            static public long Pow(long x, long y)
            {
                var a = 1L;
                while (y != 0)
                {
                    if ((y & 1) == 1) a = Mod.Multi(a, x);
                    x = Mod.Multi(x, x);
                    y >>= 1;
                }
                return a;
            }
            static public long Inverse(long x)
            {
                var b = _mod;
                var r = 1L;
                var u = 0L;
                while (b > 0L)
                {
                    var q = x / b;
                    var t = u; u = r - q * u; r = t;
                    t = b; b = x - q * b; x = t;
                }
                return r < 0 ? r + _mod : r;
            }
            static private List<long> _fact = new List<long>();
            static private List<long> _ifact = new List<long>();
            static private void Build(int n)
            {
                if (n >= _fact.Count)
                    for (int i = _fact.Count; i <= n; ++i)
                        if (i == 0L) { _fact.Add(1); _ifact.Add(1); }
                        else { _fact.Add(Mod.Multi(_fact[i - 1], i)); _ifact.Add(Mod.Multi(_ifact[i - 1], Mod.Pow(i, _mod - 2))); }
            }
            static public long Comb(int n, int k)
            {
                Build(n);
                if (n == 0 && k == 0) return 1;
                if (n < k || n < 0) return 0;
                return Mod.Multi(Mod.Multi(_ifact[n - k], _ifact[k]), _fact[n]);
            }
        }
    }
}
