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
    class LIB_SlidingWindowAggregation<T>
    {
        T[][] list;
        T[][] agg;
        int[] size;
        Func<T, T, T> fun;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SlidingWindowAggregation(IEnumerable<T> ary, Func<T, T, T> f)
        {
            list = new T[2][]; agg = new T[2][]; size = new int[2];
            fun = f;
            list[0] = new T[16]; agg[0] = new T[16]; size[0] = 0;
            if (ary.Any())
            {
                var temp = ary.ToArray();
                size[1] = temp.Length;
                list[1] = new T[size[1]];
                agg[1] = new T[size[1]];
                for (var i = 0; i < list[1].Length; i++)
                {
                    list[1][i] = temp[i];
                    if (i == 0) agg[1][i] = temp[i];
                    else agg[1][i] = fun(agg[1][i - 1], temp[i]);
                }
            }
            else
            {
                list[1] = new T[16]; agg[1] = new T[16]; size[1] = 0;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_SlidingWindowAggregation(Func<T, T, T> f) : this(new T[0], f) { }
        public int Count => size[0] + size[1];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            size[0] = size[1] = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Push(int lista, T val)
        {
            if (list[lista].Length == size[lista])
            {
                var newAry = new T[size[lista] * 2];
                var newAgg = new T[size[lista] * 2];
                Array.Copy(list[lista], newAry, size[lista]);
                Array.Copy(agg[lista], newAgg, size[lista]);
                list[lista] = newAry;
                agg[lista] = newAgg;
            }
            if (size[lista] == 0) agg[lista][0] = val;
            else if (lista == 0) agg[lista][size[lista]] = fun(val, agg[lista][size[lista] - 1]);
            else agg[lista][size[lista]] = fun(agg[lista][size[lista] - 1], val);
            list[lista][size[lista]++] = val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushBack(T val) => Push(1, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushFront(T val) => Push(0, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop(int lista)
        {
            var listb = 1 - lista;
            if (size[lista] == 0)
            {
                if (size[listb] == 0) throw new Exception();
                var mid = (size[listb] - 1) / 2;
                if (list[lista].Length <= mid)
                {
                    list[lista] = new T[mid + 1];
                    agg[lista] = new T[mid + 1];
                }
                size[lista] = 0;
                for (var i = mid; i >= 0; i--)
                {
                    if (size[lista] == 0) agg[lista][size[lista]] = list[listb][i];
                    else if (lista == 0) agg[lista][size[lista]] = fun(list[listb][i], agg[lista][size[lista] - 1]);
                    else agg[lista][size[lista]] = fun(agg[lista][size[lista] - 1], list[listb][i]);
                    list[lista][size[lista]++] = list[listb][i];
                }
                for (var i = mid + 1; i < size[listb]; i++)
                {
                    var idx = i - mid - 1;
                    if (idx == 0) agg[listb][idx] = list[listb][i];
                    else if (lista == 0) agg[listb][idx] = fun(list[listb][i], agg[listb][idx - 1]);
                    else agg[listb][idx] = fun(agg[listb][idx - 1], list[listb][i]);
                    list[listb][idx] = list[listb][i];
                }
                size[listb] -= size[lista];
            }
            return list[lista][--size[lista]];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopBack() => Pop(1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopFront() => Pop(0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Aggregate()
        {
            if (size[0] == 0 && size[1] == 0) throw new Exception();
            else if (size[1] == 0) return agg[0][size[0] - 1];
            else if (size[0] == 0) return agg[1][size[1] - 1];
            else return fun(agg[0][size[0] - 1], agg[1][size[1] - 1]);
        }
    }
    ////end
}