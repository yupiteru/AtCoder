using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class AGC033A
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

            var blackTable = new bool[H, W];
            var list = new List<Tuple<int, int>>();
            for (int i = 0; i < H; ++i)
            {
                var ws = Console_.NextString().ToArray();
                for (int j = 0; j < W; ++j)
                {
                    if (ws[j] == '#')
                    {
                        blackTable[i, j] = true;
                        list.Add(Tuple.Create(i, j));
                    }
                }
            }

            var count = 0;
            while (list.Count != 0)
            {
                ++count;
                var nextList = new List<Tuple<int, int>>();
                foreach (var item in list)
                {
                    if (item.Item1 > 0)
                    {
                        if (!blackTable[item.Item1 - 1, item.Item2])
                        {
                            blackTable[item.Item1 - 1, item.Item2] = true;
                            nextList.Add(Tuple.Create(item.Item1 - 1, item.Item2));
                        }
                    }
                    if (item.Item1 < H - 1)
                    {
                        if (!blackTable[item.Item1 + 1, item.Item2])
                        {
                            blackTable[item.Item1 + 1, item.Item2] = true;
                            nextList.Add(Tuple.Create(item.Item1 + 1, item.Item2));
                        }
                    }
                    if (item.Item2 > 0)
                    {
                        if (!blackTable[item.Item1, item.Item2 - 1])
                        {
                            blackTable[item.Item1, item.Item2 - 1] = true;
                            nextList.Add(Tuple.Create(item.Item1, item.Item2 - 1));
                        }
                    }
                    if (item.Item2 < W - 1)
                    {
                        if (!blackTable[item.Item1, item.Item2 + 1])
                        {
                            blackTable[item.Item1, item.Item2 + 1] = true;
                            nextList.Add(Tuple.Create(item.Item1, item.Item2 + 1));
                        }
                    }
                }
                list = nextList;
            }

            Console.WriteLine(count - 1);
        }
    }
}
