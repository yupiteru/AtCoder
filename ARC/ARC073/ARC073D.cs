using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ARC073D
    {
        static public void Main(string[] args)
        {
            var N = NextInt;
            var W = NextInt;
            var vAry = new List<ulong>[4];
            for (int i = 0; i < 4; ++i)
            {
                vAry[i] = new List<ulong>();
            }
            var firstW = (ulong)0;
            for (int i = 0; i < N; ++i)
            {
                var w = (ulong)NextInt;
                var v = (ulong)NextInt;
                if (i == 0)
                {
                    firstW = w;
                }
                var idx = w - firstW;
                vAry[idx].Add(v);
            }
            for (int i = 0; i < 4; ++i)
            {
                vAry[i].Sort((x, y) => y.CompareTo(x));
                vAry[i].Insert(0, 0);
                for (int j = 1; j < vAry[i].Count; ++j)
                {
                    vAry[i][j] = vAry[i][j] + vAry[i][j - 1];
                }
            }

            var ans = (ulong)0;
            for (int i = 0; i < vAry[0].Count; ++i)
            {
                for (int j = 0; j < vAry[1].Count; ++j)
                {
                    for (int k = 0; k < vAry[2].Count; ++k)
                    {
                        for (int l = 0; l < vAry[3].Count; ++l)
                        {
                            var sumW = (firstW + 0) * (ulong)i +
                                       (firstW + 1) * (ulong)j +
                                       (firstW + 2) * (ulong)k +
                                       (firstW + 3) * (ulong)l;
                            var sumV = vAry[0][i] +
                                       vAry[1][j] +
                                       vAry[2][k] +
                                       vAry[3][l];
                            if (sumW <= (ulong)W)
                            {
                                ans = Math.Max(ans, sumV);
                            }
                        }
                    }
                }
            }

            Console.WriteLine(ans);
        }

        static class Console_
        {
            private static Queue<string> param = new Queue<string>();
            public static string NextString()
            {
                if (param.Count == 0) foreach (var item in Console.ReadLine().Split(' ')) param.Enqueue(item);
                return param.Dequeue();
            }
        }
        static int NextInt => int.Parse(Console_.NextString());
        static long NextLong => long.Parse(Console_.NextString());
        static string NextString => Console_.NextString();

        static IEnumerable<long> Prime(long x)
        {
            if (x < 2) yield break;
            yield return 2;
            var halfx = x / 2;
            var table = new bool[halfx + 1];
            var max = (long)(Math.Sqrt(x) / 2);
            for (long i = 1; i <= max; ++i)
            {
                if (table[i]) continue;
                var add = 2 * i + 1;
                yield return add;
                for (long j = 2 * i * (i + 1); j <= halfx; j += add) table[j] = true;
            }
            for (long i = max + 1; i <= halfx; ++i) if (!table[i]) yield return 2 * i + 1;
        }
        static IEnumerable<long> Divisor(long x)
        {
            if (x < 1) yield break;
            var max = (long)Math.Sqrt(x);
            for (long i = 1; i < max; ++i)
            {
                if (x % i != 0) continue;
                yield return i;
                if (i != x / i) yield return x / i;
            }
        }
        static long GCD(long a, long b)
        {
            long tmpa = a, tmpb = b;
            while (tmpb > 0) { var tmp = tmpb; tmpb = tmpa % tmpb; tmpa = tmp; }
            return tmpa;
        }
    }
}
