using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class ABC075D
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
            var K = Console_.NextInt();


            var xary = new long[N];
            var yary = new long[N];
            var pos = new Tuple<long, long>[N];
            for (int i = 0; i < N; ++i)
            {
                var x = Console_.NextLong();
                var y = Console_.NextLong();
                xary[i] = x;
                yary[i] = y;
                pos[i] = Tuple.Create(x, y);
            }
            Array.Sort(xary);
            Array.Sort(yary);

            var ans = 0L;
            for (int xl = 0; xl <= N - K; ++xl)
            {
                for (int xr = N - 1; xr >= K - 1; --xr)
                {
                    for (int yl = 0; yl <= N - K; ++yl)
                    {
                        for (int yr = N - 1; yr >= K - 1; --yr)
                        {
                            var count = 0;
                            for (int i = 0; i < N; ++i)
                            {
                                if (xary[xl] <= pos[i].Item1 && pos[i].Item1 <= xary[xr] &&
                                    yary[yl] <= pos[i].Item2 && pos[i].Item2 <= yary[yr])
                                {
                                    ++count;
                                }
                            }
                            if (count >= K)
                            {
                                var anstmp = (xary[xr] - xary[xl]) * (yary[yr] - yary[yl]);
                                if (ans == 0 || ans > anstmp) ans = anstmp;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(ans);
        }
    }
}
