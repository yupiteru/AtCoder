using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC073D
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
            var R = Console_.NextInt();

            var rs = new int[R];
            for (int i = 0; i < R; ++i)
            {
                var r = Console_.NextInt();
                rs[i] = r - 1;
            }

            var dist = new int[N][];
            for (int i = 0; i < N; ++i)
            {
                dist[i] = new int[N];
                for (int j = 0; j < N; ++j)
                {
                    dist[i][j] = i == j ? 0 : 1000000000;
                }
            }
            for (int i = 0; i < M; ++i)
            {
                var A = Console_.NextInt();
                var B = Console_.NextInt();
                var C = Console_.NextInt();

                dist[A - 1][B - 1] = C;
                dist[B - 1][A - 1] = C;
            }

            for (int k = 0; k < N; ++k)
            {
                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        dist[i][j] = Math.Min(dist[i][j], dist[i][k] + dist[k][j]);
                    }
                }
            }

            Console.WriteLine(MinCost(-1, rs, dist));
        }

        public static int MinCost(int first, int[] tgt, int[][] dist)
        {
            if (tgt.Length == 0) return 0;
            int min = 1000000000;
            for (int i = 0; i < tgt.Length; ++i)
            {
                var list = tgt.ToList();
                list.RemoveAt(i);
                var c = (first != -1 ? dist[first][tgt[i]] : 0) + MinCost(tgt[i], list.ToArray(), dist);
                if (c < min) min = c;
            }
            return min;
        }
    }
}
