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
    class LIB_Common
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public List<Tuple<int, T>> RunLength<T>(IEnumerable<T> l)
        {
            T before = default(T);
            var cnt = 0;
            var ret = new List<Tuple<int, T>>();
            foreach (var item in l)
            {
                if (!before.Equals(item))
                {
                    if (cnt != 0)
                    {
                        ret.Add(Tuple.Create(cnt, before));
                        cnt = 0;
                    }
                }
                before = item;
                ++cnt;
            }
            if (cnt != 0) ret.Add(Tuple.Create(cnt, before));
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public List<T> LCS<T>(T[] s, T[] t) where T : IEquatable<T>
        {
            int sl = s.Length, tl = t.Length;
            var dp = new int[sl + 1, tl + 1];
            for (var i = 0; i < sl; i++)
            {
                for (var j = 0; j < tl; j++)
                {
                    dp[i + 1, j + 1] = s[i].Equals(t[j]) ? dp[i, j] + 1 : Max(dp[i + 1, j], dp[i, j + 1]);
                }
            }
            {
                var r = new List<T>();
                int i = sl, j = tl;
                while (i > 0 && j > 0)
                {
                    if (s[--i].Equals(t[--j])) r.Add(s[i]);
                    else if (dp[i, j + 1] > dp[i + 1, j]) ++j;
                    else ++i;
                }
                r.Reverse();
                return r;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long LIS<T>(T[] array, bool strict)
        {
            var l = new List<T>();
            foreach (var e in array)
            {
                var i = l.BinarySearch(e);
                if (i < 0) i = ~i;
                else if (!strict) ++i;
                if (i == l.Count) l.Add(e);
                else l[i] = e;
            }
            return l.Count;
        }
    }
    ////end
}