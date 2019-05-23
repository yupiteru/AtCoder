using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC123A
    {
        static public void Main(string[] args)
        {
            var a = NextIntArray(6);

            Console.WriteLine(Math.Abs(a[0] - a[4]) <= a[5] ? "Yay!" : ":(");
        }

        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static int NextInt => int.Parse(Console_.NextString());
        static int[] NextIntArray(int N)
        {
            var ret = new int[N];
            for (int i = 0; i < N; ++i) ret[i] = NextInt;
            return ret;
        }
        static long NextLong => long.Parse(Console_.NextString());
        static long[] NextLongArray(int N)
        {
            var ret = new long[N];
            for (int i = 0; i < N; ++i) ret[i] = NextLong;
            return ret;
        }
        static string NextString => Console_.NextString();
        static void Rep(int count, Action<int> func)
        {
            for (int i = 0; i < count; ++i) func(i);
        }
        static void Rep(int count, Action func)
        {
            for (int i = 0; i < count; ++i) func();
        }
        static void Foreach<T>(IEnumerable<T> list, Action<T> func)
        {
            foreach (var item in list) func(item);
        }
        static void Foreach<T>(IEnumerable<T> list, Action<T, int> func)
        {
            int idx = -1;
            foreach (var item in list) func(item, ++idx);
        }
        static IEnumerable<long> Prime(long x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (long)(Math.Sqrt(x) / 2);
            for (long i = 1; i <= max; ++i)
            {
                if (table[i]) continue;
                var add = 2 * i + 1;
                yield return add;
                for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true;
            }
            for (long i = max + 1; i <= halfx; ++i) if (!table[i]) yield return 2 * i + 1;
        }
        static IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i < max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        static long GCD(long a, long b)
        {
            long tmpa = a, tmpb = b;
            while (tmpb > 0) { var tmp = tmpb; tmpb = tmpa % tmpb; tmpa = tmp; }
            return tmpa;
        }
    }
}
