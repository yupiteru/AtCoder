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
    public static class ABC166F
    {
        static bool SAIKI = false;
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
            var N = NN;
            var A = NN;
            var B = NN;
            var C = NN;
            var sList = NSList(N);
            var ans = new List<char>();
            var ok = true;
            var canFlipAB = false;
            var canFlipAC = false;
            var canFlipBC = false;
            foreach (var item in sList)
            {
                var ncanFlipAB = false;
                var ncanFlipAC = false;
                var ncanFlipBC = false;
                if (item == "AB")
                {
                    if (A == 0 && B == 0)
                    {
                        if (canFlipAC)
                        {
                            ans[ans.Count - 1] = 'A';
                            ++A;
                            --C;
                            ++A;
                            --C;
                        }
                        else if (canFlipBC)
                        {
                            ans[ans.Count - 1] = 'B';
                            ++B;
                            --C;
                            ++B;
                            --C;
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (A == B) ncanFlipAB = true;
                    if (A > B)
                    {
                        ans.Add('B');
                        --A;
                        ++B;
                    }
                    else
                    {
                        ans.Add('A');
                        ++A;
                        --B;
                    }
                }
                else if (item == "BC")
                {
                    if (B == 0 && C == 0)
                    {
                        if (canFlipAB)
                        {
                            ans[ans.Count - 1] = 'B';
                            ++B;
                            --A;
                            ++B;
                            --A;
                        }
                        else if (canFlipAC)
                        {
                            ans[ans.Count - 1] = 'C';
                            ++C;
                            --A;
                            ++C;
                            --A;
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (B == C) ncanFlipBC = true;
                    if (B > C)
                    {
                        ans.Add('C');
                        --B;
                        ++C;
                    }
                    else
                    {
                        ans.Add('B');
                        ++B;
                        --C;
                    }
                }
                else
                {
                    if (A == 0 && C == 0)
                    {
                        if (canFlipAB)
                        {
                            ans[ans.Count - 1] = 'A';
                            ++A;
                            ++A;
                            --B;
                            --B;
                        }
                        else if (canFlipBC)
                        {
                            ans[ans.Count - 1] = 'C';
                            ++C;
                            ++C;
                            --B;
                            --B;
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (A == C) ncanFlipAC = true;
                    if (A > C)
                    {
                        ans.Add('C');
                        --A;
                        ++C;
                    }
                    else
                    {
                        ans.Add('A');
                        ++A;
                        --C;
                    }
                }
                canFlipAB = ncanFlipAB;
                canFlipAC = ncanFlipAC;
                canFlipBC = ncanFlipBC;
            }
            if (ok)
            {
                Console.WriteLine("Yes");
                foreach (var item in ans)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("No");
            }
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static LIB_FastIO fastio = new LIB_FastIODebug();
        static public void Main(string[] args) { if (args.Length == 0) { fastio = new LIB_FastIO(); Console.SetOut(new Printer(Console.OpenStandardOutput())); } if (SAIKI) { var t = new Thread(Solve, 134217728); t.Start(); t.Join(); } else Solve(); Console.Out.Flush(); }
        static long NN => fastio.Long();
        static double ND => fastio.Double();
        static string NS => fastio.Scan();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
        static long Count<T>(this IEnumerable<T> x, Func<T, bool> pred) => Enumerable.Count(x, pred);
        static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        static IOrderedEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x, _ => xorshift);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderBy<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderBy(x.OrderByRand(), selector);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x) => Enumerable.OrderByDescending(x.OrderByRand(), e => e);
        static IOrderedEnumerable<T1> OrderByDescending<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderByDescending(x.OrderByRand(), selector);
        static IOrderedEnumerable<string> OrderBy(this IEnumerable<string> x) => x.OrderByRand().OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderBy(selector, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<string> OrderByDescending(this IEnumerable<string> x) => x.OrderByRand().OrderByDescending(e => e, StringComparer.OrdinalIgnoreCase);
        static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderByDescending(selector, StringComparer.OrdinalIgnoreCase);
        static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        static IEnumerator<uint> _xsi = _xsc();
        static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
    }
}
