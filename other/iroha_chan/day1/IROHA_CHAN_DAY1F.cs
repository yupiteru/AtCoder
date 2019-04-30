using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1F
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
            var K = Console_.NextInt();
            var ary = new bool[31623];
            var list = new List<int>();
            for (int i = 2; i < 31623; ++i)
            {
                if (!ary[i])
                {
                    if (N % i == 0)
                    {
                        list.Add(i);
                        for (int j = i * 2; j < 31623; j += i)
                        {
                            ary[j] = true;
                        }
                    }
                    else
                    {
                        for (int j = i; j < 31623; j += i)
                        {
                            ary[j] = true;
                        }
                    }
                }
            }
            var count = 0;
            long tmp = N;
            var outList = new List<long>();
            if(K == 1) {
                Console.WriteLine(tmp);
                return;
            }
            foreach (var item in list)
            {
                while (tmp % item == 0)
                {
                    ++count;
                    outList.Add(item);
                    tmp /= item;
                    if (count == K)
                    {
                        if (tmp == 1)
                        {
                            Console.WriteLine(outList[0]);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("-1");
                            return;
                        }
                    }
                    if (count == K - 1)
                    {
                        if (tmp >= 2)
                        {
                            foreach (var item2 in outList)
                            {
                                Console.Write(item2 + " ");
                            }
                            Console.WriteLine(tmp);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("-1");
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("-1");
        }
    }
}
