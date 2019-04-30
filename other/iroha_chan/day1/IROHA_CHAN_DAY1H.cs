using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1H
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
            long sum = 0;
            long tmpN = N;
            while (tmpN > 0)
            {
                sum += tmpN % 10;
                tmpN /= 10;
            }
            var count9 = sum / 9;
            var countNext = sum % 9;
            long tmp = countNext;
            for (int i = 0; i < count9; ++i)
            {
                tmp *= 10;
                tmp += 9;
            }
            var shift = false;
            if (tmp == N)
            {
                if (count9 == 0)
                {
                    Console.Write("1");
                    Console.WriteLine(countNext - 1);
                    return;
                }
                ++countNext;
                --count9;
                shift = true;
            }
            if (countNext != 0)
            {
                Console.Write(countNext);
            }
            if (shift)
            {
                Console.Write("8");
            }
            for (int i = 0; i < count9; ++i)
            {
                Console.Write("9");
            }
            Console.WriteLine("");
        }
    }
}
