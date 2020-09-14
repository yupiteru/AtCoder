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
        long[] heap;
        int[] dat;
        public long Count
        {
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue()
        {
            heap = new long[16384];
            dat = new int[16384];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(long key, int val)
        {
            if (Count == heap.Length) Expand();
            var i = Count++;
            heap[i] = key;
            dat[i] = val;
            while (i > 0)
            {
                var ni = (i - 1) / 2;
                if (key >= heap[ni]) break;
                heap[i] = heap[ni];
                dat[i] = dat[ni];
                i = ni;
            }
            heap[i] = key;
            dat[i] = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<long, int> Pop()
        {
            var ret = new KeyValuePair<long, int>(heap[0], dat[0]);
            var key = heap[--Count];
            var val = dat[Count];
            if (Count == 0) return ret;
            var i = 0; while ((i << 1) + 1 < Count)
            {
                var i1 = (i << 1) + 1;
                var i2 = (i << 1) + 2;
                if (i2 < Count && heap[i1] > heap[i2]) i1 = i2;
                if (key <= heap[i1]) break;
                heap[i] = heap[i1];
                dat[i] = dat[i1];
                i = i1;
            }
            heap[i] = key;
            dat[i] = val;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Expand()
        {
            var tmp = new long[Count << 1];
            for (var i = 0; i < heap.Length; ++i) tmp[i] = heap[i];
            heap = tmp;
            var tmp2 = new int[Count << 1];
            for (var i = 0; i < dat.Length; ++i) tmp2[i] = dat[i];
            dat = tmp2;
        }
    }
    class LIB_PriorityQueue<T>
    {
        T[] heap;
        Comparison<T> comp;
        public T Peek => heap[0];
        public long Count
        {
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(long cap, Comparison<T> cmp, bool asc = true)
        {
            heap = new T[cap];
            comp = asc ? cmp : (x, y) => cmp(y, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(Comparison<T> cmp, bool asc = true)
        {
            heap = new T[16384];
            comp = asc ? cmp : (x, y) => cmp(y, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(long cap, bool asc = true) : this(cap, Comparer<T>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(bool asc = true) : this(Comparer<T>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T val)
        {
            if (Count == heap.Length) Expand();
            var i = Count++;
            heap[i] = val;
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
            var val = heap[--Count];
            if (Count == 0) return ret;
            var i = 0; while ((i << 1) + 1 < Count)
            {
                var i1 = (i << 1) + 1;
                var i2 = (i << 1) + 2;
                if (i2 < Count && comp(heap[i1], heap[i2]) > 0) i1 = i2;
                if (comp(val, heap[i1]) <= 0) break;
                heap[i] = heap[i1]; i = i1;
            }
            heap[i] = val;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Expand()
        {
            var tmp = new T[Count << 1];
            for (var i = 0; i < heap.Length; ++i) tmp[i] = heap[i];
            heap = tmp;
        }
    }
    class LIB_PriorityQueue<TK, TV>
    {
        LIB_PriorityQueue<KeyValuePair<TK, TV>> q;
        public KeyValuePair<TK, TV> Peek => q.Peek;
        public long Count => q.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(long cap, Comparison<TK> cmp, bool asc = true)
        {
            q = new LIB_PriorityQueue<KeyValuePair<TK, TV>>(cap, (x, y) => cmp(x.Key, y.Key), asc);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(Comparison<TK> cmp, bool asc = true)
        {
            q = new LIB_PriorityQueue<KeyValuePair<TK, TV>>((x, y) => cmp(x.Key, y.Key), asc);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(long cap, bool asc = true) : this(cap, Comparer<TK>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue(bool asc = true) : this(Comparer<TK>.Default.Compare, asc) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(TK k, TV v) => q.Push(new KeyValuePair<TK, TV>(k, v));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TK, TV> Pop() => q.Pop();
    }
    ////end
}