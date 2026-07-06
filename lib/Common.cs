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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public long[] NoshiBasis(long[] elementList)
        {
            var basis = new List<long>();
            foreach (var item in elementList)
            {
                var e = item;
                foreach (var item2 in basis)
                {
                    var v = e ^ item2;
                    if (e > v) e = v;
                }
                if (e > 0)
                {
                    basis.Add(e);
                }
            }
            var ret = new List<long>();
            basis = basis.OrderByDescending(e => e).ToList();
            for (var i = 0; i < basis.Count; ++i)
            {
                var v = basis[i];
                for (var j = i + 1; j < basis.Count; ++j)
                {
                    var v2 = v ^ basis[j];
                    if (v > v2) v = v2;
                }
                ret.Add(v);
            }
            return ret.ToArray();
        }

        static public int[] MergeSort<T>(IList<T> list, int l, int r, Func<T, T, bool> leftIsSmall)
        {
            int Fill(int x)
            {
                --x;
                x |= x >> 1;
                x |= x >> 2;
                x |= x >> 4;
                x |= x >> 8;
                x |= x >> 16;
                return ++x;
            }

            var N = r - l;
            var ret = Enumerable.Range(0, N).ToArray();

            if (N == 1) return ret;

            {
                var i = 0;
                var j = (N + 1) / 2;
                while (j < N)
                {
                    if (!leftIsSmall(list[l + i], list[l + j]))
                    {
                        (list[l + i], list[l + j]) = (list[l + j], list[l + i]);
                        (ret[i], ret[j]) = (ret[j], ret[i]);
                    }
                    ++i; ++j;
                }
            }

            if (N == 2) return ret;

            {
                var perm = MergeSort(list, l + (N + 1) / 2, r, leftIsSmall);
                var inv = Enumerable.Range(0, N / 2).ToArray();
                var p = Enumerable.Range(0, N / 2).ToArray();
                for (var i = 0; i < N / 2; ++i)
                {
                    var j = inv[perm[i]];
                    if (i != j)
                    {
                        (list[l + i], list[l + j]) = (list[l + j], list[l + i]);
                        (ret[i], ret[j]) = (ret[j], ret[i]);
                        (ret[i + (N + 1) / 2], ret[j + (N + 1) / 2]) = (ret[j + (N + 1) / 2], ret[i + (N + 1) / 2]);
                        (p[i], p[j]) = (p[j], p[i]);
                        (inv[p[i]], inv[p[j]]) = (inv[p[j]], inv[p[i]]);
                    }
                }
            }
            {
                var inv = Enumerable.Range(0, N + 1).ToArray();
                var p = Enumerable.Range(0, N + 1).ToArray();
                void Swap(int x, int y)
                {
                    if (x == y) return;
                    (list[l + x], list[l + y]) = (list[l + y], list[l + x]);
                    (ret[x], ret[y]) = (ret[y], ret[x]);
                    (p[x], p[y]) = (p[y], p[x]);
                    (inv[p[x]], inv[p[y]]) = (inv[p[y]], inv[p[x]]);
                }
                void Rotate(int L, int R)
                {
                    for (var i = R; --i > L;) Swap(L, i);
                }
                void BinarySearch(int X, int L, int R)
                {
                    while (L + 1 < R)
                    {
                        var x = R - L;
                        var y = Fill(x);
                        var z = 3 * y / 4;
                        var M = L + (x < z ? y / 4 : x - y / 2);
                        if (leftIsSmall(list[l + M], list[l + X])) L = M;
                        else R = M;
                    }
                    Rotate(X, R);
                }
                Rotate(0, (N + 1) / 2);
                var L = 1;
                var R = Min(3, (N + 1) / 2);
                var C = 8;
                var now = 1;
                while (L < (N + 1) / 2)
                {
                    for (var i = R; i-- > L; ++now)
                    {
                        BinarySearch(i - L, (N + 1) / 2 - now - 1, inv[i + (N + 1) / 2]);
                    }
                    C *= 2;
                    L = R;
                    R = Min((C + 1) / 3, (N + 1) / 2);
                }
            }
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public KeyValuePair<Key, Value>[] KetaDP<Key, Value>(string N, Func<long, (Key, Value)> init, Func<long, Key, Value, (Key, Value)> forward, Func<Value, Value, Value> merge)
        {
            var dic1 = new Dictionary<Key, Value>();
            var dic0 = new Dictionary<Key, Value>();
            for (var i = N.Length - 1; i >= 0; --i)
            {
                var dic1List = dic1.ToArray();
                var dic0List = dic0.ToArray();
                dic1.Clear();
                dic0.Clear();
                var num = N[N.Length - i - 1] - '0';
                foreach (var item in dic1List)
                {
                    for (var j = 0; j < num; ++j)
                    {
                        Value val;
                        var nv = forward(j, item.Key, item.Value);
                        if (dic0.TryGetValue(nv.Item1, out val)) val = merge(val, nv.Item2);
                        else val = nv.Item2;
                        dic0[nv.Item1] = val;
                    }
                    {
                        Value val;
                        var nv = forward(num, item.Key, item.Value);
                        if (dic1.TryGetValue(nv.Item1, out val)) val = merge(val, nv.Item2);
                        else val = nv.Item2;
                        dic1[nv.Item1] = val;
                    }
                }
                foreach (var item in dic0List)
                {
                    for (var j = 0; j < 10; ++j)
                    {
                        Value val;
                        var nv = forward(j, item.Key, item.Value);
                        if (dic0.TryGetValue(nv.Item1, out val)) val = merge(val, nv.Item2);
                        else val = nv.Item2;
                        dic0[nv.Item1] = val;
                    }
                }
                for (var j = 0; j < 10; ++j)
                {
                    if (i == N.Length - 1)
                    {
                        if (0 < j && j < num)
                        {
                            Value val;
                            var iv = init(j);
                            if (dic0.TryGetValue(iv.Item1, out val)) val = merge(val, iv.Item2);
                            else val = iv.Item2;
                            dic0[iv.Item1] = val;
                        }
                        else if (j == num)
                        {
                            Value val;
                            var iv = init(j);
                            if (dic1.TryGetValue(iv.Item1, out val)) val = merge(val, iv.Item2);
                            else val = iv.Item2;
                            dic1[iv.Item1] = val;
                        }
                    }
                    else
                    {
                        if (0 < j)
                        {
                            Value val;
                            var iv = init(j);
                            if (dic0.TryGetValue(iv.Item1, out val)) val = merge(val, iv.Item2);
                            else val = iv.Item2;
                            dic0[iv.Item1] = val;
                        }
                    }
                }
            }
            return dic0.Concat(dic1).ToArray();
        }
    }
    ////end
}