using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class DIVERTA2019A
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var K = NextLong;
            
            Console.WriteLine(N - K + 1);
        }

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
        static int NextInt => Console_.NextInt();
        static long NextLong => Console_.NextLong();
        static string NextString => Console_.NextString();

        static IEnumerable<int> Prime(int x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (int)(Math.Sqrt(x) / 2);
            for (int i = 1; i <= max; ++i)
            {
                if (!table[i])
                {
                    var add = 2 * i + 1;
                    yield return add;
                    for (int j = 2 * i * (i + 1); j <= halfx; j += add)
                    {
                        table[j] = true;
                    }
                }
            }
            for (int i = max + 1; i <= halfx; ++i)
            {
                if (!table[i]) yield return 2 * i + 1;
            }
        }
    }
}
