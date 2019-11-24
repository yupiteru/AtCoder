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
    public static class ABC146D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var ab = Repeat(0, N - 1).Select(_ => new { a = (int)NN - 1, b = (int)NN - 1 }).ToArray();
            var path = Repeat(0, N).Select(_ => new List<int>()).ToArray();
            var pathToIdx = Repeat(0, N).Select(_ => new Dictionary<int, int>()).ToArray();
            for (var i = 0; i < ab.Length; i++)
            {
                var item = ab[i];
                path[item.a].Add(item.b);
                path[item.b].Add(item.a);
                pathToIdx[item.a][item.b] = i;
                pathToIdx[item.b][item.a] = i;
            }
            var maxColor = Range(0, N).Max(e => path[e].Count);
            var ans = new int[N - 1];
            Func<int, int, int, int> func = null;
            func = (vtx, parent, nocolor) =>
            {
                var c = 1;
                foreach (var item in path[vtx])
                {
                    if (item == parent) continue;
                    if (c == nocolor) ++c;
                    ans[pathToIdx[vtx][item]] = c;
                    func(item, vtx, c);
                    ++c;
                }
                return 0;
            };
            func(0, -1, -1);
            Console.WriteLine(maxColor);
            foreach (var item in ans)
            {
                Console.WriteLine(item);
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
