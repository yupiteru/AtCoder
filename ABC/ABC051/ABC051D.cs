using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC051D
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
            var dist = new int[N, N];
            var checkPath = new List<Tuple<int, int, int>>();
            for (int i = 0; i < M; ++i)
            {
                var a = Console_.NextInt();
                var b = Console_.NextInt();
                var c = Console_.NextInt();
                dist[a - 1, b - 1] = c;
                dist[b - 1, a - 1] = c;
                checkPath.Add(Tuple.Create(a - 1, b - 1, c));
            }
            for (int k = 0; k < N; ++k)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        if (dist[i, k] != 0 && dist[k, j] != 0)
                        {
                            if (dist[i, j] == 0 || dist[i, j] > dist[i, k] + dist[k, j])
                            {
                                dist[i, j] = dist[i, k] + dist[k, j];
                            }
                        }
                    }
                }
            }
            var sum = 0;
            foreach (var item in checkPath)
            {
                if (dist[item.Item1, item.Item2] < item.Item3)
                {
                    ++sum;
                }
            }
            Console.WriteLine(sum);
        }
    }
}
