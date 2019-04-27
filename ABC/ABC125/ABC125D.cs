using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC125D
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
            long sum = 0;
            long mini = 10000000000;
            bool odd = false;
            for (int i = 0; i < N; ++i)
            {
                var A = Console_.NextInt();
                if (A < 0) odd = !odd;
                var absA = Math.Abs(A);
                if (absA < mini) mini = absA;
                sum += absA;
            }

            Console.WriteLine(odd ? sum - mini * 2 : sum);
        }
    }
}
