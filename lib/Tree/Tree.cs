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
    partial class LIB_Tree
    {
        int N;
        List<int>[] path;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Tree(long n)
        {
            N = (int)n;
            path = Enumerable.Repeat(0, N).Select(_ => new List<int>()).ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long u, long v)
        {
            if (u >= N || v >= N) throw new Exception();
            path[u].Add((int)v);
            path[v].Add((int)u);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] GetSurround(long u) => path[u].ToArray();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<(long node, long parent)> BFSFromRoot(long root)
        {
            var bfsList = new List<(long node, long parent)>();
            var q = new Queue<int>();
            var done = new bool[N];
            bfsList.Add((root, -1));
            done[root] = true;
            q.Enqueue((int)root);
            while (q.Count > 0)
            {
                var w = q.Dequeue();
                foreach (var i in path[w])
                {
                    if (done[i]) continue;
                    done[i] = true;
                    q.Enqueue(i);
                    bfsList.Add((i, w));
                }
            }
            return bfsList;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long[] GetDistanceFrom(long root)
        {
            var ret = new long[N];
            foreach (var item in BFSFromRoot(root))
            {
                if (item.parent == -1) continue;
                ret[item.node] = ret[item.parent] + 1;
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] GetJuusin()
        {
            var center = new List<int>();
            var dp = new long[N];
            foreach (var item in BFSFromLeaf(0))
            {
                dp[item.node] += 1;
                var ok = false;
                foreach (var item2 in path[item.node])
                {
                    if (item2 == item.parent) continue;
                    dp[item.node] += dp[item2];
                    if (dp[item2] <= N / 2) ok = true;
                }
                if (ok)
                {
                    if (N - dp[item.node] <= N / 2)
                    {
                        center.Add((int)item.node);
                    }
                }
            }
            return center.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int[] GetTyokkei()
        {
            var dist1 = GetDistanceFrom(0);
            var maxPos = 0;
            var maxDist = 0L;
            for (var i = 0; i < N; i++)
            {
                if (maxDist < dist1[i])
                {
                    maxDist = dist1[i];
                    maxPos = i;
                }
            }
            var dist2 = new int[N];
            var parent = new int[N];
            maxDist = 0;
            var anotherMaxPos = 0L;
            foreach (var item in BFSFromRoot(maxPos))
            {
                parent[item.node] = (int)item.parent;
                if (item.parent == -1) continue;
                dist2[item.node] = dist2[item.parent] + 1;
                if (maxDist < dist2[item.node])
                {
                    maxDist = dist2[item.node];
                    anotherMaxPos = item.node;
                }
            }
            var nownode = (int)anotherMaxPos;
            var ret = new List<int>();
            while (nownode != -1)
            {
                ret.Add(nownode);
                nownode = parent[nownode];
            }
            return ret.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<(long node, long parent)> BFSFromLeaf(long root) => BFSFromRoot(root).ToArray().Reverse().ToList();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (long node, long parent, long direction)[] EulerTour(long root)
        {
            var eulerList = new (long node, long parent, long direction)[N << 1];
            ref var eulerListRef = ref eulerList[0];
            var stack = new int[N << 1];
            ref var stackref = ref stack[0];
            var si = 0;
            Unsafe.Add(ref stackref, si++) = (int)root + 1;
            var idx = 0;
            var par = new int[N];
            ref var parref = ref par[0];
            Unsafe.Add(ref parref, (int)root) = -1;
            while (si > 0)
            {
                var vtx = Unsafe.Add(ref stackref, --si);
                if (vtx < 0)
                {
                    vtx *= -1;
                    --vtx;
                    Unsafe.Add(ref eulerListRef, idx) = (vtx, Unsafe.Add(ref parref, vtx), -1);
                }
                else
                {
                    Unsafe.Add(ref stackref, si++) = -vtx;
                    --vtx;
                    var pare = Unsafe.Add(ref parref, vtx);
                    Unsafe.Add(ref eulerListRef, idx) = (vtx, pare, 1);
                    foreach (var item in path[vtx])
                    {
                        if (item == pare) continue;
                        Unsafe.Add(ref parref, item) = vtx;
                        Unsafe.Add(ref stackref, si++) = item + 1;
                    }
                }
                ++idx;
            }
            return eulerList;
        }
    }
    ////end
}