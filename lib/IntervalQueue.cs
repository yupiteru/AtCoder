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
    class LIB_IntervalQueue<T> where T : IComparable<T>
    {
        T[] heap;

        public long Count
        {
            get;
            private set;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_IntervalQueue()
        {
            heap = new T[8];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Expand()
        {
            var tmp = new T[Count << 1];
            for (var i = 0; i < heap.Length; ++i) tmp[i] = heap[i];
            heap = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T x)
        {
            if (Count == heap.Length) Expand();
            var k = Count;
            heap[Count++] = x;
            Up((int)k);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopMin()
        {
            if (Count < 3)
            {
                return heap[--Count];
            }
            (heap[1], heap[Count - 1]) = (heap[Count - 1], heap[1]);
            var ret = heap[--Count];
            var k = Down(1);
            Up(k);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopMax()
        {
            if (Count < 2)
            {
                return heap[--Count];
            }
            (heap[0], heap[Count - 1]) = (heap[Count - 1], heap[0]);
            var ret = heap[--Count];
            var k = Down(0);
            Up(k);
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Present(int k) => ((k >> 1) - 1) & ~1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Down(int k)
        {
            if ((k & 1) != 0)
            {
                while (2 * k + 1 < Count)
                {
                    var c = 2 * k + 3;
                    if (Count <= c || heap[c - 2].CompareTo(heap[c]) < 0) c -= 2;
                    if (c < Count && heap[c].CompareTo(heap[k]) < 0)
                    {
                        (heap[k], heap[c]) = (heap[c], heap[k]);
                        k = c;
                    }
                    else break;
                }
            }
            else
            {
                while (2 * k + 2 < Count)
                {
                    var c = 2 * k + 4;
                    if (Count <= c || heap[c - 2].CompareTo(heap[c]) > 0) c -= 2;
                    if (c < Count && heap[c].CompareTo(heap[k]) > 0)
                    {
                        (heap[k], heap[c]) = (heap[c], heap[k]);
                        k = c;
                    }
                    else break;
                }
            }
            return k;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Up(int k)
        {
            if ((k | 1) < Count && heap[k & ~1].CompareTo(heap[k | 1]) < 0)
            {
                (heap[k & ~1], heap[k | 1]) = (heap[k | 1], heap[k & ~1]);
                k ^= 1;
            }

            var root = 1;
            var p = 0;
            while (root < k && heap[p = Present(k)].CompareTo(heap[k]) < 0)
            {
                (heap[p], heap[k]) = (heap[k], heap[p]);
                k = p;
            }
            while (root < k && heap[p = Present(k) | 1].CompareTo(heap[k]) > 0)
            {
                (heap[p], heap[k]) = (heap[k], heap[p]);
                k = p;
            }
            return k;
        }

        public T Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (Count < 2) return heap[0];
                return heap[1];
            }
        }

        public T Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => heap[0];
        }
    }
    ////end
}