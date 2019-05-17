using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC067D
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var path = new List<int>[N];
            for (int i = 0; i < N; ++i)
            {
                path[i] = new List<int>();
            }
            for (int i = 0; i < N - 1; ++i)
            {
                var a = NextInt - 1;
                var b = NextInt - 1;
                path[a].Add(b);
                path[b].Add(a);
            }

            var bfs = new Queue<int>();
            var dist = new int[N];
            var pred = new int[N];
            var done = new bool[N];
            bfs.Enqueue(N - 1);
            done[N - 1] = true;
            while (!done[0])
            {
                var v = bfs.Dequeue();
                foreach (var item in path[v])
                {
                    if (!done[item])
                    {
                        dist[item] = dist[v] + 1;
                        pred[item] = v;
                        done[item] = true;
                        bfs.Enqueue(item);
                    }
                }
            }

            var midlength = dist[0] / 2;
            var p = 0;
            var count = 0;
            while (count != midlength)
            {
                p = pred[p];
                ++count;
            }
            foreach (var item in path[p])
            {
                path[item].Remove(p);
            }
            path[p].Clear();

            var queue = new Queue<int>();
            queue.Enqueue(N - 1);
            done = new bool[N];
            var field = 0;
            while (queue.Any())
            {
                var v = queue.Dequeue();
                if (!done[v])
                {
                    done[v] = true;
                    ++field;
                    foreach (var item in path[v])
                    {
                        queue.Enqueue(item);
                    }
                }
            }

            Console.WriteLine(field < (N + 1) / 2 ? "Fennec" : "Snuke");
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
