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
    class LIB_PriorityQueue
    {
        List<long> heap;
        bool asc;
        public long Peek => heap[0];
        public int Count => heap.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(int cap, bool asc = true)
        {
            this.asc = asc;
            heap = new List<long>(cap);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(bool asc = true)
        {
            this.asc = asc;
            heap = new List<long>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(long val)
        {
            var i = heap.Count;
            heap.Add(val);
            while (i > 0)
            {
                var ni = (i - 1) / 2;
                if (asc ? val >= heap[ni] : val <= heap[ni]) break;
                heap[i] = heap[ni];
                i = ni;
            }
            heap[i] = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Pop()
        {
            var ret = heap[0];
            var val = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count == 0) return ret;
            var i = 0; while ((i << 1) + 1 < heap.Count)
            {
                var i1 = (i << 1) + 1;
                var i2 = (i << 1) + 2;
                if (i2 < heap.Count && (asc ? heap[i1] > heap[i2] : heap[i1] < heap[i2])) i1 = i2;
                if (asc ? val <= heap[i1] : val >= heap[i1]) break;
                heap[i] = heap[i1]; i = i1;
            }
            heap[i] = val;
            return ret;
        }
    }
    class LIB_PriorityQueueT<T>
    {
        List<T> heap;
        Comparison<T> comp;
        public T Peek => heap[0];
        public int Count => heap.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(int cap, Comparison<T> cmp, bool asc = true)
        {
            heap = new List<T>(cap);
            comp = asc ? cmp : (x, y) => cmp(y, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(Comparison<T> cmp, bool asc = true)
        {
            heap = new List<T>();
            comp = asc ? cmp : (x, y) => cmp(y, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(int cap, bool asc = true) : this(cap, Comparer<T>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(bool asc = true) : this(Comparer<T>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T val)
        {
            var i = heap.Count;
            heap.Add(val);
            while (i > 0)
            {
                var ni = (i - 1) / 2;
                if (comp(val, heap[ni]) >= 0) break;
                heap[i] = heap[ni];
                i = ni;
            }
            heap[i] = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            var ret = heap[0];
            var val = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count == 0) return ret;
            var i = 0; while ((i << 1) + 1 < heap.Count)
            {
                var i1 = (i << 1) + 1;
                var i2 = (i << 1) + 2;
                if (i2 < heap.Count && comp(heap[i1], heap[i2]) > 0) i1 = i2;
                if (comp(val, heap[i1]) <= 0) break;
                heap[i] = heap[i1]; i = i1;
            }
            heap[i] = val;
            return ret;
        }
    }
    class LIB_PriorityQueueT<TK, TV>
    {
        LIB_PriorityQueueT<KeyValuePair<TK, TV>> q;
        public KeyValuePair<TK, TV> Peek => q.Peek;
        public int Count => q.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(int cap, Comparison<TK> cmp, bool asc = true)
        {
            q = new LIB_PriorityQueueT<KeyValuePair<TK, TV>>(cap, (x, y) => cmp(x.Key, y.Key), asc);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(Comparison<TK> cmp, bool asc = true)
        {
            q = new LIB_PriorityQueueT<KeyValuePair<TK, TV>>((x, y) => cmp(x.Key, y.Key), asc);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(int cap, bool asc = true) : this(cap, Comparer<TK>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueueT(bool asc = true) : this(Comparer<TK>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(TK k, TV v) => q.Push(new KeyValuePair<TK, TV>(k, v));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TK, TV> Pop() => q.Pop();
    }
    ////end
}