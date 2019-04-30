using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1D
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
            var N = Console_.NextInt();
            var X = Console_.NextInt();
            var Y = Console_.NextInt();
            var list = new SortedDictionary<int, int>();
            for (int i = 0; i < N; ++i)
            {
                var tmp = Console_.NextInt();
                if (list.ContainsKey(tmp))
                {
                    list[tmp]++;
                }
                else
                {
                    list[tmp] = 1;
                }
            }
            var ary = list.Reverse().ToArray();
            var count = 0;
            for (int i = 0; i < ary.Length; ++i)
            {
                var tmp = ary[i].Value;
                while (tmp > 0)
                {
                    if (count % 2 == 0)
                    {
                        X += ary[i].Key;
                    }
                    else
                    {
                        Y += ary[i].Key;
                    }
                    --tmp;
                    ++count;
                }
            }
            Console.WriteLine(X < Y ? "Aoki" : X > Y ? "Takahashi" : "Draw");
        }
    }
}
