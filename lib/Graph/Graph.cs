
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Library
{
    ////start
    static class LIB_Graph
    {
        public static (long cost, long[] history) AStar(long N, long start, Func<long, bool> isGoal, Func<long, (long v, long c)[]> listupPaths, Func<long, long> h)
        {
            var dist = Enumerable.Repeat(long.MaxValue >> 2, (int)N).ToArray();
            var pred = new int?[N];
            dist[start] = 0;
            var q = new LIB_PriorityQueue();
            q.Push(0, (int)start);
            var goalNode = -1;
            while (0 < q.Count)
            {
                var u = q.Pop();
                if (isGoal(u.Value))
                {
                    goalNode = u.Value;
                    break;
                }
                if (dist[u.Value] < u.Key) continue;
                foreach (var pathItem in listupPaths(u.Value))
                {
                    var v = pathItem.v;
                    var alt = u.Key - h(u.Value) + pathItem.c + h(v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        pred[v] = u.Value;
                        q.Push(alt, (int)v);
                    }
                }
            }
            if (goalNode == -1) return (long.MaxValue, new long[0]);
            var history = new List<long>();
            for (var u = goalNode; u != start; u = pred[u].Value) history.Add(u);
            history.Add(start);
            history.Reverse();
            return (dist[goalNode], history.ToArray());
        }
    }
    ////end
}