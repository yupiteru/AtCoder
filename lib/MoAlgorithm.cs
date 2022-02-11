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
    class LIB_MoAlgorithm
    {
        struct Query
        {
            public int idx;
            public int l;
            public int r;
        }
        int maxL;
        int maxR;
        int minL;
        int minR;
        int queryNum;
        List<Query> queries;
        Action<int> add;
        Action<int> delete;
        Action<int> checker;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_MoAlgorithm()
        {
            maxL = maxR = int.MinValue;
            minL = minR = int.MaxValue;
            queries = new List<Query>();
            add = e => { };
            delete = e => { };
            checker = e => { };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddQuery(long L, long R)
        {
            var l = (int)L;
            var r = (int)R;
            if (maxL < l) maxL = l;
            if (minL > l) minL = l;
            if (maxR < r) maxR = r;
            if (minR > r) minR = r;
            queries.Add(new Query() { idx = queryNum, l = l, r = r });
            ++queryNum;
            return queryNum - 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 指定した 範囲index を区間に追加するメソッドを設定
        /// </summary>
        public LIB_MoAlgorithm SetAddMethod(Action<int> act)
        {
            add = act;
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 指定した 範囲index を区間から削除するメソッドを設定
        /// </summary>
        public LIB_MoAlgorithm SetDeleteMethod(Action<int> act)
        {
            delete = act;
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// 指定した クエリindex の答えを処理するメソッドを設定
        /// </summary>
        public LIB_MoAlgorithm SetChecker(Action<int> act)
        {
            checker = act;
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build()
        {
            Query[] sorted;
            if (maxL - minL > maxR - minR)
            {
                var blockSize = Max(1, (int)(1.7320508 * (maxR - minR + 1) / Sqrt(2 * queryNum)));
                sorted = queries.OrderBy(e => (e.r - minR + 1) / blockSize).ThenBy(e => (((e.r - minR + 1) / blockSize & 1) == 1) ? -e.l : e.l).ToArray();
            }
            else
            {
                var blockSize = Max(1, (int)(1.7320508 * (maxL - minL + 1) / Sqrt(2 * queryNum)));
                sorted = queries.OrderBy(e => (e.l - minL + 1) / blockSize).ThenBy(e => (((e.l - minL + 1) / blockSize & 1) == 1) ? -e.r : e.r).ToArray();
            }
            var lidx = 0;
            var ridx = 0;
            foreach (var item in sorted)
            {
                while (lidx < item.l)
                {
                    delete(lidx);
                    ++lidx;
                }
                while (item.l < lidx)
                {
                    --lidx;
                    add(lidx);
                }
                while (ridx < item.r)
                {
                    add(ridx);
                    ++ridx;
                }
                while (item.r < ridx)
                {
                    --ridx;
                    delete(ridx);
                }
                checker(item.idx);
            }
        }
    }
    ////end
}