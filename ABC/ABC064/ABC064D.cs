using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC064D
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
            var S = Console_.NextString().ToArray();
            var ary = new int[N + 1];
            var min = 0;
            ary[0] = 0;
            for (int i = 1; i <= N; ++i)
            {
                ary[i] = (i != 0 ? ary[i - 1] : 0) + (S[i - 1] == '(' ? 1 : -1);
                if (min > ary[i])
                {
                    min = ary[i];
                }
            }
            min *= -1;
            for (int i = 0; i < ary[0] + min; ++i)
            {
                Console.Write("(");
            }
            Console.Write(new string(S));
            for (int i = 0; i < ary[N] + min; ++i)
            {
                Console.Write(")");
            }
            Console.WriteLine();
        }
    }
}
