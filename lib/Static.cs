using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Library
{
    ////start
    static partial class LIB_Static
    {
        public static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
        public static IEnumerator<uint> _xsi = _xsc();
        public static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = 0; while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }
        public static long Count<T>(this IEnumerable<T> x, Func<T, bool> pred) => Enumerable.Count(x, pred);
        public static IEnumerable<T> Repeat<T>(T v, long n) => Enumerable.Repeat<T>(v, (int)n);
        public static IEnumerable<int> Range(long s, long c) => Enumerable.Range((int)s, (int)c);
        public static IOrderedEnumerable<T> OrderByRand<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x, _ => xorshift);
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x) => Enumerable.OrderBy(x.OrderByRand(), e => e);
        public static IOrderedEnumerable<T1> OrderBy<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderBy(x.OrderByRand(), selector);
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x) => Enumerable.OrderByDescending(x.OrderByRand(), e => e);
        public static IOrderedEnumerable<T1> OrderByDescending<T1, T2>(this IEnumerable<T1> x, Func<T1, T2> selector) => Enumerable.OrderByDescending(x.OrderByRand(), selector);
        public static IOrderedEnumerable<string> OrderBy(this IEnumerable<string> x) => x.OrderByRand().OrderBy(e => e, StringComparer.OrdinalIgnoreCase);
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderBy(selector, StringComparer.OrdinalIgnoreCase);
        public static IOrderedEnumerable<string> OrderByDescending(this IEnumerable<string> x) => x.OrderByRand().OrderByDescending(e => e, StringComparer.OrdinalIgnoreCase);
        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> x, Func<T, string> selector) => x.OrderByRand().OrderByDescending(selector, StringComparer.OrdinalIgnoreCase);
        public static string Join<T>(this IEnumerable<T> x, string separator = "") => string.Join(separator, x);
        public static bool Chmax<T>(this ref T lhs, T rhs) where T : struct, IComparable<T> { if (lhs.CompareTo(rhs) < 0) { lhs = rhs; return true; } return false; }
        public static bool Chmin<T>(this ref T lhs, T rhs) where T : struct, IComparable<T> { if (lhs.CompareTo(rhs) > 0) { lhs = rhs; return true; } return false; }
        public static void Fill<T>(this T[] array, T value) => array.AsSpan().Fill(value);
        public static void Fill<T>(this T[,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0], array.Length).Fill(value);
        public static void Fill<T>(this T[,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0], array.Length).Fill(value);
        public static void Fill<T>(this T[,,,] array, T value) => MemoryMarshal.CreateSpan(ref array[0, 0, 0, 0], array.Length).Fill(value);
    }
    ////end
}