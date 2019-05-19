using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC126F
    {
        static public void Main(string[] args)
        {
            var M = (ulong)NextInt;
            var K = (ulong)NextInt;

            var two = (ulong)1;
            for (ulong i = 0; i < M; ++i)
            {
                two *= 2;
            }

            if (K >= two)
            {
                Console.WriteLine(-1);
                return;
            }
            if ((M == 0 || M == 1) && K != 0)
            {
                Console.WriteLine(-1);
                return;
            }
            if (K == 0)
            {
                Console.Write("0 0");
                var num = (ulong)1;
                var cnt = (ulong)1;
                for (ulong i = 0; i < M; ++i)
                {
                    for (ulong j = 0; j < cnt; ++j)
                    {
                        Console.Write(" ");
                        Console.Write(num);
                        Console.Write(" ");
                        Console.Write(num);
                        ++num;
                    }
                    cnt *= 2;
                }
                Console.WriteLine();
                return;
            }
            Console.Write(K);
            for (ulong i = 0; i < two; ++i)
            {
                if (i != K)
                {
                    Console.Write(" ");
                    Console.Write(i);
                }
            }
            Console.Write(" ");
            Console.Write(K);
            for (ulong i = two - 1; i > 0; --i)
            {
                if (i != K)
                {
                    Console.Write(" ");
                    Console.Write(i);
                }
            }
            Console.WriteLine(" 0");
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
