using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC125A
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
            var A = Console_.NextInt();
            var B = Console_.NextInt();
            var T = Console_.NextInt();

            var bis = Math.Floor((T + 0.5) / A) * B;

            Console.WriteLine(bis);
        }
    }
}
