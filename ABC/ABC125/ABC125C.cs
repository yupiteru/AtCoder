using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC125C
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
            var Aary = new int[N];
            for (int i = 0; i < N; ++i)
            {
                Aary[i] = Console_.NextInt();
            }
            var Lary = new int[N];
            var Rary = new int[N];
            for (int i = 1; i < Aary.Length; ++i)
            {
                if (i == 1)
                {
                    Lary[i - 1] = Aary[i - 1];
                }
                else
                {
                    Lary[i - 1] = gcd(Aary[i - 1], Lary[i - 2]);
                }
            }
            for (int i = Aary.Length - 2; i > -1; --i)
            {
                if (i == Aary.Length - 2)
                {
                    Rary[i + 1] = Aary[i + 1];
                }
                else
                {
                    Rary[i + 1] = gcd(Aary[i + 1], Rary[i + 2]);
                }
            }
            int max = 0;
            for (int i = 0; i < Aary.Length; ++i)
            {
                int v = 0;
                if(i == 0) {
                    v = Rary[i + 1];
                }else if(i == Aary.Length - 1) {
                    v = Lary[i - 1];
                }else {
                    v = gcd(Rary[i + 1], Lary[i - 1]);
                }
                if(max < v) {
                    max = v;
                }
            }
 
            Console.WriteLine(max);
        }
 
        static int gcd(int a, int b)
        {
            if (b == 0) return a;
            return gcd(b, a % b);
        }
    }
}
