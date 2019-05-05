using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC054D
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
            var Ma = Console_.NextInt();
            var Mb = Console_.NextInt();
            var dp = new int[N + 1][][];
            for (int i = 0; i < N + 1; ++i)
            {
                dp[i] = new int[40 * 10 + 1][];
                for (int j = 0; j < 40 * 10 + 1; ++j)
                {
                    dp[i][j] = new int[40 * 10 + 1];
                    for (int k = 0; k < 40 * 10 + 1; ++k)
                    {
                        dp[i][j][k] = 1000000000;
                    }
                }
            }
            dp[0][0][0] = 0;
            for (int i = 1; i <= N; ++i)
            {
                var a = Console_.NextInt();
                var b = Console_.NextInt();
                var c = Console_.NextInt();

                for (int j = 0; j <= 40 * 10 - a; ++j)
                {
                    for (int k = 0; k <= 40 * 10 - b; ++k)
                    {
                        dp[i][j][k] = Math.Min(dp[i - 1][j][k], dp[i][j][k]);
                        dp[i][j + a][k + b] = Math.Min(dp[i][j + a][k + b], dp[i - 1][j][k] + c);
                    }
                }
            }
            var ax = Ma;
            var bx = Mb;
            var maxMulti = Math.Min(40 * 10 / Ma, 40 * 10 / Mb);
            var min = 1000000000;
            for (int i = 1; i < maxMulti; ++i)
            {
                if (dp[N][i * ax][i * bx] < min)
                {
                    min = dp[N][i * ax][i * bx];
                }
            }

            Console.WriteLine(min == 1000000000 ? -1 : min);
        }
    }
}
