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
    public static class SOUNDHOUND2018_SUMMER_QUALD
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var n = NN;
            var m = NN;
            var s = NN - 1;
            var t = NN - 1;
            var uvab = Repeat(0, m).Select(_ => new { u = (int)NN - 1, v = (int)NN - 1, a = NN, b = NN }).ToArray();
            var enPath = Repeat(0, n).Select(_ => new Dictionary<int, long>()).ToArray();
            var sunukuPath = Repeat(0, n).Select(_ => new Dictionary<int, long>()).ToArray();
            foreach (var item in uvab)
            {
                enPath[item.u][item.v] = item.a;
                sunukuPath[item.u][item.v] = item.b;
                enPath[item.v][item.u] = item.a;
                sunukuPath[item.v][item.u] = item.b;
            }
            var nodeNum = n;
            var enDist = Repeat(long.MaxValue >> 2, nodeNum).ToArray();
            {
                var start = s;
                var pred = new int?[nodeNum];
                enDist[start] = 0;
                var q = new LIB_PriorityQueue<long, int>();
                q.Push(0, (int)start);
                while (q.Count > 0)
                {
                    var u = q.Pop().Value;
                    foreach (var pathItem in enPath[u])
                    {
                        var v = pathItem.Key;
                        var alt = enDist[u] + pathItem.Value;
                        if (enDist[v] > alt)
                        {
                            enDist[v] = alt;
                            pred[v] = u;
                            q.Push(alt, v);
                        }
                    }
                }
            }
            var sunukuDist = Repeat(long.MaxValue >> 2, nodeNum).ToArray();
            {
                var start = t;
                var pred = new int?[nodeNum];
                sunukuDist[start] = 0;
                var q = new LIB_PriorityQueue<long, int>();
                q.Push(0, (int)start);
                while (q.Count > 0)
                {
                    var u = q.Pop().Value;
                    foreach (var pathItem in sunukuPath[u])
                    {
                        var v = pathItem.Key;
                        var alt = sunukuDist[u] + pathItem.Value;
                        if (sunukuDist[v] > alt)
                        {
                            sunukuDist[v] = alt;
                            pred[v] = u;
                            q.Push(alt, v);
                        }
                    }
                }
            }
            var ans = new long[n];
            var min = 10000000000000000;
            for (var i = n - 1; i >= 0; i--)
            {
                var sumEnSunuku = enDist[i] + sunukuDist[i];
                min = Min(min, sumEnSunuku);
                ans[i] = min;
            }
            for (var i = 0; i < n; i++)
            {
                Console.WriteLine(1000000000000000 - ans[i]);
            }
        }
        static class Console_
        {
            static Queue<string> param = new Queue<string>();
            public static string NextString() { if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item); return param.Dequeue(); }
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static public void Main(string[] args) { if (args.Length == 0) { Console.SetOut(new Printer(Console.OpenStandardOutput())); } var t = new Thread(Solve, 134217728); t.Start(); t.Join(); Console.Out.Flush(); }
        static long NN => long.Parse(Console_.NextString());
        static double ND => double.Parse(Console_.NextString());
        static string NS => Console_.NextString();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        static IEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => x.OrderBy(_ => xorshift);
        static long Count<T>(this IEnumerable<T> x, Func<T, bool> pred) => Enumerable.Count(x, pred);
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
    }
}
