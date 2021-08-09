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
    partial struct /* not copy key */ LIB_Mod1000000007
    {
        /// <summary>
        /// S(n, 0), S(n, 1), S(n, 2) ... S(n, k)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod1000000007[] LIB_StirlingSecond(long n, long k)
        {
            var a = new LIB_Mod1000000007[k + 1];
            for (var i = 0; i <= k; i++) a[i] = (i % 2 == 1 ? -1 : 1) / Perm(i, i);
            var b = new LIB_Mod1000000007[k + 1];
            for (var i = 0; i <= k; i++) b[i] = Pow(i, n) / Perm(i, i);
            return LIB_NTT.Multiply(a.Select(e => (long)e).ToArray(), b.Select(e => (long)e).ToArray(), _mod).Take((int)k + 1).Select(e => (LIB_Mod1000000007)e).ToArray();
        }
    }
    partial struct /* not copy key */ LIB_Mod998244353
    {
        /// <summary>
        /// S(n, 0), S(n, 1), S(n, 2) ... S(n, k)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public LIB_Mod998244353[] LIB_StirlingSecond(long n, long k)
        {
            var a = new LIB_Mod998244353[k + 1];
            for (var i = 0; i <= k; i++) a[i] = (i % 2 == 1 ? -1 : 1) / Perm(i, i);
            var b = new LIB_Mod998244353[k + 1];
            for (var i = 0; i <= k; i++) b[i] = Pow(i, n) / Perm(i, i);
            return LIB_NTT.Multiply(a.Select(e => (long)e).ToArray(), b.Select(e => (long)e).ToArray()).Take((int)k + 1).Select(e => (LIB_Mod998244353)e).ToArray();
        }
    }
    ////end
}