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
    public static class ABC149D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var K = NN;
            var R = NN;
            var S = NN;
            var P = NN;
            var T = NS;
            var ary = new List<Tuple<long, int>>();
            ary.Add(Tuple.Create(R, 0));
            ary.Add(Tuple.Create(S, 1));
            ary.Add(Tuple.Create(P, 2));
            ary = ary.OrderByDescending(e => e.Item1).ToList();
            var choice = new int[N];
            for (var i = 0; i < N; i++)
            {
                choice[i] = -1;
            }
            foreach (var item in ary)
            {
                for (var i = 0; i < N; i++)
                {
                    var aite = 0;
                    if (T[i] == 'r')
                    {
                        aite = 0;
                    }
                    else if (T[i] == 's')
                    {
                        aite = 1;
                    }
                    else
                    {
                        aite = 2;
                    }
                    if (choice[i] == -1)
                    {
                        if (aite == (item.Item2 + 1) % 3)
                        {
                            if (i >= K)
                            {
                                if (choice[i - K] != item.Item2)
                                {
                                    choice[i] = item.Item2;
                                }
                            }
                            else
                            {
                                choice[i] = item.Item2;
                            }
                        }
                    }
                }
            }
            var ans = 0L;
            for (var i = 0; i < N; i++)
            {
                var aite = 0;
                if (T[i] == 'r')
                {
                    aite = 0;
                }
                else if (T[i] == 's')
                {
                    aite = 1;
                }
                else
                {
                    aite = 2;
                }
                if (aite == (choice[i] + 1) % 3)
                {
                    if (choice[i] == 0) ans += R;
                    if (choice[i] == 1) ans += S;
                    if (choice[i] == 2) ans += P;
                }
            }
            Console.WriteLine(ans);
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
