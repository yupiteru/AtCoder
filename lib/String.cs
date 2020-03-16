
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