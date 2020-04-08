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
    public static class ABC157F
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var K = NN;
            var xycList = Repeat(0, N).Select(_ => new { x = ND, y = ND, c = ND }).ToArray();
            var left = 0d;
            var right = 3000000d;
            var centers = xycList.Select(e => new LIB_Geo2D.Vec(e.x, e.y)).ToArray();
            for (int xx = 0; xx < 200; ++xx)
            {
                var mid = (right + left) / 2;
                var cnt = 0L;
                {
                    var points = new List<LIB_Geo2D.Vec>();
                    var curcles = xycList.Select(e => new LIB_Geo2D.Circle() { p = new LIB_Geo2D.Vec(e.x, e.y), r2 = (mid / e.c) * (mid / e.c) }).ToArray();
                    for (var i = 0; i < N; i++)
                    {
                        for (var j = i + 1; j < N; j++)
                        {
                            points.AddRange(LIB_Geo2D.Intersection(curcles[i], curcles[j]));
                        }
                    }
                    foreach (var item in points.Concat(centers))
                    {
                        var thisCnt = 0L;
                        foreach (var item2 in curcles)
                        {
                            if (LIB_Geo2D.Distance(item, item2) <= LIB_Geo2D.EPS) ++thisCnt;
                        }
                        cnt = Max(cnt, thisCnt);
                    }
                }
                if (cnt < K) left = mid;
                else right = mid;
            }
            Console.WriteLine(right);
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
        static IEnumerable<string> OrderBy(this IEnumerable<string> x) => x.OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
        static IEnumerable<T> OrderBy<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderBy(selector, StringComparer.OrdinalIgnoreCase);
        static IEnumerable<string> OrderByDescending(this IEnumerable<string> x) => x.OrderByDescending(e => e, StringComparer.OrdinalIgnoreCase);
        static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByDescending(selector, StringComparer.OrdinalIgnoreCase);
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
    }
}
namespace Library
{
}
