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
    class LIB_RetroactivePriorityQueue<T>
    {
        // noshi91 さんの提出を参考にしました
        // URL：https://atcoder.jp/contests/abc363/submissions/55828915

        int n;
        int m;
        Comparison<T> comp;
        T[] poppedVal;
        T[] remainVal;
        List<T> insert;
        List<T> erase;
        (int sum, int min)[] ruisekiFlows;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RetroactivePriorityQueue(long maxOpes, T min, T max, Comparison<T> cmp, bool asc = true)
        {
            comp = asc ? cmp : (x, y) => cmp(y, x);
            n = (int)maxOpes;
            m = 1;
            while (m < n + 1) m <<= 1;
            poppedVal = new T[m * 2];
            remainVal = new T[m * 2];
            ruisekiFlows = new (int sum, int min)[m * 2];
            poppedVal.AsSpan().Fill(asc ? min : max);
            remainVal.AsSpan().Fill(asc ? max : min);
            insert = new List<T>();
            erase = new List<T>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RetroactivePriorityQueue(long maxOpes, T min, T max, bool asc = true) : this(maxOpes, min, max, Comparer<T>.Default.Compare, asc) { }
        void UpdateFlows(int i)
        {
            while ((i /= 2) > 0)
            {
                ruisekiFlows[i].sum = ruisekiFlows[i * 2].sum + ruisekiFlows[i * 2 + 1].sum;
                ruisekiFlows[i].min = Min(ruisekiFlows[i * 2].min, ruisekiFlows[i * 2].sum + ruisekiFlows[i * 2 + 1].min);
            }
        }
        void UpdatePoppedVal(int i)
        {
            while ((i /= 2) > 0)
            {
                if (comp(poppedVal[i * 2], poppedVal[i * 2 + 1]) < 0) poppedVal[i] = poppedVal[i * 2 + 1];
                else poppedVal[i] = poppedVal[i * 2];
            }
        }
        void UpdateRemainVal(int i)
        {
            while ((i /= 2) > 0)
            {
                if (comp(remainVal[i * 2], remainVal[i * 2 + 1]) < 0) remainVal[i] = remainVal[i * 2];
                else remainVal[i] = remainVal[i * 2 + 1];
            }
        }
        void IncrementalUpdate(int i)
        {
            // この時点で poppedVal[i] には最終的に pop される値が入っている

            ruisekiFlows[i].sum++;
            UpdateFlows(i);
            // ruisekiFlows[i] で流入するなら、poppedVal[i] の値は最終的に pop される

            var s = ruisekiFlows[1].sum - 1;
            var k = 1;
            while (k < m)
            {
                k <<= 1;
                if (ruisekiFlows[k].sum + ruisekiFlows[k + 1].min == s)
                {
                    s -= ruisekiFlows[k].sum;
                    ++k;
                }
            }
            // k := ruisekiFlows[i] で流したことで代わりに流入しなくなる可能性のある位置の端
            if (k == m)
            {
                // 代わりに流入しなくなる位置は無し
                // poppedVal[i] の値は pop されるままなので最終の列に変化は無い
                return;
            }

            // k 以降の poppedVal の最大値を探して、その位置のフローを流入しないように変更する
            //   → poppedVal の値は最終的に残るように変化
            var c = 0;
            var r = poppedVal.Length;
            while (k < r) // セグ木を上方向に探索
            {
                if ((k & 1) != 0)
                {
                    if (comp(poppedVal[c], poppedVal[k]) < 0) c = k;
                    ++k;
                }
                k >>= 1;
                r >>= 1;
            }
            while (c < m) // セグ木を下方向に探索
            {
                c <<= 1;
                if (comp(poppedVal[c], poppedVal[c + 1]) < 0) c++;
            }
            // c := poppedVal の最大値の位置
            // ruisekiFlows[c] で流入しないように変更する
            // poppedVal[c] の値は最終的に残るように変化する
            insert.Add(poppedVal[c]);
            ruisekiFlows[c].sum = 0;
            remainVal[c] = poppedVal[c];
            poppedVal[c] = poppedVal[0];
            UpdateFlows(c);
            UpdatePoppedVal(c);
            UpdateRemainVal(c);
        }
        void DecrementalUpdate(int i)
        {
            // ruisekiFlows[i] で流出するように変更する
            ruisekiFlows[i].sum--;
            UpdateFlows(i);

            var s = ruisekiFlows[1].sum;
            var k = 1;
            while (k < m)
            {
                k <<= 1;
                if (ruisekiFlows[k].min != s)
                {
                    s -= ruisekiFlows[k].sum;
                    ++k;
                }
            }
            // k := ruisekiFlows[i] で流出するようにしたことで、流入するようになる可能性のある位置の端
            // k 以降の remainVal の最小値を探して、その位置のフローを流入するように変更する
            //   → remainVal の値は最終的に pop されるように変化
            var c = 0;
            while (k > 0) // セグ木を上方向に探索
            {
                if ((k & 1) != 0)
                {
                    if (comp(remainVal[--k], remainVal[c]) < 0) c = k;
                }
                k >>= 1;
            }
            if (c == 0)
            {
                // k 以降に有効な remainVal が無い
                // 最終の列に変化は無い
                return;
            }
            while (c < m) // セグ木を下方向に探索
            {
                c <<= 1;
                if (comp(remainVal[c + 1], remainVal[c]) < 0) ++c;
            }

            // c := remainVal の最小値の位置
            // ruisekiFlows[c] で流入するように変更する
            // remainVal[c] の値は最終的に pop されるように変化する
            erase.Add(remainVal[c]);
            ruisekiFlows[c].sum = 1;
            poppedVal[c] = remainVal[c];
            remainVal[c] = remainVal[0];
            UpdateFlows(c);
            UpdatePoppedVal(c);
            UpdateRemainVal(c);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) SetPush(long i, T val)
        {
            SetNoOp(i);
            i += m;
            poppedVal[i] = val;
            UpdatePoppedVal((int)i);
            IncrementalUpdate((int)i);
            return (insert.ToArray(), erase.ToArray());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) SetPop(long i)
        {
            SetNoOp(i);
            i += m;
            DecrementalUpdate((int)i);
            return (insert.ToArray(), erase.ToArray());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) SetNoOp(long i)
        {
            insert.Clear();
            erase.Clear();
            i += m;
            if (ruisekiFlows[i].sum == -1)
            {
                IncrementalUpdate((int)i);
            }
            else if (comp(remainVal[i], remainVal[0]) != 0)
            {
                erase.Add(remainVal[i]);
                remainVal[i] = remainVal[0];
                UpdateRemainVal((int)i);
            }
            else if (comp(poppedVal[i], poppedVal[0]) != 0)
            {
                poppedVal[i] = poppedVal[0];
                UpdatePoppedVal((int)i);
                DecrementalUpdate((int)i);
            }
            return (insert.ToArray(), erase.ToArray());
        }
    }
    class LIB_RetroactivePriorityQueue2<T>
    {
        // noshi91 さんの提出を参考にしました
        // URL：https://atcoder.jp/contests/abc363/submissions/55828915

        int n;
        int m;
        Comparison<T> comp;
        T[] poppedVal;
        T[] remainVal;
        LIB_RedBlackTree<T>[] poppedQueue;
        LIB_RedBlackTree<T>[] remainQueue;
        List<T> insert;
        List<T> erase;
        (int sum, int min)[] ruisekiFlows;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RetroactivePriorityQueue2(long maxOpes, T min, T max, Comparison<T> cmp, bool asc = true)
        {
            comp = asc ? cmp : (x, y) => cmp(y, x);
            n = (int)maxOpes;
            m = 1;
            while (m < n + 1) m <<= 1;
            poppedVal = new T[m * 2];
            remainVal = new T[m * 2];
            poppedQueue = Enumerable.Repeat(0, m).Select(_ => new LIB_RedBlackTree<T>((x, y) => comp(y, x))).ToArray();
            remainQueue = Enumerable.Repeat(0, m).Select(_ => new LIB_RedBlackTree<T>(comp)).ToArray();
            ruisekiFlows = new (int sum, int min)[m * 2];
            poppedVal.AsSpan().Fill(asc ? min : max);
            remainVal.AsSpan().Fill(asc ? max : min);
            insert = new List<T>();
            erase = new List<T>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RetroactivePriorityQueue2(long maxOpes, T min, T max, bool asc = true) : this(maxOpes, min, max, Comparer<T>.Default.Compare, asc) { }
        void UpdateFlows(int i)
        {
            while ((i /= 2) > 0)
            {
                ruisekiFlows[i].sum = ruisekiFlows[i * 2].sum + ruisekiFlows[i * 2 + 1].sum;
                ruisekiFlows[i].min = Min(ruisekiFlows[i * 2].min, ruisekiFlows[i * 2].sum + ruisekiFlows[i * 2 + 1].min);
            }
        }
        void UpdatePoppedVal(int i)
        {
            while ((i /= 2) > 0)
            {
                if (comp(poppedVal[i * 2], poppedVal[i * 2 + 1]) < 0) poppedVal[i] = poppedVal[i * 2 + 1];
                else poppedVal[i] = poppedVal[i * 2];
            }
        }
        void UpdateRemainVal(int i)
        {
            while ((i /= 2) > 0)
            {
                if (comp(remainVal[i * 2], remainVal[i * 2 + 1]) < 0) remainVal[i] = remainVal[i * 2];
                else remainVal[i] = remainVal[i * 2 + 1];
            }
        }
        void IncrementalUpdate(int i)
        {
            // この時点で poppedVal[i] には最終的に pop される値が入っている

            ruisekiFlows[i].sum++;
            UpdateFlows(i);
            // ruisekiFlows[i] で流入するなら、poppedVal[i] の値は最終的に pop される

            var s = ruisekiFlows[1].sum - 1;
            var k = 1;
            while (k < m)
            {
                k <<= 1;
                if (ruisekiFlows[k].sum + ruisekiFlows[k + 1].min == s)
                {
                    s -= ruisekiFlows[k].sum;
                    ++k;
                }
            }
            // k := ruisekiFlows[i] で流したことで代わりに流入しなくなる可能性のある位置の端
            if (k == m)
            {
                // 代わりに流入しなくなる位置は無し
                // poppedVal[i] の値は pop されるままなので最終の列に変化は無い
                return;
            }

            // k 以前の poppedVal の最大値を探して、その位置のフローを流入しないように変更する
            //   → poppedVal の値は最終的に残るように変化
            var c = 0;
            var r = poppedVal.Length;
            while (k < r) // セグ木を上方向に探索
            {
                if ((k & 1) != 0)
                {
                    if (comp(poppedVal[c], poppedVal[k]) < 0) c = k;
                    ++k;
                }
                k >>= 1;
                r >>= 1;
            }
            while (c < m) // セグ木を下方向に探索
            {
                c <<= 1;
                if (comp(poppedVal[c], poppedVal[c + 1]) < 0) c++;
            }
            // c := poppedVal の最大値の位置
            // ruisekiFlows[c] で流入しないように変更する
            // poppedVal[c] の値は最終的に残るように変化する
            insert.Add(poppedVal[c]);
            ruisekiFlows[c].sum--;
            remainQueue[c - m].Add(poppedVal[c]);
            remainVal[c] = remainQueue[c - m].Min();
            poppedQueue[c - m].RemoveAt(0);
            poppedVal[c] = poppedQueue[c - m].Count > 0 ? poppedQueue[c - m].Min() : poppedVal[0];
            UpdateFlows(c);
            UpdatePoppedVal(c);
            UpdateRemainVal(c);
        }
        void DecrementalUpdate(int i)
        {
            // ruisekiFlows[i] で流出するように変更する
            ruisekiFlows[i].sum--;
            UpdateFlows(i);

            var s = ruisekiFlows[1].sum;
            var k = 1;
            while (k < m)
            {
                k <<= 1;
                if (ruisekiFlows[k].min != s)
                {
                    s -= ruisekiFlows[k].sum;
                    ++k;
                }
            }
            // k := ruisekiFlows[i] で流出するようにしたことで、流入するようになる可能性のある位置の端
            // k 以前の remainVal の最小値を探して、その位置のフローを流入するように変更する
            //   → remainVal の値は最終的に pop されるように変化
            var c = 0;
            while (k > 0) // セグ木を上方向に探索
            {
                if ((k & 1) != 0)
                {
                    if (comp(remainVal[--k], remainVal[c]) < 0) c = k;
                }
                k >>= 1;
            }
            if (c == 0)
            {
                // k 以降に有効な remainVal が無い
                // 最終の列に変化は無い
                return;
            }
            while (c < m) // セグ木を下方向に探索
            {
                c <<= 1;
                if (comp(remainVal[c + 1], remainVal[c]) < 0) ++c;
            }

            // c := remainVal の最小値の位置
            // ruisekiFlows[c] で流入するように変更する
            // remainVal[c] の値は最終的に pop されるように変化する
            erase.Add(remainVal[c]);
            ruisekiFlows[c].sum++;

            poppedQueue[c - m].Add(remainVal[c]);
            poppedVal[c] = poppedQueue[c - m].Min();
            remainQueue[c - m].RemoveAt(0);
            remainVal[c] = remainQueue[c - m].Count > 0 ? remainQueue[c - m].Min() : remainVal[0];
            UpdateFlows(c);
            UpdatePoppedVal(c);
            UpdateRemainVal(c);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) SetPush(long i, T val)
        {
            insert.Clear();
            erase.Clear();
            i += m;
            poppedQueue[i - m].Add(val);
            poppedVal[i] = poppedQueue[i - m].Min();
            UpdatePoppedVal((int)i);
            IncrementalUpdate((int)i);
            return (insert.ToArray(), erase.ToArray());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) SetPop(long i)
        {
            insert.Clear();
            erase.Clear();
            i += m;
            DecrementalUpdate((int)i);
            return (insert.ToArray(), erase.ToArray());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) CancelPop(long i)
        {
            insert.Clear();
            erase.Clear();
            i += m;
            IncrementalUpdate((int)i);
            return (insert.ToArray(), erase.ToArray());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (T[] inserts, T[] erases) CancelPush(long i, T val)
        {
            insert.Clear();
            erase.Clear();
            i += m;
            if (remainQueue[i - m].ContainsKey(val))
            {
                erase.Add(val);
                remainQueue[i - m].Remove(val);
                remainVal[i] = remainQueue[i - m].Count > 0 ? remainQueue[i - m].Min() : remainVal[0];
                UpdateRemainVal((int)i);
            }
            else
            {
                poppedQueue[i - m].Remove(val);
                poppedVal[i] = poppedQueue[i - m].Count > 0 ? poppedQueue[i - m].Min() : poppedVal[0];
                UpdatePoppedVal((int)i);
                DecrementalUpdate((int)i);
            }
            return (insert.ToArray(), erase.ToArray());
        }
    }
    ////end
}