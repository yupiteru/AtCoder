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
    public static class ABC146F
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var M = NN;
            var S = NS;
            var seg = new LIB_LazySegTree<Tuple<int, int>, Tuple<int, int>>(N + 1, Tuple.Create(1000000000, 0), Tuple.Create(1000000000, 0), (x, y) =>
            {
                if (x.Item1 <= y.Item1) return x;
                else return y;
            }, (x, y) =>
            {
                if (x.Item1 <= y.Item1) return x;
                else return y;
            }, (x, y) =>
            {
                if (x.Item1 <= y.Item1) return x;
                else return y;
            });
            seg[0] = Tuple.Create(0, 0);
            for (var i = 0; i <= N; i++)
            {
                if (S[i] == '1') continue;
                seg.Update(i, Min(i + M + 1, N + 1), Tuple.Create(seg[i].Item1 + 1, i));
            }
            if (seg[N].Item1 == 1000000000)
            {
                Console.WriteLine(-1);
            }
            else
            {
                var now = N;
                var ans = new List<long>();
                while (now != 0)
                {
                    var before = seg[now].Item2;
                    ans.Add(now - before);
                    now = before;
                }
                ans.Reverse();
                Console.WriteLine(string.Join(" ", ans));
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