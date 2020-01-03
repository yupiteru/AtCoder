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
    public static class ABC148F
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var u = NN - 1;
            var v = NN - 1;
            var AB = Repeat(0, N - 1).Select(_ => new { A = NN - 1, B = NN - 1 }).ToArray();
            var path = Repeat(0, N).Select(_ => new List<long>()).ToArray();
            foreach (var item in AB)
            {
                path[item.A].Add(item.B);
                path[item.B].Add(item.A);
            }
            Func<long, long, long, long> dfs0 = null;
            Func<long, long, long, long> dfs1 = null;
            Func<long, long, long> dfs2 = null;
            Func<long, long, long> dfs3 = null;
            var saienTyoten = -1L;
            var saienKyori = 0L;
            var map = new long[N];
            dfs0 = (vtx, parent, dist) =>
            {
                map[vtx] = dist;
                foreach (var item in path[vtx])
                {
                    if (item == parent) continue;
                    dfs0(item, vtx, dist + 1);
                }
                return 0;
            };
            dfs1 = (vtx, parent, dist) =>
            {
                if (map[vtx] <= dist) return 0;
                if (saienKyori < map[vtx])
                {
                    saienKyori = map[vtx];
                    saienTyoten = vtx;
                }
                foreach (var item in path[vtx])
                {
                    if (item == parent) continue;
                    dfs1(item, vtx, dist + 1);
                }
                return 0;
            };
            dfs2 = (vtx, parent) =>
            {
                var ret = -1L;
                if (vtx == saienTyoten)
                {
                    return 0;
                }
                foreach (var item in path[vtx])
                {
                    if (item == parent) continue;
                    ret = Max(dfs2(item, vtx), ret);
                }
                if (ret == -1) return -1;
                return ret + 1;
            };
            dfs3 = (vtx, parent) =>
            {
                var ret = -1L;
                if (vtx == u)
                {
                    return 0;
                }
                foreach (var item in path[vtx])
                {
                    if (item == parent) continue;
                    ret = Max(dfs3(item, vtx), ret);
                }
                if (ret == -1) return -1;
                return ret + 1;
            };
            dfs0(v, -1, 0);
            dfs1(u, -1, 0);
            Console.WriteLine(Max(dfs2(v, -1), dfs3(v, -1)) - 1);
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
