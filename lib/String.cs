
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
    class LIB_String
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] ZAlgorithm(long[] s)
        {
            var ret = new int[s.Length];
            ret[0] = s.Length;
            int i = 1, j = 0;
            while (i < s.Length)
            {
                while (i + j < s.Length && s[j] == s[i + j]) ++j;
                ret[i] = j;
                if (j == 0) { ++i; continue; }
                var k = 1;
                while (i + k < s.Length && k + ret[k] < j) { ret[i + k] = ret[k]; ++k; }
                i += k; j -= k;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int[] SAIS(int[] s, int upper)
        {
            var n = s.Length;
            if (n == 0) return new int[0];
            if (n == 1) return new[] { 0 };
            if (n == 2)
            {
                if (s[0] < s[1]) return new[] { 0, 1 };
                return new[] { 1, 0 };
            }
            var ls = new bool[n];
            for (var i = n - 2; i >= 0; --i)
            {
                ls[i] = s[i] == s[i + 1] ? ls[i + 1] : s[i] < s[i + 1];
            }
            var suml = new int[upper + 1];
            var sums = new int[upper + 1];
            for (var i = 0; i < n; ++i)
            {
                if (!ls[i]) ++sums[s[i]];
                else ++suml[s[i] + 1];
            }
            for (var i = 0; i <= upper; ++i)
            {
                sums[i] += suml[i];
                if (i < upper) suml[i + 1] += sums[i];
            }
            var sa = new int[n];
            Action<int[], int> induce = (plms, len) =>
            {
                for (var i = 0; i < sa.Length; i++) sa[i] = -1;
                var buf = sums.ToArray();
                for (var i = 0; i < len; i++)
                {
                    var d = plms[i];
                    if (d == n) continue;
                    sa[buf[s[d]]++] = d;
                }
                for (var i = 0; i < buf.Length; i++) buf[i] = suml[i];
                sa[buf[s[n - 1]]++] = n - 1;
                for (var i = 0; i < n; ++i)
                {
                    var v = sa[i];
                    if (v >= 1 && !ls[v - 1]) sa[buf[s[v - 1]]++] = v - 1;
                }
                for (var i = 0; i < buf.Length; i++) buf[i] = suml[i];
                for (var i = n - 1; i >= 0; --i)
                {
                    var v = sa[i];
                    if (v >= 1 && ls[v - 1]) sa[--buf[s[v - 1] + 1]] = v - 1;
                }
            };
            var lmsMap = new int[n + 1];
            lmsMap[0] = lmsMap[n] = -1;
            var m = 0;
            for (var i = 1; i < n; i++)
            {
                if (!ls[i - 1] && ls[i]) lmsMap[i] = m++;
                else lmsMap[i] = -1;
            }
            var lms = new int[m];
            var lmsIdx = -1;
            for (var i = 1; i < n; ++i)
            {
                if (!ls[i - 1] && ls[i]) lms[++lmsIdx] = i;
            }
            induce(lms, lmsIdx + 1);
            if (m > 0)
            {
                var sortedLms = new int[m];
                var sortedLmsIdx = -1;
                foreach (var v in sa)
                {
                    if (lmsMap[v] != -1) sortedLms[++sortedLmsIdx] = v;
                }
                var recs = new int[m];
                var recUpper = 0;
                recs[lmsMap[sortedLms[0]]] = 0;
                for (var i = 1; i < m; ++i)
                {
                    var l = sortedLms[i - 1];
                    var r = sortedLms[i];
                    var endl = lmsMap[l] + 1 < m ? lms[lmsMap[l] + 1] : n;
                    var endr = lmsMap[r] + 1 < m ? lms[lmsMap[r] + 1] : n;
                    var same = true;
                    if (endl - l != endr - r) same = false;
                    else
                    {
                        while (l < endl)
                        {
                            if (s[l] != s[r]) break;
                            ++l; ++r;
                        }
                        if (l == n || s[l] != s[r]) same = false;
                    }
                    if (!same) ++recUpper;
                    recs[lmsMap[sortedLms[i]]] = recUpper;
                }

                var recsa = SAIS(recs, recUpper);
                for (var i = 0; i < m; ++i) sortedLms[i] = lms[recsa[i]];
                induce(sortedLms, sortedLmsIdx + 1);
            }
            return sa;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] SuffixArray(long[] ary)
        {
            var idx = new int[ary.Length];
            var s2 = new int[ary.Length];
            for (var i = 0; i < idx.Length; i++) idx[i] = i;
            Array.Sort(idx, (x, y) => ary[x].CompareTo(ary[y]));
            var now = 0;
            s2[idx[0]] = now;
            for (var i = 1; i < idx.Length; ++i)
            {
                if (ary[idx[i - 1]] != ary[idx[i]]) ++now;
            }
            return SAIS(s2, now);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] SuffixArray(string s)
        {
            var s2 = new int[s.Length];
            for (var i = 0; i < s.Length; i++) s2[i] = (int)s[i];
            return SAIS(s2, 255);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long[] LCPArray(int[] ary, int[] sa)
        {
            var rnk = new int[ary.Length];
            for (var i = 0; i < rnk.Length; ++i) rnk[sa[i]] = i;
            var lcp = new long[ary.Length - 1];
            var h = 0;
            for (var i = 0; i < rnk.Length; ++i)
            {
                if (h > 0) --h;
                if (rnk[i] == 0) continue;
                var j = sa[rnk[i] - 1];
                for (; j + h < ary.Length && i + h < ary.Length; ++h)
                {
                    if (ary[j + h] != ary[i + h]) break;
                }
                lcp[rnk[i] - 1] = h;
            }
            return lcp;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] LCPArray(long[] ary, int[] suffixArray) => LCPArray(ary.Select(e => (int)e).ToArray(), suffixArray);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] LCPArray(long[] ary) => LCPArray(ary.Select(e => (int)e).ToArray(), SuffixArray(ary));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] LCPArray(string s, int[] suffixArray)
        {
            var s2 = new int[s.Length];
            for (var i = 0; i < s.Length; i++) s2[i] = (int)s[i];
            return LCPArray(s2, suffixArray);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] LCPArray(string s)
        {
            var s2 = new int[s.Length];
            for (var i = 0; i < s.Length; i++) s2[i] = (int)s[i];
            var sa = SAIS(s2, 255);
            return LCPArray(s2, sa);
        }
        /// <summary>
        /// 入力の各要素は0以上。-1の要素は任意と一致。
        /// マッチした位置の1文字目の位置を0-indexedで返す
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<int> ShiftAnd(long[] s, long[] t)
        {
            var finish = new LIB_Bitset(t.Length);
            finish[t.Length - 1] = true;
            var mask = s.Where(e => e >= 0).Distinct().ToDictionary(e => e, _ => new LIB_Bitset(t.Length));
            var fil = new LIB_Bitset(t.Length);
            mask[-1] = ~fil;
            fil |= 1;
            foreach (var item in t)
            {
                if (item >= 0 && mask.ContainsKey(item)) mask[item] |= fil;
                if (item == -1) foreach (var item2 in mask.Keys.ToArray()) mask[item2] |= fil;
                fil <<= 1;
            }
            var state = new LIB_Bitset(s.Length);
            var ret = new List<int>();
            for (var i = 0; i < s.Length; i++)
            {
                state = ((state << 1) | 1) & mask[s[i]];
                if ((state & finish) == finish) ret.Add(i - t.Length + 1);
            }
            return ret;
        }
    }
    ////end
}