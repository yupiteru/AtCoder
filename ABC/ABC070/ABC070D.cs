using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC070D
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
            var next = new List<Tuple<int, int>>[N];
            for (int i = 0; i < N; ++i)
            {
                next[i] = new List<Tuple<int, int>>();
            }
            for (int i = 0; i < N - 1; ++i)
            {
                var a = Console_.NextInt();
                var b = Console_.NextInt();
                var c = Console_.NextInt();
                next[a - 1].Add(Tuple.Create(b - 1, c));
                next[b - 1].Add(Tuple.Create(a - 1, c));
            }

            var Q = Console_.NextInt();
            var K = Console_.NextInt() - 1;

            var kpath = new long[N];
            var done = new bool[N];
            var queue = new Queue<int>();
            queue.Enqueue(K);
            done[K] = true;
            while (queue.Count != 0)
            {
                var item = queue.Dequeue();
                foreach (var item2 in next[item])
                {
                    if (!done[item2.Item1])
                    {
                        done[item2.Item1] = true;
                        queue.Enqueue(item2.Item1);
                        kpath[item2.Item1] = kpath[item] + item2.Item2;
                    }
                }
            }

            for (int i = 0; i < Q; ++i)
            {
                var x = Console_.NextInt() - 1;
                var y = Console_.NextInt() - 1;
                Console.WriteLine(kpath[x] + kpath[y]);
            }
        }
    }
}
