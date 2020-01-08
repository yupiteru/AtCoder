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
    public static class ABC149E
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var M = NN;
            var AList = NNList(N).OrderByRand().OrderBy(e => e).ToArray();
            var tree = new LIB_RedBlackTree<long>();
            foreach (var item in AList)
            {
                tree.Add(item);
            }
            var left = 2L;
            var right = 200001L;
            while (right - left > 1)
            {
                var mid = (right + left) / 2;
                var cnt = 0L;
                foreach (var item in AList)
                {
                    var tgt = mid - item;
                    var idx = tree.LowerBound(tgt);
                    cnt += N - idx;
                }
                if (cnt >= M) left = mid;
                else right = mid;
            }
            var pivot = left;
            var seg = new LIB_DualSegTree<long>(N, 0, (x, y) => x + y);
            for (var i = 0; i < N; i++)
            {
                var idx = tree.LowerBound(pivot - AList[i]);
                seg.Update(idx, N, 1);
                seg[i] += N - idx;
            }
            var ans = 0L;
            var count = 0L;
            for (var i = 0; i < N; i++)
            {
                ans += seg[i] * AList[i];
                count += seg[i];
            }
            if (count / 2 > M) ans -= pivot * (count / 2 - M);
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
