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
    public static class ABC025C
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var b = new long[2, 3];
            var c = new long[3, 2];
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    b[i, j] = NN;
                }
            }
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    c[i, j] = NN;
                }
            }
            Func<long[,], long[,]> deepCopy = x =>
            {
                var ret = new long[3, 3];
                for (var i = 0; i < 3; i++)
                    for (var j = 0; j < 3; j++)
                        ret[i, j] = x[i, j];
                return ret;
            };
            Func<bool, long[,], Tuple<long, long>> calc = null;
            calc = (chokudai, state) =>
            {
                var ret = Tuple.Create(0L, 0L);
                var calcp = true;
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        if (state[i, j] == 0)
                        {
                            calcp = false;
                            break;
                        }
                    }
                    if (!calcp)
                    {
                        break;
                    }
                }
                if (calcp)
                {
                    var daiToku = 0L;
                    var koToku = 0L;
                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            if (i < 2)
                            {
                                if (state[i, j] == state[i + 1, j])
                                {
                                    daiToku += b[i, j];
                                }
                                else
                                {
                                    koToku += b[i, j];
                                }
                            }
                            if (j < 2)
                            {
                                if (state[i, j] == state[i, j + 1])
                                {
                                    daiToku += c[i, j];
                                }
                                else
                                {
                                    koToku += c[i, j];
                                }
                            }
                        }
                    }
                    return Tuple.Create(daiToku, koToku);
                }
                if (chokudai)
                {
                    var max = 0L;
                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            if (state[i, j] == 0)
                            {
                                var tmp = deepCopy(state);
                                tmp[i, j] = 1;
                                var cal = calc(!chokudai, tmp);
                                if (max <= cal.Item1)
                                {
                                    max = cal.Item1;
                                    ret = cal;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var max = 0L;
                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            if (state[i, j] == 0)
                            {
                                var tmp = deepCopy(state);
                                tmp[i, j] = 2;
                                var cal = calc(!chokudai, tmp);
                                if (max <= cal.Item2)
                                {
                                    max = cal.Item2;
                                    ret = cal;
                                }
                            }
                        }
                    }
                }
                return ret;
            };
            var res = calc(true, new long[3, 3]);
            Console.WriteLine(res.Item1);
            Console.WriteLine(res.Item2);
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
