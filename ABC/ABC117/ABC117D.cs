using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC117D
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var K = (ulong)NextLong;
            var count = new ulong[64];
            var Aary = new ulong[N];
            for (int i = 0; i < N; ++i)
            {
                var A = (ulong)NextLong;
                Aary[i] = A;
                for (int j = 0; j < 64; ++j)
                {
                    count[j] += A & 1;
                    A >>= 1;
                }
            }
            var mask = (ulong)0x8000000000000000;
            var X = (ulong)0;
            var ok = false;
            for (int i = 0; i < 64; ++i)
            {
                X <<= 1;
                if (ok)
                {
                    if (count[63 - i] <= (ulong)N / 2) X += 1;
                }
                else
                {
                    if ((mask & K) != 0)
                    {
                        if (count[63 - i] <= (ulong)N / 2) X += 1;
                        else ok = true;
                    }
                }
                mask >>= 1;
            }

            var ans = (ulong)0;
            foreach (var item in Aary)
            {
                ans += item ^ X;
            }

            Console.WriteLine(ans);
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
        static long NextLong => long.Parse(Console_.NextString());
        static string NextString => Console_.NextString();

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
