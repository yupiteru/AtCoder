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
    public static class ABC161D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var K = NN;
            var tmp = Repeat(-1L, 100000).ToArray();
            tmp[0] = 1;
            for (var i = 1; i < K; i++)
            {
                Func<long, long> fun = null;
                fun = idx =>
                {
                    if (tmp[idx] == -1)
                    {
                        tmp[idx] = 1;
                        return 1;
                    }
                    ++tmp[idx];
                    if (tmp[idx] == 10)
                    {
                        var nex2 = fun(idx + 1);
                        tmp[idx] = nex2 - 1;
                        if (tmp[idx] < 0) tmp[idx] = 0;
                        return tmp[idx];
                    }
                    if (tmp[idx + 1] == -1) return tmp[idx];
                    if (Abs(tmp[idx] - tmp[idx + 1]) <= 1) return tmp[idx];
                    var nex = fun(idx + 1);
                    tmp[idx] = nex - 1;
                    if (tmp[idx] < 0) tmp[idx] = 0;
                    return tmp[idx];
                };
                fun(0);
            }
            var output = false;
            foreach (var item in tmp.Reverse())
            {
                if (item >= 0) output = true;
                if (output) Console.Write(item);
            }
            Console.WriteLine();
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static LIB_FastIO fastio = new LIB_FastIODebug();
        static public void Main(string[] args) { if (args.Length == 0) { fastio = new LIB_FastIO(); Console.SetOut(new Printer(Console.OpenStandardOutput())); } var t = new Thread(Solve, 134217728); t.Start(); t.Join(); Console.Out.Flush(); }
        static long NN => fastio.Long();
        static double ND => fastio.Double();
        static string NS => fastio.Scan();
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