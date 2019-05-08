using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class AGC019A
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
            var Q = (ulong)Console_.NextInt();
            var H = (ulong)Console_.NextInt();
            var S = (ulong)Console_.NextInt();
            var D = (ulong)Console_.NextInt();
            var N = (ulong)Console_.NextLong() * 4;

            var sortdic = new SortedDictionary<ulong, ulong>();
            sortdic[Q * 8] = 1;
            sortdic[H * 4] = 2;
            sortdic[S * 2] = 4;
            sortdic[D] = 8;

            var sum = (ulong)0;
            foreach (var item in sortdic)
            {
                sum += (N / item.Value) * item.Key / (8 / item.Value);
                N %= item.Value;
            }

            Console.WriteLine(sum);
        }
    }
}
