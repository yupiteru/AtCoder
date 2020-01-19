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
    public static class ABC152D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var keta = N.ToString().Length;
            var cnt = 0L;
            for (var i = 1; i <= 9; i++)
            {
                for (var j = 1; j <= 9; j++)
                {
                    for (var k = 1; k <= keta; k++)
                    {
                        var ashurui = 0L;
                        if (k < keta)
                        {
                            if (k == 1 || k == 2) ashurui = 1;
                            else ashurui = LIB_Math.Pow(10, k - 2);
                        }
                        else
                        {
                            if (i == (N.ToString()[0] - '0'))
                            {
                                if (j <= (N.ToString().Last() - '0'))
                                {
                                    if (k == 1 || k == 2) ashurui = 1;
                                    else ashurui = (N % LIB_Math.Pow(10, keta - 1)) / 10 + 1;
                                }
                                else
                                {
                                    ashurui = (N % LIB_Math.Pow(10, keta - 1)) / 10;
                                }
                            }
                            else if (i < (N.ToString()[0] - '0'))
                            {
                                if (k == 1 || k == 2) ashurui = 1;
                                else ashurui = LIB_Math.Pow(10, k - 2);
                            }
                            else
                            {
                                ashurui = 0;
                            }
                        }
                        if (k == 1 && i != j) ashurui = 0;
                        for (var l = 1; l <= keta; l++)
                        {

                            var bshurui = 0L;
                            if (l < keta)
                            {
                                if (l == 1 || l == 2) bshurui = 1;
                                else bshurui = LIB_Math.Pow(10, l - 2);
                            }
                            else
                            {
                                if (j == (N.ToString()[0] - '0'))
                                {
                                    if (i <= (N.ToString().Last() - '0'))
                                    {
                                        if (l == 1 || l == 2) bshurui = 1;
                                        else bshurui = (N % LIB_Math.Pow(10, keta - 1)) / 10 + 1;
                                    }
                                    else
                                    {
                                        bshurui = (N % LIB_Math.Pow(10, keta - 1)) / 10;
                                    }
                                }
                                else if (j < (N.ToString()[0] - '0'))
                                {
                                    if (l == 1 || l == 2) bshurui = 1;
                                    else bshurui = LIB_Math.Pow(10, l - 2);
                                }
                                else
                                {
                                    bshurui = 0;
                                }
                            }
                            if (l == 1 && i != j) bshurui = 0;
                            cnt += ashurui * bshurui;
                        }
                    }
                }
            }
            Console.WriteLine(cnt);
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
