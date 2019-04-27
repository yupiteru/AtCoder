using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC125B
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
            var Vary = new int[N];
            for (int i = 0; i < N; ++i)
            {
                Vary[i] = Console_.NextInt();
            }
            for (int i = 0; i < N; ++i)
            {
                Vary[i] -= Console_.NextInt();
                if (Vary[i] < 0)
                {
                    Vary[i] = 0;
                }
            }

            Console.WriteLine(Vary.Sum());
        }
    }
}
