using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC079D
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

            var costTable = new int[10][];
            for (int i = 0; i < 10; ++i)
            {
                costTable[i] = new int[10];
                for (int j = 0; j < 10; ++j)
                {
                    costTable[i][j] = Console_.NextInt();
                }
            }

            for (int k = 0; k < 10; ++k)
            {
                for (int i = 0; i < 10; ++i)
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        costTable[i][j] = Math.Min(costTable[i][j], costTable[i][k] + costTable[k][j]);
                    }
                }
            }

            var ans = 0;
            for (int i = 0; i < H; ++i)
            {
                for (int j = 0; j < W; ++j)
                {
                    var val = Console_.NextInt();
                    if (val == -1) continue;
                    ans += costTable[val][1];
                }
            }

            Console.WriteLine(ans);
        }
    }
}
