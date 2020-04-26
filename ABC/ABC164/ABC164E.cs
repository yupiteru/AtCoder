using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using Library;

namespace Program
{
    public static class ABC164E
    {
        static bool SAIKI = false;
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        struct HEN
        {
            public long A;
            public long B;
        }
        static public void Solve()
        {
            var N = NN;
            var M = NN;
            var S = NN;
            var UVAB = Repeat(0, M).Select(_ => new { U = NN - 1, V = NN - 1, A = NN, B = NN }).ToArray();
            var CD = Repeat(0, N).Select(_ => new { C = NN, D = NN }).ToArray();
            var path = Repeat(0, N).Select(_ => new Dictionary<long, HEN>()).ToArray();
            foreach (var item in UVAB)
            {
                path[item.U][item.V] = new HEN { A = item.A, B = item.B };
                path[item.V][item.U] = new HEN { A = item.A, B = item.B };
            }
            var pathe = new List<Tuple<Tuple<long, long>, long>>[N, 2501];
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j <= 2500; j++)
                {
                    pathe[i, j] = new List<Tuple<Tuple<long, long>, long>>();
                }
            }
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j <= 2500; j++)
                {
                    if (j + CD[i].C <= 2500) pathe[i, j].Add(Tuple.Create(Tuple.Create((long)i, j + CD[i].C), CD[i].D));
                    foreach (var item in path[i])
                    {
                        if (j - item.Value.A >= 0) pathe[(long)i, j].Add(Tuple.Create(Tuple.Create(item.Key, j - item.Value.A), item.Value.B));
                    }
                }
            }
            {
                var nodeNum = N * 2501;
                var dist = new long[N, 2501];
                for (var i = 0; i < N; i++)
                {
                    for (var j = 0; j <= 2500; j++)
                    {
                        dist[i, j] = long.MaxValue >> 2;
                    }
                }
                dist[0, Min(S, 2500)] = 0;
                var q = new LIB_PriorityQueue<long, Tuple<long, long>>();
                q.Push(0, Tuple.Create(0L, Min(S, 2500)));
                while (q.Count > 0)
                {
                    var u = q.Pop().Value;
                    foreach (var pathItem in pathe[u.Item1, u.Item2])
                    {
                        var v = pathItem.Item1;
                        var alt = dist[u.Item1, u.Item2] + pathItem.Item2;
                        if (dist[v.Item1, v.Item2] > alt)
                        {
                            dist[v.Item1, v.Item2] = alt;
                            q.Push(alt, v);
                        }
                    }
                }
                for (var i = 1; i < N; i++)
                {
                    var ans = long.MaxValue;
                    for (var j = 0; j <= 2500; j++)
                    {
                        ans = Min(ans, dist[i, j]);
                    }
                    Console.WriteLine(ans);
                }
            }
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static LIB_FastIO fastio = new LIB_FastIODebug();
        static public void Main(string[] args) { if (args.Length == 0) { fastio = new LIB_FastIO(); Console.SetOut(new Printer(Console.OpenStandardOutput())); } if (SAIKI) { var t = new Thread(Solve, 134217728); t.Start(); t.Join(); } else Solve(); Console.Out.Flush(); }
        static long NN => fastio.Long();
        static double ND => fastio.Double();
        static string NS => fastio.Scan();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        static long Count<T>(this IEnumerable<T> x, Func<T, bool> pred) => Enumerable.Count(x, pred);
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        static IOrderedEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x, _ => xorshift);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderBy<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderBy(x.OrderByRand(), selector);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x) => Enumerable.OrderByDescending(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderByDescending<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderByDescending(x.OrderByRand(), selector);
        static IOrderedEnumerable<string> OrderBy(this IEnumerable<string> x) => x.OrderByRand().OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderBy(selector, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<string> OrderByDescending(this IEnumerable<string> x) => x.OrderByRand().OrderByDescending(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderByDescending(selector, StringComparer.OrdinalIgnoreCase);
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
    }
}
