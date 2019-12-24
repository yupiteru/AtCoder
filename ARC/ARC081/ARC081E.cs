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
    public static class ARC081E
    {
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var A = NS;
            var toList = new int[26];
            for (var i = 0; i < 26; i++)
            {
                toList[i] = A.Length;
            }
            var list = new Tuple<int, int, int>[A.Length + 1];
            list[A.Length] = Tuple.Create(-1, -1, 0);
            for (var i = A.Length - 1; i >= 0; i--)
            {
                var min = A.Length + 1;
                var minL = -1;
                var minJ = -1;
                for (var j = 0; j < 26; j++)
                {
                    if (min > list[toList[j]].Item3)
                    {
                        min = list[toList[j]].Item3;
                        minL = toList[j];
                        minJ = j;
                    }
                }
                list[i] = Tuple.Create(minL, minJ, min + 1);
                toList[A[i] - 'a'] = i;
            }
            {
                var min = A.Length + 1;
                var minL = -1;
                var minJ = -1;
                for (var j = 0; j < 26; j++)
                {
                    if (min > list[toList[j]].Item3)
                    {
                        min = list[toList[j]].Item3;
                        minL = toList[j];
                        minJ = j;
                    }
                }
                var ans = new List<int>();
                var tmp = minL;
                ans.Add(minJ);
                while (tmp != A.Length)
                {
                    ans.Add(list[tmp].Item2);
                    tmp = list[tmp].Item1;
                }
                Console.WriteLine(new string(ans.Select(e => (char)(e + 'a')).ToArray()));
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
