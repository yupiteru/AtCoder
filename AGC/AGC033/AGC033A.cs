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
            var fixW = (int)((W - 1) / 64) + 1;

            var ary = new ulong[H][];
            for (int i = 0; i < H; ++i)
            {
                ary[i] = new ulong[fixW];
                var ws = Console_.NextString().ToArray();
                for (int j = 0; j < W; ++j)
                {
                    var bit = (ulong)(ws[j] == '#' ? 1 : 0);
                    var shift = j % 64;
                    var idx = j / 64;
                    ary[i][idx] += bit << shift;
                }
            }

            var allF = (ulong)0xffffffffffffffff;
            var partialF = (ulong)0;
            for (int i = 0; i < (W % 64 == 0 ? 64 : W % 64); ++i)
            {
                partialF <<= 1;
                partialF += 1;
            }

            var checkList = new Queue<int>();
            for (int i = 0; i < H; ++i)
            {
                for (int j = 0; j < fixW - 1; ++j)
                {
                    checkList.Enqueue(j * 1000 + i);
                }
                checkList.Enqueue((fixW - 1) * 1000 + i);
            }

            Func<bool> Check = () =>
            {
                while(checkList.Count != 0)
                {
                    var item = checkList.Dequeue();
                    int i = item % 1000;
                    int j = item / 1000;
                    if (j == fixW - 1)
                    {
                        if ((partialF & ary[i][fixW - 1]) != partialF)
                        {
                            checkList.Enqueue(item);
                            return false;
                        }
                    }
                    else
                    {
                        if (ary[i][j] != allF)
                        {
                            checkList.Enqueue(item);
                            return false;
                        }
                    }
                }
                return true;
            };

            var count = 0;
            while (!Check())
            {
                ++count;

                var newary = new ulong[H][];
                for (int i = 0; i < H; ++i)
                {
                    newary[i] = new ulong[fixW];
                    for (int j = 0; j < fixW; ++j)
                    {
                        newary[i][j] = ary[i][j];
                        if(ary[i][j] == allF) continue;
                        if (i != 0)
                        {
                            newary[i][j] |= ary[i - 1][j];
                        }
                        if (i != H - 1)
                        {
                            newary[i][j] |= ary[i + 1][j];
                        }
                        newary[i][j] |= (ary[i][j] << 1);
                        newary[i][j] |= (ary[i][j] >> 1);
                        if (j != 0)
                        {
                            newary[i][j] |= ((ulong)1 & (ary[i][j - 1] >> 63));
                        }
                        if (j != fixW - 1)
                        {
                            newary[i][j] |= (((ulong)1 & (ary[i][j + 1])) << 63);
                        }
                    }
                }
                ary = newary;
            }
            Console.WriteLine(count);
        }
    }
}
