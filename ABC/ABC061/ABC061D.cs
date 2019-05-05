using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC061D
    {
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static int NextInt() => int.Parse(NextString());
            public static long NextLong() => long.Parse(NextString());
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static public void Main(string[] args)
        {
            var N = Console_.NextInt();
            var M = Console_.NextInt();

            var inf = 200000000000000L;

            var dist = new long[N];
            for (int i = 0; i < N; ++i)
            {
                dist[i] = i == 0 ? 0 : inf;
            }
            var paths = new List<Tuple<int, int, long>>();
            for (int i = 0; i < M; ++i)
            {
                var a = Console_.NextInt();
                var b = Console_.NextInt();
                var c = Console_.NextLong() * (-1);

                paths.Add(Tuple.Create(a - 1, b - 1, c));
            }

            for (int i = 0; i < N - 1; ++i)
            {
                foreach (var item in paths)
                {
                    var src = item.Item1;
                    var dst = item.Item2;
                    var cst = item.Item3;

                    if (dist[dst] > dist[src] + cst)
                    {
                        dist[dst] = dist[src] + cst;
                    }
                }
            }

            var neg = new bool[N];
            for (int i = 0; i < N; ++i)
            {
                foreach (var item in paths)
                {
                    var src = item.Item1;
                    var dst = item.Item2;
                    var cst = item.Item3;

                    if (dist[dst] > dist[src] + cst)
                    {
                        dist[dst] = dist[src] + cst;
                        neg[dst] = true;
                    }
                    if (neg[src])
                    {
                        neg[dst] = true;
                    }
                }
            }

            if (neg[N - 1])
            {
                Console.WriteLine("inf");
                return;
            }

            Console.WriteLine(dist[N - 1] * -1);
        }
    }
}
