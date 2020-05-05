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
    public static class ABC163F
    {
        static bool SAIKI = false;
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var cList = NNList(N).Select(e => e - 1).ToArray();
            var abList = Repeat(0, N - 1).Select(_ => new { a = NN - 1, b = NN - 1 }).ToArray();
            var tree = new LIB_Tree(N);
            foreach (var item in abList)
            {
                tree.AddPath(item.a, item.b);
            }
            var dic = new Dictionary<long, long>[N];
            var cnt = new long[N];
            var list = Repeat(0, N).Select(_ => new List<long>()).ToArray();
            foreach (var item in tree.BFSFromLeaf(0))
            {
                foreach (var item2 in tree.GetSurround(item.node).Where(e => e != item.parent).OrderByDescending(e => dic[e].Count))
                {
                    cnt[item.node] += cnt[item2];
                    if (dic[item.node] == null)
                    {
                        dic[item.node] = dic[item2];
                        if (dic[item2].ContainsKey(cList[item.node]))
                        {
                            list[cList[item.node]].Add(cnt[item2] - dic[item2][cList[item.node]]);
                        }
                        else
                        {
                            list[cList[item.node]].Add(cnt[item2]);
                        }
                    }
                    else
                    {
                        foreach (var item3 in dic[item2])
                        {
                            if (dic[item.node].ContainsKey(item3.Key))
                            {
                                dic[item.node][item3.Key] += item3.Value;
                            }
                            else
                            {
                                dic[item.node][item3.Key] = item3.Value;
                            }
                        }
                        if (dic[item2].ContainsKey(cList[item.node]))
                        {
                            list[cList[item.node]].Add(cnt[item2] - dic[item2][cList[item.node]]);
                        }
                        else
                        {
                            list[cList[item.node]].Add(cnt[item2]);
                        }
                    }
                }
                if (dic[item.node] == null)
                {
                    dic[item.node] = new Dictionary<long, long>();
                }
                cnt[item.node]++;
                dic[item.node][cList[item.node]] = cnt[item.node];
            }
            for (var i = 0; i < N; i++)
            {
                var v = 0L;
                if (dic[0].ContainsKey(i)) v = dic[0][i];
                list[i].Add(cnt[0] - v);
            }
            var basenum = N * (N + 1) / 2;
            for (var i = 0; i < N; i++)
            {
                var ans = 0L;
                foreach (var item in list[i])
                {
                    ans += item * (item + 1) / 2;
                }
                Console.WriteLine(basenum - ans);
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
