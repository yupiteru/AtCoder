using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC076D
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
            var N = Console_.NextInt();

            var vary = new int[N];
            var tary = new int[N];

            for (int i = 0; i < N; ++i)
            {
                tary[i] = Console_.NextInt();
            }
            for (int i = 0; i < N; ++i)
            {
                vary[i] = Console_.NextInt();
            }

            var l = new int[N + 2];
            var r = new int[N + 2];
            var v = new int[N + 2];
            var totalTime = 0;
            for (int i = 1; i <= N; ++i)
            {
                l[i] = totalTime;
                r[i] = totalTime + tary[i - 1];
                v[i] = vary[i - 1];
                totalTime += tary[i - 1];
            }
            l[N + 1] = r[N + 1] = totalTime;

            var beforevt = 0.0;
            var sum = 0.0;
            for (int i = 1; i <= totalTime * 2; ++i)
            {
                var t = i / 2.0;
                var minv = 1000000000.0;
                for (int j = 0; j < N + 2; ++j)
                {
                    var vt = 0.0;
                    if (t < l[j])
                    {
                        vt = v[j] + l[j] - t;
                    }
                    else if (t > r[j])
                    {
                        vt = v[j] + t - r[j];
                    }
                    else
                    {
                        vt = v[j];
                    }
                    if (minv > vt) minv = vt;
                }
                sum += 0.25 * (beforevt + minv);
                beforevt = minv;
            }
            Console.WriteLine(sum);
        }
    }
}
