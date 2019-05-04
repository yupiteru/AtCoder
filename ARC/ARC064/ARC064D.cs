using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ARC064D
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
            var S = Console_.NextString().ToArray();
            if (S[0] == S[S.Length - 1])
            {
                Console.WriteLine(S.Length % 2 == 0 ? "First" : "Second");
                return;
            }
            else
            {
                var oddChara = false;
                for (int i = 1; i < S.Length - 1; ++i)
                {
                    if (S[i] != S[0] && S[i] != S[S.Length - 1])
                    {
                        oddChara = true;
                        break;
                    }
                }
                if (oddChara)
                {
                    Console.WriteLine(S.Length % 2 == 1 ? "First" : "Second");
                    return;
                }
                else
                {
                    Console.WriteLine("Second");
                    return;
                }
            }
        }
    }
}
