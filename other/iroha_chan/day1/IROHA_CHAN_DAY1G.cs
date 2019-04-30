using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1G
    {
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static int NextInt() => int.Parse(NextString());
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static public void Main(string[] args)
        {
            List<int> A = new List<int>();
            var N = Console_.NextInt();
            var M = Console_.NextInt();
            var K = Console_.NextInt();
            for (int i = 0; i < N; ++i)
            {
                A.Add(Console_.NextInt());
            }
            long[][][] memo = new long[2][][];
            for (int i = 0; i <= N; ++i)
            {
                memo[1] = new long[K][];
                for (int j = 0; j < K; ++j)
                {
                    memo[1][j] = new long[M + 1];
                    for (int k = 0; k <= M; ++k)
                    {
                        if (i / K > k)
                        {
                            memo[1][j][k] = -1;
                        }
                        else if (k == 0)
                        {
                            memo[1][j][k] = 0;
                        }
                        else if (i == 0)
                        {
                            if (k != 0)
                            {
                                memo[1][j][k] = -1;
                            }
                        }
                        else if (j == K - 1)
                        {
                            if (memo[0][0][k - 1] == -1)
                            {
                                memo[1][j][k] = -1;
                            }
                            else
                            {
                                memo[1][j][k] = memo[0][0][k - 1] + A[N - i];
                            }
                        }
                        else
                        {
                            var a = memo[0][j + 1][k];
                            var b = memo[0][0][k - 1];
                            if (a == -1 && b == -1)
                            {
                                memo[1][j][k] = -1;
                            }
                            else
                            if (a == -1)
                            {
                                memo[1][j][k] = b + A[N - i];
                            }
                            else
                            if (b == -1)
                            {
                                memo[1][j][k] = a;
                            }
                            else
                            {
                                memo[1][j][k] = Math.Max(a, b + A[N - i]);
                            }
                        }
                    }
                }
                memo[0] = memo[1];
            }
            Console.WriteLine(memo[0][0][M]);
        }
    }
}
