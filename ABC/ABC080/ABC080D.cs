using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC080D
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
            var C = Console_.NextInt();

            var time = new uint[100000];
            for (int i = 0; i < N; ++i)
            {
                var s = Console_.NextInt();
                var t = Console_.NextInt();
                var c = Console_.NextInt();
                var mask = (uint)1 << (c - 1);
                time[s - 1] ^= mask;
                time[t - 1] ^= mask;
            }

            var maxCh = 0;
            uint state = 0;
            for (int i = 0; i < 100000; ++i)
            {
                var chk = state | time[i];
                state ^= time[i];
                var chCount = 0;
                uint mask = 1;
                for (int j = 0; j < 30; ++j)
                {
                    if ((chk & mask) != 0) ++chCount;
                    mask <<= 1;
                }
                if (maxCh < chCount) maxCh = chCount;
            }

            Console.WriteLine(maxCh);
        }
    }
}
