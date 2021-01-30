
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
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
            ref int sref = ref s[0];
            var ls = new bool[n];
            ref bool lsref = ref ls[0];
            var befores = Unsafe.Add(ref sref, n - 1);
            var beforels = Unsafe.Add(ref lsref, n - 1);
            for (var i = n - 2; i >= 0; --i)
            {
                var siref = Unsafe.Add(ref sref, i);
                beforels = Unsafe.Add(ref lsref, i) = siref == befores ? beforels : siref < befores;
                befores = siref;
            }
            var suml = new int[upper + 1];
            var sums = new int[upper + 1];
            ref int sumlref = ref suml[0];
            ref int sumsref = ref sums[0];
            for (var i = 0; i < n; ++i)
            {
                if (!Unsafe.Add(ref lsref, i)) ++Unsafe.Add(ref sumsref, Unsafe.Add(ref sref, i));
                else ++Unsafe.Add(ref sumlref, Unsafe.Add(ref sref, i) + 1);
            }
            for (var i = 0; i <= upper; ++i)
            {
                ref int sumsiref = ref Unsafe.Add(ref sumsref, i);
                sumsiref += Unsafe.Add(ref sumlref, i);
                if (i < upper) Unsafe.Add(ref sumlref, i + 1) += sumsiref;
            }
            var sa = new int[n];
            ref int saref = ref sa[0];
            void induce(ref int plmsref, int len, ref int sref, ref bool lsref, ref int sumsref, ref int sumlref, ref int saref)
            {
                Unsafe.InitBlock(ref Unsafe.As<int, byte>(ref saref), 255, (uint)(4 * n));
                var buf = new int[upper + 1];
                ref int bufref = ref buf[0];
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref bufref), ref Unsafe.As<int, byte>(ref sumsref), (uint)(4 * (upper + 1)));
                for (var i = 0; i < len; ++i)
                {
                    var d = Unsafe.Add(ref plmsref, i);
                    if (d == n) continue;
                    Unsafe.Add(ref saref, Unsafe.Add(ref bufref, Unsafe.Add(ref sref, d))++) = d;
                }
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref bufref), ref Unsafe.As<int, byte>(ref sumlref), (uint)(4 * (upper + 1)));
                Unsafe.Add(ref saref, Unsafe.Add(ref bufref, Unsafe.Add(ref sref, n - 1))++) = n - 1;
                for (var i = 0; i < n; ++i)
                {
                    var v = Unsafe.Add(ref saref, i);
                    if (v >= 1 && !Unsafe.Add(ref lsref, v - 1)) Unsafe.Add(ref saref, Unsafe.Add(ref bufref, Unsafe.Add(ref sref, v - 1))++) = v - 1;
                }
                Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref bufref), ref Unsafe.As<int, byte>(ref sumlref), (uint)(4 * (upper + 1)));
                for (var i = n - 1; i >= 0; --i)
                {
                    var v = Unsafe.Add(ref saref, i);
                    if (v >= 1 && Unsafe.Add(ref lsref, v - 1)) Unsafe.Add(ref saref, --Unsafe.Add(ref bufref, Unsafe.Add(ref sref, v - 1) + 1)) = v - 1;
                }
            }
            var lmsMap = new int[n + 1];
            ref int lmsMapref = ref lmsMap[0];
            Unsafe.Add(ref lmsMapref, 0) = Unsafe.Add(ref lmsMapref, n) = -1;
            var m = 0;
            var beforelsi = Unsafe.Add(ref lsref, 0);
            for (var i = 1; i < n; ++i)
            {
                var lsi = Unsafe.Add(ref lsref, i);
                if (!beforelsi && lsi) Unsafe.Add(ref lmsMapref, i) = m++;
                else Unsafe.Add(ref lmsMapref, i) = -1;
                beforelsi = lsi;
            }
            if (m == 0) induce(ref lmsMapref, 0, ref sref, ref lsref, ref sumsref, ref sumlref, ref saref);
            else
            {
                var lms = new int[m];
                ref int lmsref = ref lms[0];
                var lmsIdx = -1;
                beforelsi = Unsafe.Add(ref lsref, 0);
                for (var i = 1; i < n; ++i)
                {
                    var lsi = Unsafe.Add(ref lsref, i);
                    if (!beforelsi && lsi) Unsafe.Add(ref lmsref, ++lmsIdx) = i;
                    beforelsi = lsi;
                }
                induce(ref lmsref, lmsIdx + 1, ref sref, ref lsref, ref sumsref, ref sumlref, ref saref);
                var sortedLms = new int[m];
                ref int sortedLmsref = ref sortedLms[0];
                var sortedLmsIdx = -1;
                for (var i = 0; i < n; ++i)
                {
                    var v = Unsafe.Add(ref saref, i);
                    if (Unsafe.Add(ref lmsMapref, v) != -1) Unsafe.Add(ref sortedLmsref, ++sortedLmsIdx) = v;
                }
                var recs = new int[m];
                ref int recsref = ref recs[0];
                var recUpper = 0;
                var beforeSortedLms = sortedLmsref;
                for (var i = 1; i < m; ++i)
                {
                    var l = beforeSortedLms;
                    var sortedLmsi = Unsafe.Add(ref sortedLmsref, i);
                    var r = sortedLmsi;
                    var lmsMapl = Unsafe.Add(ref lmsMapref, l);
                    var lmsMapr = Unsafe.Add(ref lmsMapref, r);
                    var endl = lmsMapl + 1 < m ? Unsafe.Add(ref lmsref, lmsMapl + 1) : n;
                    var endr = lmsMapr + 1 < m ? Unsafe.Add(ref lmsref, lmsMapr + 1) : n;
                    var same = true;
                    if (endl - l != endr - r) same = false;
                    else
                    {
                        while (l < endl)
                        {
                            if (Unsafe.Add(ref sref, l) != Unsafe.Add(ref sref, r)) break;
                            ++l; ++r;
                        }
                        if (l == n || Unsafe.Add(ref sref, l) != Unsafe.Add(ref sref, r)) same = false;
                    }
                    if (!same) ++recUpper;
                    Unsafe.Add(ref recsref, Unsafe.Add(ref lmsMapref, sortedLmsi)) = recUpper;
                    beforeSortedLms = sortedLmsi;
                }
                var recsa = SAIS(recs, recUpper);
                ref int recsaref = ref recsa[0];
                for (var i = 0; i < m; ++i) Unsafe.Add(ref sortedLmsref, i) = Unsafe.Add(ref lmsref, Unsafe.Add(ref recsaref, i));
                induce(ref sortedLmsref, sortedLmsIdx + 1, ref sref, ref lsref, ref sumsref, ref sumlref, ref saref);
            }
            return sa;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] SuffixArray(long[] ary)
        {
            var len = ary.Length;
            if (len == 0) return new int[0];
            var idx = new int[len];
            var s2 = new int[len];
            ref int idxref = ref idx[0];
            ref long aryref = ref ary[0];
            for (var i = 0; i < len; ++i) Unsafe.Add(ref idxref, i) = i;
            Array.Sort(idx, (x, y) => ary[x].CompareTo(ary[y]));
            var now = 0;
            s2[idxref] = now;
            for (var i = 1; i < len; ++i)
            {
                ref int thisref = ref Unsafe.Add(ref idxref, i);
                if (Unsafe.Add(ref aryref, thisref) != Unsafe.Add(ref aryref, idxref)) ++now;
                idxref = ref thisref;
            }
            return SAIS(s2, now);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int[] SuffixArray(string s)
        {
            var s2 = new int[s.Length];
            for (var i = 0; i < s.Length; ++i) s2[i] = (int)s[i];
            return SAIS(s2, 255);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long[] LCPArray(int[] ary, int[] sa)
        {
            var len = ary.Length;
            if (len == 0) return new long[0];
            ref int aryref = ref ary[0];
            Span<int> rnk = stackalloc int[len];
            ref int rnkref = ref rnk[0];
            ref int saref = ref sa[0];
            for (var i = 0; i < len; ++i) Unsafe.Add(ref rnkref, Unsafe.Add(ref saref, i)) = i;
            var lcp = new long[len - 1];
            ref long lcpref = ref lcp[0];
            var h = 0;
            for (var i = 0; i < len; ++i)
            {
                if (h > 0) --h;
                var rnki = Unsafe.Add(ref rnkref, i);
                if (rnki == 0) continue;
                var j = Unsafe.Add(ref saref, rnki - 1);
                for (int ih = i + h, jh = j + h; jh < len && ih < len; ++ih, ++jh, ++h)
                {
                    if (Unsafe.Add(ref aryref, jh) != Unsafe.Add(ref aryref, ih)) break;
                }
                Unsafe.Add(ref lcpref, rnki - 1) = h;
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
            for (var i = 0; i < s.Length; ++i) s2[i] = (int)s[i];
            return LCPArray(s2, suffixArray);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long[] LCPArray(string s)
        {
            var s2 = new int[s.Length];
            for (var i = 0; i < s.Length; ++i) s2[i] = (int)s[i];
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
            for (var i = 0; i < s.Length; ++i)
            {
                state = ((state << 1) | 1) & mask[s[i]];
                if ((state & finish) == finish) ret.Add(i - t.Length + 1);
            }
            return ret;
        }
        /// <summary>
        /// 編集距離。レーベンシュタイン距離。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EditLength(string s, string t) => EditLength(s.Select(e => (long)e).ToArray(), t.Select(e => (long)e).ToArray());
        /// <summary>
        /// 編集距離。レーベンシュタイン距離。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EditLength(long[] s, long[] t)
        {
            var dp = new long[s.Length + 1, t.Length + 1];
            for (var i = 1; i <= s.Length; ++i) dp[i, 0] = i;
            for (var j = 1; j <= t.Length; ++j) dp[0, j] = j;
            for (var i = 1; i <= s.Length; ++i)
            {
                for (var j = 1; j <= t.Length; ++j)
                {
                    dp[i, j] = Min(dp[i - 1, j], Min(dp[i, j - 1], dp[i - 1, j - 1])) + 1;
                    if (s[i - 1] == t[j - 1]) dp[i, j] = Min(dp[i, j], dp[i - 1, j - 1]);
                }
            }
            return dp[s.Length, t.Length];
        }
    }
    ////end
}