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
    public static class ABC065B
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var AList = NNList(N);
            var path = Repeat(0, N).Select(_ => new Dictionary<int, int>()).ToArray();
            for (var i = 0; i < N; i++)
            {
                path[i][(int)AList[i] - 1] = 1;
            }
            {
                var n = N;
                var s = 0;
                var dist = Repeat(long.MaxValue >> 2, n).ToArray();
                var pred = new int?[n];
                dist[s] = 0;
                var q = new LIB_PriorityQueue<long, int>();
                q.Push(0, (int)s);
                while (q.Count > 0)
                {
                    var u = q.Pop().Value;
                    foreach (var pathItem in path[u])
                    {
                        var v = pathItem.Key;
                        var alt = dist[u] + pathItem.Value;
                        if (dist[v] > alt)
                        {
                            dist[v] = alt;
                            pred[v] = u;
                            q.Push(alt, v);
                        }
                    }
                }
                if (pred[1].HasValue)
                {
                    Console.WriteLine(dist[1]);
                }
                else
                {
                    Console.WriteLine(-1);
                }
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
        static Func<TR> Lambda<TR>(Func<Func<TR>, TR> f) { Func<TR> t = () => default(TR); return t = () => f(t); }
        static Func<T1, TR> Lambda<T1, TR>(Func<T1, Func<T1, TR>, TR> f) { Func<T1, TR> t = x1 => default(TR); return t = x1 => f(x1, t); }
        static Func<T1, T2, TR> Lambda<T1, T2, TR>(Func<T1, T2, Func<T1, T2, TR>, TR> f) { Func<T1, T2, TR> t = (x1, x2) => default(TR); return t = (x1, x2) => f(x1, x2, t); }
        static Func<T1, T2, T3, TR> Lambda<T1, T2, T3, TR>(Func<T1, T2, T3, Func<T1, T2, T3, TR>, TR> f) { Func<T1, T2, T3, TR> t = (x1, x2, x3) => default(TR); return t = (x1, x2, x3) => f(x1, x2, x3, t); }
        static Func<T1, T2, T3, T4, TR> Lambda<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Func<T1, T2, T3, T4, TR>, TR> f) { Func<T1, T2, T3, T4, TR> t = (x1, x2, x3, x4) => default(TR); return t = (x1, x2, x3, x4) => f(x1, x2, x3, x4, t); }
    }
}
