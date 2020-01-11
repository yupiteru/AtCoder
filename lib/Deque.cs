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
    class LIB_Deque<T>
    {
        T[] array;
        int front, cap;
        public int Count;
        public T this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return array[GetIndex((int)i)]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { array[GetIndex((int)i)] = value; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Deque(long cap = 16)
        {
            array = new T[this.cap = (int)cap];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetIndex(int i)
        {
            if (i >= cap) throw new Exception();
            var r = front + i;
            return r >= cap ? r - cap : r;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushFront(T x)
        {
            if (Count == cap) Extend();
            if (--front < 0) front += array.Length;
            array[front] = x;
            ++Count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopFront()
        {
            if (Count-- == 0) throw new Exception();
            var r = array[front++];
            if (front >= cap) front -= cap;
            return r;
        }
        public T Front => array[front];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushBack(T x)
        {
            if (Count == cap) Extend();
            var i = front + Count++;
            array[i >= cap ? i - cap : i] = x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopBack()
        {
            if (Count == 0) throw new Exception();
            return array[GetIndex(--Count)];
        }
        public T Back => array[GetIndex(Count - 1)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Extend()
        {
            T[] nb = new T[cap << 1];
            if (front > cap - Count)
            {
                var l = array.Length - front; Array.Copy(array, front, nb, 0, l);
                Array.Copy(array, 0, nb, l, Count - l);
            }
            else Array.Copy(array, front, nb, 0, Count);
            array = nb; front = 0; cap <<= 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(long i, T x)
        {
            if (i > Count) throw new Exception();
            this.PushFront(x);
            for (int j = 0; j < i; j++) this[j] = this[j + 1];
            this[i] = x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T RemoveAt(long i)
        {
            if (i < 0 || i >= Count) throw new Exception();
            var r = this[i];
            for (var j = i; j > 0; j--) this[j] = this[j - 1];
            this.PopFront();
            return r;
        }
    }
    ////end
}