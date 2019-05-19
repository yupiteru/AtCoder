using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC126E
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var M = NextInt;
            var path = new List<int>[N];
            for (int i = 0; i < N; ++i)
            {
                path[i] = new List<int>();
            }
            for (int i = 0; i < M; ++i)
            {
                var X = NextInt - 1;
                var Y = NextInt - 1;
                var Z = NextInt;
                path[X].Add(Y);
                path[Y].Add(X);
            }

            var group = new int[N];
            var groupCount = 0;
            for (int i = 0; i < N; ++i)
            {
                if (group[i] == 0)
                {
                    ++groupCount;
                    group[i] = groupCount;
                    var queue = new Queue<int>();
                    queue.Enqueue(i);
                    while (queue.Any())
                    {
                        var v = queue.Dequeue();
                        foreach (var item in path[v])
                        {
                            if (group[item] == 0)
                            {
                                group[item] = group[v];
                                queue.Enqueue(item);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(groupCount);
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
