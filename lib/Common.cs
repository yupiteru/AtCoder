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
        static public (int[] result, List<T> dp) LIS<T>(IEnumerable<T> array, bool strict) where T : IComparable<T>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public (long k, T val) SanbunTansaku<T>(long left, long right, Func<long, T> fun) where T : IComparable<T>
        {
            var size = right - left;
            var fib = new LIB_Deque<long>();
            fib.PushBack(1);
            fib.PushBack(1);
            while (fib[fib.Count - 1] * 2 + fib[fib.Count - 2] <= size)
            {
                fib.PushBack(fib[fib.Count - 1] + fib[fib.Count - 2]);
            }
            var valueLeft = left;
            var valueRight = right;
            Func<long, (bool isMax, T value)> calc = x =>
            {
                if (x < valueLeft || valueRight <= x) return (true, default(T));
                return (false, fun(x));
            };
            --left;
            var lv = calc(left);
            var m1 = left + fib.PopBack();
            var m2 = m1 + fib.Back;
            var m1v = calc(m1);
            var m2v = calc(m2);
            var rv = calc(right = m1 - left + m1 + fib.Back);
            while (fib.Count > 2)
            {
                if (m1v.CompareTo(m2v) < 0)
                {
                    rv = m2v; right = m2;
                    m2v = m1v; m2 = m1;
                    m1v = calc(m1 = left + fib.PopBack());
                }
                else
                {
                    lv = m1v; left = m1;
                    m1v = m2v; m1 = m2;
                    m2v = calc(m2 = left + fib.PopBack() + fib.Back);
                }
            }
            var ans = calc(left);
            for (var i = left + 1; i < right; ++i)
            {
                var na = calc(i);
                if (ans.CompareTo(na) > 0)
                {
                    ans = na;
                    left = i;
                }
            }
            return (left, ans.value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public (double k, T val) SanbunTansaku<T>(double left, double right, Func<double, T> fun) where T : IComparable<T>
        {
            const double PHI = 0.6180339887;
            var lv = fun(left);
            var rv = fun(right);
            var m2 = left + (right - left) * PHI;
            var m2v = fun(m2);
            var m1 = left + (m2 - left) * PHI;
            var m1v = fun(m1);
            for (var i = 0; i < 200; ++i)
            {
                if (m1v.CompareTo(m2v) < 0)
                {
                    rv = m2v; right = m2;
                    m2v = m1v; m2 = m1;
                    m1v = fun(m1 = left + (m2 - left) * PHI);
                }
                else
                {
                    lv = m1v; left = m1;
                    m1v = m2v; m1 = m2;
                    m2v = fun(m2 = m1 + (m1 - left) * PHI);
                }
            }
            return (m1, m1v);
        }
    }
    ////end
}