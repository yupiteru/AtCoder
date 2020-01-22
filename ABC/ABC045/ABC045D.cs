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
    public static class ABC045D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var H = NN;
            var W = NN;
            var N = NN;
            var ab = Repeat(0, N).Select(_ => new { a = NN, b = NN }).ToArray();
            var table = new LIB_Dict<long, LIB_Dict<long, int>>(_ => new LIB_Dict<long, int>());
            foreach (var item in ab)
            {
                for (var i = -1; i < 2; i++)
                {
                    if (item.a + i <= 0 || item.a + i > H) continue;
                    for (var j = -1; j < 2; j++)
                    {
                        if (item.b + j <= 0 || item.b + j > W) continue;
                        table[item.a + i][item.b + j]++;
                    }
                }
            }
            var ans = new long[10];
            var zeroCnt = (H - 2) * (W - 2);
            foreach (var item in table)
            {
                if (item.Key <= 1 || item.Key >= H) continue;
                foreach (var item2 in item.Value)
                {
                    if (item2.Key <= 1 || item2.Key >= W) continue;
                    ++ans[item2.Value];
                    zeroCnt--;
                }
            }
            ans[0] = zeroCnt;
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
