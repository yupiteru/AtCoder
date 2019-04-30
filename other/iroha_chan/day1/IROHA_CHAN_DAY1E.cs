using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1E
    {
        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static long NextLong() => long.Parse(NextString());
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static public void Main(string[] args)
        {
            var N = Console_.NextLong();
            var A = Console_.NextLong();
            var B = Console_.NextLong();
            var dateFlag = new SortedSet<long>();
            for (int i = 0; i < B; ++i)
            {
                dateFlag.Add(Console_.NextLong());
            }
            long start = 1;
            var list = new List<long>();
            foreach (var item in dateFlag)
            {
                list.Add(item - start);
                start = item + 1;
            }
            list.Add(N + 1 - start);
            long sum = 0;
            foreach (var item in list)
            {
                sum += (item - item / A);
            }
            Console.WriteLine(sum);
        }
    }
}
