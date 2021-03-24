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
        public (long Key, int Value) Peek => (heap[0], dat[0]);
        public long Count
        {
            get;
            private set;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_PriorityQueue()
        {
            heap = new long[8];
            dat = new int[8];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(long key) => Push(key, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(long key, int val)
        {
            if (Count == heap.Length) Expand();
            var i = (int)Count++;
            ref long heapref = ref heap[0];
            ref int datref = ref dat[0];
            Unsafe.Add<long>(ref heapref, i) = key;
            Unsafe.Add<int>(ref datref, i) = val;
            while (i > 0)
            {
                var ni = (i - 1) / 2;
                var heapni = Unsafe.Add<long>(ref heapref, ni);
                if (key >= heapni) break;
                Unsafe.Add<long>(ref heapref, i) = heapni;
                Unsafe.Add<int>(ref datref, i) = Unsafe.Add<int>(ref datref, ni);
                i = ni;
            }
            Unsafe.Add<long>(ref heapref, i) = key;
            Unsafe.Add<int>(ref datref, i) = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (long Key, int Value) Pop()
        {
            ref long heapref = ref heap[0];
            ref int datref = ref dat[0];
            var ret = (heapref, datref);
            var cnt = (int)(--Count);
            var key = Unsafe.Add<long>(ref heapref, cnt);
            var val = Unsafe.Add<int>(ref datref, cnt);
            if (cnt == 0) return ret;
            var i = 0; while ((i << 1) + 1 < cnt)
            {
                var i1 = (i << 1) + 1;
                var i2 = (i << 1) + 2;
                if (i2 < cnt && Unsafe.Add<long>(ref heapref, i1) > Unsafe.Add<long>(ref heapref, i2)) i1 = i2;
                var heapi1 = Unsafe.Add<long>(ref heapref, i1);
                if (key <= heapi1) break;
                Unsafe.Add<long>(ref heapref, i) = heapi1;
                Unsafe.Add<int>(ref datref, i) = Unsafe.Add<int>(ref datref, i1);
                i = i1;
            }
            Unsafe.Add<long>(ref heapref, i) = key;
            Unsafe.Add<int>(ref datref, i) = val;
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Expand()
        {
            var len = heap.Length;
            var tmp = new long[len << 1];
            var tmp2 = new int[len << 1];
            Unsafe.CopyBlock(ref Unsafe.As<long, byte>(ref tmp[0]), ref Unsafe.As<long, byte>(ref heap[0]), (uint)(8 * len));
            Unsafe.CopyBlock(ref Unsafe.As<int, byte>(ref tmp2[0]), ref Unsafe.As<int, byte>(ref dat[0]), (uint)(4 * len));
            heap = tmp;
            dat = tmp2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (long Key, int Value)[] NoSortList()
        {
            var ret = new (long Key, int Value)[Count];
            ref long heapref = ref heap[0];
            ref int datref = ref dat[0];
            for (var i = 0; i < Count; ++i)
            {
                ret[i] = (Unsafe.Add<long>(ref heapref, i), Unsafe.Add<int>(ref datref, i));
            }
            return ret;
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
            heap = new T[8];
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] NoSortList()
        {
            var ret = new List<T>();
            for (var i = 0; i < Count; ++i)
            {
                ret.Add(heap[i]);
            }
            return ret.ToArray();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TK, TV>[] NoSortList() => q.NoSortList();
    }
    ////end
}