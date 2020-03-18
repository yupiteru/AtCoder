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
    class LIB_PersistentQueue<T>
    {
        int DOUBLING_MAX = 30;
        class Node
        {
            public int[] p;
            public int limit;
            public T v;
            public bool isDequeueNode;
        }
        List<Node> info;
        public LIB_PersistentQueue()
        {
            info = new List<Node>();
        }
        public long Enqueue(long t, T x)
        {
            var ret = info.Count;
            var n = new Node();
            n.v = x;
            n.p = Enumerable.Repeat(ret, DOUBLING_MAX).ToArray();
            n.limit = -1;
            if (t >= 0)
            {
                n.p[0] = (int)t;
                n.limit = info[n.p[0]].limit;
                if (info[n.p[0]].isDequeueNode) n.p[0] = info[n.p[0]].p[0];
                for (var i = 1; i < DOUBLING_MAX; i++) n.p[i] = info[n.p[i - 1]].p[i - 1];
            }
            info.Add(n);
            return ret;
        }
        int FrontIdx(int t)
        {
            var idx = t;
            var limit = info[idx].limit;
            if (info[t].isDequeueNode) idx = info[t].p[0];
            for (var i = DOUBLING_MAX - 1; i >= 0; i--)
            {
                var nextIdx = info[idx].p[i];
                if (nextIdx > limit) idx = nextIdx;
            }
            return idx;
        }
        public T Peek(long t) => info[FrontIdx((int)t)].v;
        public long Dequeue(long t)
        {
            var idx = (int)t;
            var n = new Node();
            n.limit = FrontIdx(idx);
            n.p = new[] { idx };
            if (info[idx].isDequeueNode) n.p[0] = info[idx].p[0];
            n.isDequeueNode = true;
            info.Add(n);
            return info.Count - 1;
        }
    }
    ////end
}