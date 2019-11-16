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
    public static class ABC065D
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var xy = Repeat(0, N).Select(_ => new { x = NN, y = NN }).ToArray();
            var uf = new LIB_UnionFind((int)N);
            var pq = new LIB_PriorityQueue<long, Tuple<int, int>>();
            var xs = xy.Select((e, i) => new { e, i }).OrderByRand().OrderBy(e => e.e.x).ToList();
            var ys = xy.Select((e, i) => new { e, i }).OrderByRand().OrderBy(e => e.e.y).ToList();
            for (var i = 1; i < xs.Count; i++)
            {
                pq.Push(Min(Abs(xs[i - 1].e.x - xs[i].e.x), Abs(xs[i - 1].e.y - xs[i].e.y)), Tuple.Create(xs[i - 1].i, xs[i].i));
            }
            for (var i = 1; i < ys.Count; i++)
            {
                pq.Push(Min(Abs(ys[i - 1].e.x - ys[i].e.x), Abs(ys[i - 1].e.y - ys[i].e.y)), Tuple.Create(ys[i - 1].i, ys[i].i));
            }
            var ans = 0L;
            while (pq.Count > 0)
            {
                var pop = pq.Pop();
                if (!uf.IsSame(pop.Value.Item1, pop.Value.Item2))
                {
                    ans += pop.Key;
                    uf.Unite(pop.Value.Item1, pop.Value.Item2);
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
        static Func<TR> Lambda<TR>(Func<Func<TR>, TR> f) { Func<TR> t = () => default(TR); return t = () => f(t); }
        static Func<T1, TR> Lambda<T1, TR>(Func<T1, Func<T1, TR>, TR> f) { Func<T1, TR> t = x1 => default(TR); return t = x1 => f(x1, t); }
        static Func<T1, T2, TR> Lambda<T1, T2, TR>(Func<T1, T2, Func<T1, T2, TR>, TR> f) { Func<T1, T2, TR> t = (x1, x2) => default(TR); return t = (x1, x2) => f(x1, x2, t); }
        static Func<T1, T2, T3, TR> Lambda<T1, T2, T3, TR>(Func<T1, T2, T3, Func<T1, T2, T3, TR>, TR> f) { Func<T1, T2, T3, TR> t = (x1, x2, x3) => default(TR); return t = (x1, x2, x3) => f(x1, x2, x3, t); }
        static Func<T1, T2, T3, T4, TR> Lambda<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Func<T1, T2, T3, T4, TR>, TR> f) { Func<T1, T2, T3, T4, TR> t = (x1, x2, x3, x4) => default(TR); return t = (x1, x2, x3, x4) => f(x1, x2, x3, x4, t); }
    }
}
