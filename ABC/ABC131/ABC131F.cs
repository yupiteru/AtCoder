using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;

namespace Program
{
    public static class ABC131F
    {
        class UF
        {
            private Dictionary<long, long> _par = new Dictionary<long, long>();
            private Dictionary<long, long> _count = new Dictionary<long, long>();
            public long Root(long key)
            {
                if (!_par.ContainsKey(key)) Unite(key, key);
                if (_par[key] == key) return key;
                return _par[key] = Root(_par[key]);
            }
            public bool IsRoot(long key)
            {
                if (!_par.ContainsKey(key)) return false;
                return _par[key] == key;
            }
            public long Count(long key)
            {
                return _count[Root(key)];
            }
            public void Unite(long key1, long key2)
            {
                var k1 = _par.ContainsKey(key1);
                if (key1 == key2)
                {
                    if (!k1)
                    {
                        _par[key1] = key1;
                        _count[key1] = 1;
                    }
                    return;
                }
                var k2 = _par.ContainsKey(key2);
                if (!k1 && !k2)
                {
                    _par[key1] = key1;
                    _par[key2] = key1;
                    _count[key1] = 2;
                }
                else if (!k1 && k2)
                {
                    _par[key1] = Root(key2);
                    _count[_par[key1]]++;
                }
                else if (k1 && !k2)
                {
                    _par[key2] = Root(key1);
                    _count[_par[key2]]++;
                }
                else
                {
                    var r1 = Root(key1);
                    var r2 = Root(key2);
                    if (r1 == r2) return;
                    _count[r2] += _count[r1];
                    _par[r1] = r2;
                }
            }
        }
        static public void Solve()
        {
            var N = NextInt;
            var xy = Enumerable.Repeat(0, N).Select(_ => new { x = NextLong, y = NextLong }).ToList();
            var xDic = xy.GroupBy(e => e.x).ToDictionary(e => e.Key, e => e.Select(e2 => e2.y).ToList());
            var yDic = xy.GroupBy(e => e.y).ToDictionary(e => e.Key, e => e.Select(e2 => e2.x).ToList());
            var uf = new UF();
            foreach (var item in xDic)
            {
                if (item.Value.Count > 1)
                {
                    var keymaster = 0L;
                    foreach (var y in item.Value)
                    {
                        if (keymaster == 0L)
                        {
                            keymaster = item.Key * 10000000 + y;
                        }
                        var key = item.Key * 10000000 + y;
                        uf.Unite(keymaster, key);
                    }
                }
            }
            foreach (var item in yDic)
            {
                if (item.Value.Count > 1)
                {
                    var keymaster = 0L;
                    foreach (var x in item.Value)
                    {
                        if (keymaster == 0L)
                        {
                            keymaster = item.Key + x * 10000000;
                        }
                        var key = item.Key + x * 10000000;
                        uf.Unite(keymaster, key);
                    }
                }
            }
            var inputPointCount = 0L;
            var totalPointCount = 0L;
            var xCount = new Dictionary<long, HashSet<long>>();
            var yCount = new Dictionary<long, HashSet<long>>();
            foreach (var item in xy)
            {
                var key = item.x * 10000000 + item.y;
                var rt = uf.Root(key);
                if (!xCount.ContainsKey(rt))
                {
                    xCount[rt] = new HashSet<long>();
                }
                if (!yCount.ContainsKey(rt))
                {
                    yCount[rt] = new HashSet<long>();
                }
                xCount[rt].Add(item.x);
                yCount[rt].Add(item.y);
            }
            foreach (var item in xy)
            {
                var key = item.x * 10000000 + item.y;
                if (uf.IsRoot(key))
                {
                    if (uf.Count(key) > 2)
                    {
                        inputPointCount += uf.Count(key);
                        totalPointCount += xCount[key].LongCount() * yCount[key].LongCount();
                    }
                }
            }
            Console.WriteLine(totalPointCount - inputPointCount);
        }

        static public void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
                Console.SetOut(sw);
            }
            Solve();
            Console.Out.Flush();
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
        static double NextDouble => double.Parse(Console_.NextString());
        static string NextString => Console_.NextString();
    }
}
