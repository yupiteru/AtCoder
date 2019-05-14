using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC089D
    {
        static public void Main(string[] args)
        {
            var H = NextInt;
            var W = NextInt;
            var D = NextInt;
            var pos = new Tuple<int, int>[H * W];
            for (int i = 0; i < H; ++i)
            {
                for (int j = 0; j < W; ++j)
                {
                    pos[NextInt - 1] = Tuple.Create(i, j);
                }
            }
            var sum = new int[H * W];
            for (int idx = D; idx < H * W; idx += D)
            {
                for (int off = 0; off < D && idx + off < H * W; ++off)
                {
                    sum[idx + off] = sum[idx - D + off] + Math.Abs(pos[idx - D + off].Item1 - pos[idx + off].Item1) + Math.Abs(pos[idx - D + off].Item2 - pos[idx + off].Item2);
                }
            }

            var Q = NextInt;
            for (int i = 0; i < Q; ++i)
            {
                var L = NextInt;
                var R = NextInt;
                Console.WriteLine(sum[R - 1] - sum[L - 1]);
            }

            Console.WriteLine();
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
