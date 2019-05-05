using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC057D
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
            var A = Console_.NextInt();
            var B = Console_.NextInt();

            var aryV = new long[N];
            for (int i = 0; i < N; ++i)
            {
                aryV[i] = Console_.NextLong();
            }
            Array.Sort(aryV);
            aryV = aryV.Reverse().ToArray();

            var maxAve = 0.0d;
            for (int i = 0; i < A; ++i)
            {
                maxAve += aryV[i] / (double)A;
            }

            var ctable = new long[N + 1, N + 1];
            for (int i = 0; i <= N; ++i)
            {
                for (int j = 0; j <= i; ++j)
                {
                    if (j == 0 || j == i)
                    {
                        ctable[i, j] = 1;
                    }
                    else
                    {
                        ctable[i, j] = ctable[i - 1, j - 1] + ctable[i - 1, j];
                    }
                }
            }

            var x = 0;
            var y = 0;
            for (int i = 0; i < N; ++i)
            {
                if (aryV[A - 1] == aryV[i])
                {
                    ++x;
                    if (i < A)
                    {
                        ++y;
                    }
                }
            }
            var sumCount = 0L;
            if (A == y)
            {
                for (int i = A; i <= B; ++i)
                {
                    sumCount += ctable[x, i];
                }
            }
            else
            {
                sumCount = ctable[x, y];
            }

            Console.WriteLine(maxAve);
            Console.WriteLine(sumCount);
        }
    }
}
