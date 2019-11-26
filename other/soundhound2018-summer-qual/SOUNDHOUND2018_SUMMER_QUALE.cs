using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Program
{
    public class SOUNDHOUND2018_SUMMER_QUALE
    {
        static public void Main(string[] args)
        {
            var n = NextInt;
            var m = NextInt;
            var edge = new List<Tuple<int, long>>[n];
            for (int i = 0; i < n; ++i)
            {
                edge[i] = new List<Tuple<int, long>>();
            }
            var vertexMaxNum = new long[n];
            for (int i = 0; i < n; ++i) vertexMaxNum[i] = 0;
            for (int i = 0; i < m; ++i)
            {
                var u = NextInt - 1;
                var v = NextInt - 1;
                var s = NextLong;
                edge[u].Add(Tuple.Create(v, s));
                edge[v].Add(Tuple.Create(u, s));
                if (vertexMaxNum[u] == 0) vertexMaxNum[u] = s - 1;
                if (vertexMaxNum[v] == 0) vertexMaxNum[v] = s - 1;
                if (vertexMaxNum[u] > s - 1) vertexMaxNum[u] = s - 1;
                if (vertexMaxNum[v] > s - 1) vertexMaxNum[v] = s - 1;
            }

            var checkNum = 0L;
            var checkV = -1;
            var vertexNumbers = new long[n];
            var vertexCoeff = new long[n];
            var vertexSigns = new long[n];
            {
                var doneList = new bool[n];
                var queue = new Queue<int>();
                queue.Enqueue(0);
                doneList[0] = true;
                vertexNumbers[0] = 1;
                vertexCoeff[0] = 0;
                vertexSigns[0] = 1;
                while (queue.Any() && checkV == -1)
                {
                    var v = queue.Dequeue();
                    foreach (var item in edge[v])
                    {
                        var vertex = item.Item1;
                        var cost = item.Item2;
                        if (!doneList[vertex])
                        {
                            doneList[vertex] = true;
                            vertexNumbers[vertex] = cost - vertexNumbers[v];
                            vertexSigns[vertex] = vertexSigns[v] == 1 ? -1 : 1;
                            vertexCoeff[vertex] = vertexNumbers[vertex] - vertexSigns[vertex];
                            queue.Enqueue(vertex);
                        }
                        else
                        {
                            if (vertexSigns[vertex] != vertexSigns[v])
                            {
                                if (vertexNumbers[vertex] != cost - vertexNumbers[v])
                                {
                                    Console.WriteLine(0);
                                    return;
                                }
                            }
                            else
                            {
                                if ((vertexNumbers[vertex] + cost - vertexNumbers[v]) % 2 != 0)
                                {
                                    Console.WriteLine(0);
                                    return;
                                }
                                checkV = vertex;
                                checkNum = (vertexNumbers[vertex] + cost - vertexNumbers[v]) / 2;
                                break;
                            }
                        }
                    }
                }
            }
            if (checkV != -1)
            {
                var doneList = new bool[n];
                var queue = new Queue<int>();
                var vertexNumbersChk = new long[n];
                queue.Enqueue(checkV);
                doneList[checkV] = true;
                vertexNumbersChk[checkV] = checkNum;
                if (vertexMaxNum[checkV] < vertexNumbersChk[checkV] || vertexNumbersChk[checkV] < 1)
                {
                    Console.WriteLine(0);
                    return;
                }
                while (queue.Any())
                {
                    var v = queue.Dequeue();
                    foreach (var item in edge[v])
                    {
                        var vertex = item.Item1;
                        var cost = item.Item2;
                        if (!doneList[vertex])
                        {
                            doneList[vertex] = true;
                            vertexNumbersChk[vertex] = cost - vertexNumbersChk[v];
                            queue.Enqueue(vertex);
                            if (vertexMaxNum[vertex] < vertexNumbersChk[vertex] || vertexNumbersChk[vertex] < 1)
                            {
                                Console.WriteLine(0);
                                return;
                            }
                        }
                        else
                        {
                            if (vertexNumbersChk[vertex] != cost - vertexNumbersChk[v])
                            {
                                Console.WriteLine(0);
                                return;
                            }
                        }
                    }
                }
                Console.WriteLine(1);
                return;
            }

            var maxX = long.MaxValue;
            var minX = long.MinValue;
            for (int i = 0; i < n; ++i)
            {
                if (vertexSigns[i] == 1)
                {
                    minX = Math.Max(minX, -vertexCoeff[i]);
                }
                else
                {
                    maxX = Math.Min(maxX, vertexCoeff[i]);
                }
            }


            Console.WriteLine(Math.Max(maxX - minX - 1, 0));
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
