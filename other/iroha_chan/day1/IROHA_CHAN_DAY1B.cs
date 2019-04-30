using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class IROHA_CHAN_DAY1B
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
            var S = Console_.NextString();
            var K = Console_.NextInt();

            Console.WriteLine(S.Substring(K % S.Length) + S.Substring(0, K % S.Length));
        }
    }
}
