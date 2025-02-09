using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Library;

namespace Program
{
    using static Library.LIB_Static;
    public static class template
    {
        static bool SAIKI = false;
        static public int numberOfRandomCases = 0;
        static public void MakeTestCase(List<string> _input, List<string> _output, ref Func<string[], bool> _outputChecker)
        {
        }
        static public void Solve()
        {
        }
        class Printer : StreamWriter
        {
            public override IFormatProvider FormatProvider { get { return CultureInfo.InvariantCulture; } }
            public Printer(Stream stream) : base(stream, new UTF8Encoding(false, true)) { base.AutoFlush = false; }
            public Printer(Stream stream, Encoding encoding) : base(stream, encoding) { base.AutoFlush = false; }
        }
        static LIB_FastIO fastio = new LIB_FastIODebug();
        static string[] args;
        static public void Main(string[] args_t) { args = args_t; if (args_t.Length == 0) { fastio = new LIB_FastIO(); Console.SetOut(new Printer(Console.OpenStandardOutput())); } if (SAIKI) { var t = new Thread(Solve, 134217728); t.Start(); t.Join(); } else Solve(); Console.Out.Flush(); }
        static long NN => fastio.Long();
        static double ND => fastio.Double();
        static string NS => fastio.Scan();
        static long[] NNList(long N) => Repeat(0, N).Select(_ => NN).ToArray();
        static double[] NDList(long N) => Repeat(0, N).Select(_ => ND).ToArray();
        static string[] NSList(long N) => Repeat(0, N).Select(_ => NS).ToArray();
    }
}
