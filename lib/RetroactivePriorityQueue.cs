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
    class LIB_RetroactivePriorityQueueFix<T>
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
        public LIB_RetroactivePriorityQueueFix(long maxOpes, T min, T max, Comparison<T> cmp, bool asc = true)
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
        public LIB_RetroactivePriorityQueueFix(long maxOpes, T min, T max, bool asc = true) : this(maxOpes, min, max, Comparer<T>.Default.Compare, asc) { }
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
    class LIB_RetroactivePriorityQueue<T>
    {
        int n;
        int m;
        Comparison<T> comp;
        Comparison<T> compRev;
        T[] poppedVal;
        T[] remainVal;
        PriorityQueueDeletable[] poppedQueue;
        PriorityQueueDeletable[] remainQueue;
        List<T> insert;
        List<T> erase;
        (int sum, int min)[] ruisekiFlows;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RetroactivePriorityQueue(long maxOpes, T min, T max, Comparison<T> cmp, bool asc = true)
        {
            if (asc)
            {
                comp = cmp;
                compRev = (x, y) => cmp(y, x);
            }
            else
            {
                comp = (x, y) => cmp(y, x);
                compRev = cmp;
            }
            n = (int)maxOpes;
            m = 1;
            while (m < n + 1) m <<= 1;
            poppedVal = new T[m * 2];
            remainVal = new T[m * 2];
            poppedQueue = Enumerable.Repeat(0, m).Select(_ => new PriorityQueueDeletable(compRev)).ToArray();
            remainQueue = Enumerable.Repeat(0, m).Select(_ => new PriorityQueueDeletable(comp)).ToArray();
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
            remainQueue[c - m].Push(poppedVal[c]);
            remainVal[c] = remainQueue[c - m].Peek();
            poppedQueue[c - m].Pop();
            poppedVal[c] = poppedQueue[c - m].Count > 0 ? poppedQueue[c - m].Peek() : poppedVal[0];
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

            poppedQueue[c - m].Push(remainVal[c]);
            poppedVal[c] = poppedQueue[c - m].Peek();
            remainQueue[c - m].Pop();
            remainVal[c] = remainQueue[c - m].Count > 0 ? remainQueue[c - m].Peek() : remainVal[0];
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
            poppedQueue[i - m].Push(val);
            poppedVal[i] = poppedQueue[i - m].Peek();
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
            if (comp(remainVal[i], val) <= 0)
            {
                erase.Add(val);
                remainQueue[i - m].DeleteExistKeyUnchecked(val);
                remainVal[i] = remainQueue[i - m].Count > 0 ? remainQueue[i - m].Peek() : remainVal[0];
                UpdateRemainVal((int)i);
            }
            else
            {
                poppedQueue[i - m].DeleteExistKeyUnchecked(val);
                poppedVal[i] = poppedQueue[i - m].Count > 0 ? poppedQueue[i - m].Peek() : poppedVal[0];
                UpdatePoppedVal((int)i);
                DecrementalUpdate((int)i);
            }
            return (insert.ToArray(), erase.ToArray());
        }

        class PriorityQueueDeletable
        {
            Comparison<T> comp;
            T[] deleted;
            T[] heap;
            int deletedCount;
            int heapCount;
            public T Peek()
            {
                Validate();
                return heap[0];
            }
            public long Count
            {
                get { Validate(); return heapCount - deletedCount; }
                private set { }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PriorityQueueDeletable(Comparison<T> cmp)
            {
                comp = cmp;
                deleted = new T[8];
                heap = new T[8];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Validate()
            {
                while (deletedCount > 0 && comp(heap[0], deleted[0]) == 0)
                {
                    Pop();
                    PopDeleted();
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void DeleteExistKeyUnchecked(T key)
            {
                if (deletedCount == deleted.Length) ExpandDeleted();
                var i = deletedCount++;
                ref T heapref = ref deleted[0];
                Unsafe.Add(ref heapref, i) = key;
                while (i > 0)
                {
                    var ni = (i - 1) / 2;
                    var heapni = Unsafe.Add(ref heapref, ni);
                    if (comp(key, heapni) >= 0) break;
                    Unsafe.Add(ref heapref, i) = heapni;
                    i = ni;
                }
                Unsafe.Add(ref heapref, i) = key;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(T key)
            {
                if (heapCount == heap.Length) Expand();
                var i = heapCount++;
                ref T heapref = ref heap[0];
                Unsafe.Add(ref heapref, i) = key;
                while (i > 0)
                {
                    var ni = (i - 1) / 2;
                    var heapni = Unsafe.Add(ref heapref, ni);
                    if (comp(key, heapni) >= 0) break;
                    Unsafe.Add(ref heapref, i) = heapni;
                    i = ni;
                }
                Unsafe.Add(ref heapref, i) = key;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void PopDeleted()
            {
                ref T heapref = ref deleted[0];
                var cnt = --deletedCount;
                var key = Unsafe.Add(ref heapref, cnt);
                if (cnt == 0) return;
                var i = 0; while ((i << 1) + 1 < cnt)
                {
                    var i1 = (i << 1) + 1;
                    var i2 = (i << 1) + 2;
                    if (i2 < cnt && comp(Unsafe.Add(ref heapref, i1), Unsafe.Add(ref heapref, i2)) > 0) i1 = i2;
                    var heapi1 = Unsafe.Add(ref heapref, i1);
                    if (comp(key, heapi1) <= 0) break;
                    Unsafe.Add(ref heapref, i) = heapi1;
                    i = i1;
                }
                Unsafe.Add(ref heapref, i) = key;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Pop()
            {
                ref T heapref = ref heap[0];
                var cnt = --heapCount;
                var key = Unsafe.Add(ref heapref, cnt);
                if (cnt == 0) return;
                var i = 0; while ((i << 1) + 1 < cnt)
                {
                    var i1 = (i << 1) + 1;
                    var i2 = (i << 1) + 2;
                    if (i2 < cnt && comp(Unsafe.Add(ref heapref, i1), Unsafe.Add(ref heapref, i2)) > 0) i1 = i2;
                    var heapi1 = Unsafe.Add(ref heapref, i1);
                    if (comp(key, heapi1) <= 0) break;
                    Unsafe.Add(ref heapref, i) = heapi1;
                    i = i1;
                }
                Unsafe.Add(ref heapref, i) = key;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Expand()
            {
                var len = heap.Length;
                var tmp = new T[len << 1];
                Unsafe.CopyBlock(ref Unsafe.As<T, byte>(ref tmp[0]), ref Unsafe.As<T, byte>(ref heap[0]), (uint)(Unsafe.SizeOf<T>() * len));
                heap = tmp;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void ExpandDeleted()
            {
                var len = deleted.Length;
                var tmp = new T[len << 1];
                Unsafe.CopyBlock(ref Unsafe.As<T, byte>(ref tmp[0]), ref Unsafe.As<T, byte>(ref deleted[0]), (uint)(Unsafe.SizeOf<T>() * len));
                deleted = tmp;
            }
        }
    }
    ////end
}