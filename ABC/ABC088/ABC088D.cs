using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC088D
    {
        static public void Main(string[] args)
        {
            var H = NextInt;
            var W = NextInt;

            var dist = new int[H * W];
            var weight = new List<Tuple<int, int>>();
            var vertex = new bool[H * W];
            var pred = new int[H * W];
            for (int i = 0; i < H * W; ++i)
            {
                dist[i] = 1000000000;
                pred[i] = -1;
            }
            for (int i = 0; i < H; ++i)
            {
                var s = NextString.ToArray();
                for (int j = 0; j < W; ++j)
                {
                    vertex[i * W + j] = s[j] == '#';
                    if (i != 0)
                    {
                        if (!vertex[(i - 1) * W + j] && !vertex[i * W + j])
                        {
                            weight.Add(Tuple.Create((i - 1) * W + j, i * W + j));
                            weight.Add(Tuple.Create(i * W + j, (i - 1) * W + j));
                        }
                    }
                    if (j != 0)
                    {
                        if (!vertex[i * W + j - 1] && !vertex[i * W + j])
                        {
                            weight.Add(Tuple.Create(i * W + j - 1, i * W + j));
                            weight.Add(Tuple.Create(i * W + j, i * W + j - 1));
                        }
                    }
                }
            }

            if (!vertex[1])
            {
                dist[1] = 1;
            }
            if (!vertex[W])
            {
                dist[W] = 1;
            }

            for (int i = 0; i < H * W - 1; ++i)
            {
                foreach (var edge in weight)
                {
                    var src = edge.Item1;
                    var dst = edge.Item2;
                    if (dist[dst] > dist[src] + 1)
                    {
                        dist[dst] = dist[src] + 1;
                        pred[dst] = src;
                    }
                }
            }

            if (dist[H * W - 1] == 1000000000)
            {
                Console.WriteLine(-1);
                return;
            }

            var predtmp = pred[H * W - 1];
            vertex[H * W - 1] = true;
            vertex[0] = true;
            while (predtmp != -1)
            {
                vertex[predtmp] = true;
                predtmp = pred[predtmp];
            }

            var sum = 0;
            for (int i = 0; i < H * W; ++i)
            {
                if (!vertex[i]) ++sum;
            }

            Console.WriteLine(sum);
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
