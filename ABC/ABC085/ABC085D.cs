using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC085D
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var H = NextLong;
            var count = 0;
            var maxa = 0L;
            var sum = 0L;
            var bary = new long[N];
            for (int i = 0; i < N; ++i)
            {
                var a = NextLong;
                var b = NextLong;
                if (maxa < a) maxa = a;
                bary[i] = b;
            }
            Array.Sort(bary);
            bary = bary.Reverse().ToArray();
            foreach (var item in bary)
            {
                if (item < maxa) continue;
                sum += item;
                ++count;
                if (sum >= H)
                {
                    Console.Write(count);
                    return;
                }
            }
            Console.WriteLine(count + (H - sum - 1) / maxa + 1);
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
