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
        public struct RunLengthResult<T>
        {
            public long count;
            public T value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public List<RunLengthResult<T>> RunLength<T>(IEnumerable<T> l)
        {
            T before = default(T);
            var cnt = 0;
            var ret = new List<RunLengthResult<T>>();
            foreach (var item in l)
            {
                if (!before.Equals(item))
                {
                    if (cnt != 0)
                    {
                        ret.Add(new RunLengthResult<T> { count = cnt, value = before });
                        cnt = 0;
                    }
                }
                before = item;
                ++cnt;
            }
            if (cnt != 0) ret.Add(new RunLengthResult<T> { count = cnt, value = before });
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
        static public (int[] result, List<T> dp) LIS<T>(IEnumerable<T> array, bool strict) where T : IComparable
        {
            var l = new List<T>();
            var ret = new List<int>();
            foreach (var e in array)
            {
                var left = -1;
                var right = l.Count;
                while (right - left > 1)
                {
                    var mid = (right + left) / 2;
                    if (l[mid].CompareTo(e) < 0 || !strict && l[mid].CompareTo(e) == 0) left = mid;
                    else right = mid;
                }
                if (right == l.Count) l.Add(e);
                else l[right] = e;
                ret.Add(l.Count);
            }
            return (ret.ToArray(), l);
        }
    }
    ////end
}