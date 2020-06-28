using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Library
{
    ////start
    /// copy key class LIB_StirlingSecond
    partial struct /* not copy key */ LIB_Mod
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod[] LIB_StirlingSecond(long n)
        {
            var a = new LIB_Mod[n + 1];
            for (var i = 0; i <= n; i++) a[i] = (i % 2 == 1 ? -1 : 1) / Perm(i, i);
            var b = new LIB_Mod[n + 1];
            for (var i = 0; i <= n; i++) b[i] = Pow(i, n) / Perm(i, i);
            return LIB_NTT.Multiply(a.Select(e => (long)e).ToArray(), b.Select(e => (long)e).ToArray(), _mod).Take((int)n + 1).Select(e => (LIB_Mod)e).ToArray();
        }
    }
    ////end
}