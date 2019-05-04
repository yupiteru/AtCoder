using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class AGC033B
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
            var H = Console_.NextInt();
            var W = Console_.NextInt();
            var N = Console_.NextInt();
            var sr = Console_.NextInt();
            var sc = Console_.NextInt();
            var S = Console_.NextString().ToArray();
            var T = Console_.NextString().ToArray();

            var safeL = 1;
            var safeR = W;
            var safeU = 1;
            var safeD = H;
            for (int i = N - 1; i >= 0; --i)
            {
                if (i < N - 1)
                {
                    if (T[i] == 'L' && safeR < W) ++safeR;
                    else if (T[i] == 'R' && safeL > 1) --safeL;
                    else if (T[i] == 'U' && safeD < H) ++safeD;
                    else if (T[i] == 'D' && safeU > 1) --safeU;
                }
                if (S[i] == 'L') ++safeL;
                else if (S[i] == 'R') --safeR;
                else if (S[i] == 'U') ++safeU;
                else if (S[i] == 'D') --safeD;
                if (safeL > safeR || safeU > safeD)
                {
                    Console.WriteLine("NO");
                    return;
                }
            }

            if (safeL <= sc && sc <= safeR && safeU <= sr && sr <= safeD)
            {
                Console.WriteLine("YES");
            }
            else
            {
                Console.WriteLine("NO");
            }
        }
    }
}
