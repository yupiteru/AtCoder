
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
    class LIB_TSP
    {
        int n;
        List<(int, ulong)>[] path;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_TSP(long n)
        {
            this.n = (int)n;
            path = new List<(int, ulong)>[n];
            for (var i = 0; i < n; ++i) path[i] = new List<(int, ulong)>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPath(long from, long to, long cost)
        {
            path[from].Add(((int)to, (ulong)cost));
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public ulong TSP(long[] vtxies, long start = -1, bool loop = false)
        {
            var K = vtxies.Length;
            if (start != -1)
            {
                var vtxiesList = new long[++K];
                var ok = true;
                for (var i = 0; i < vtxies.Length; ++i)
                {
                    vtxiesList[i] = vtxies[i];
                    if (vtxies[i] == start)
                    {
                        ok = false;
                        start = i;
                        --K;
                        break;
                    }
                }
                if (ok)
                {
                    vtxiesList[K - 1] = start;
                    start = K - 1;
                    vtxies = vtxiesList;
                }
            }
            var dist = new ulong[K, K];
            ref var distref = ref dist[0, 0];
            var dist2 = new ulong[n];
            ref var dist2ref = ref dist2[0];
            for (var i = 0; i < K; ++i)
            {
                Unsafe.InitBlock(ref Unsafe.As<ulong, byte>(ref dist2ref), 255, (uint)n << 3);
                Unsafe.Add(ref dist2ref, (int)vtxies[i]) = 0;
                var q2 = new LIB_PriorityQueue();
                q2.Push(0, (int)vtxies[i]);
                while (q2.Count > 0)
                {
                    var u = q2.Pop().Item2;
                    foreach (var pathItem in path[u])
                    {
                        if (Unsafe.Add(ref dist2ref, u) == ulong.MaxValue) continue;
                        var v = pathItem.Item1;
                        var alt = Unsafe.Add(ref dist2ref, u) + pathItem.Item2;
                        if (Unsafe.Add(ref dist2ref, v) > alt)
                        {
                            Unsafe.Add(ref dist2ref, v) = alt;
                            q2.Push((long)alt, v);
                        }
                    }
                }
                for (var j = 0; j < K; ++j) Unsafe.Add(ref distref, i * K + j) = Unsafe.Add(ref dist2ref, (int)vtxies[j]);
            }
            var ten = 1 << K;
            var dp = new ulong[ten, K];
            ref var dpref = ref dp[0, 0];
            Unsafe.InitBlock(ref Unsafe.As<ulong, byte>(ref dpref), 255, (uint)(ten * K) << 3);
            if (start == -1)
            {
                for (var i = 0; i < K; ++i) Unsafe.Add(ref dpref, (K << i) + i) = 0;
            }
            else Unsafe.Add(ref dpref, (int)((K << (int)start) + start)) = 0;
            for (var i = 1; i < ten; ++i)
            {
                for (var j = 0; j < K; ++j)
                {
                    var ij = K * i + j;
                    if (Unsafe.Add(ref dpref, ij) == ulong.MaxValue) continue;
                    for (var l = 0; l < K; ++l)
                    {
                        if ((i & (1 << l)) != 0) continue;
                        var jl = j * K + l;
                        if (Unsafe.Add(ref distref, jl) == ulong.MaxValue) continue;
                        var idx = K * (i | (1 << l)) + l;
                        var v = Unsafe.Add(ref dpref, ij) + Unsafe.Add(ref distref, jl);
                        if (Unsafe.Add(ref dpref, idx) > v) Unsafe.Add(ref dpref, idx) = v;
                    }
                }
            }
            ten = (ten - 1) * K;
            var ret = ulong.MaxValue;
            if (loop)
            {
                for (var i = 0; i < K; ++i)
                {
                    var v = Unsafe.Add(ref dpref, ten + i) + dist[i, start];
                    if (ret > v) ret = v;
                }
            }
            else
            {
                for (var i = 0; i < K; ++i)
                {
                    var v = Unsafe.Add(ref dpref, ten + i);
                    if (ret > v) ret = v;
                }
            }
            return ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public ulong TSP(long start = -1, bool loop = false)
        {
            var dist = new ulong[n, n];
            ref var distref = ref dist[0, 0];
            for (var i = 0; i < n; ++i)
            {
                foreach (var item in path[i]) Unsafe.Add(ref distref, i * n + item.Item1) = item.Item2;
            }
            var ten = 1 << n;
            var dp = new ulong[ten, n];
            ref var dpref = ref dp[0, 0];
            Unsafe.InitBlock(ref Unsafe.As<ulong, byte>(ref dpref), 255, (uint)(ten * n) << 3);
            if (start == -1)
            {
                for (var i = 0; i < n; ++i) Unsafe.Add(ref dpref, (n << i) + i) = 0;
            }
            else Unsafe.Add(ref dpref, (int)((n << (int)start) + start)) = 0;
            for (var i = 1; i < ten; ++i)
            {
                for (var j = 0; j < n; ++j)
                {
                    var ij = n * i + j;
                    if (Unsafe.Add(ref dpref, ij) == ulong.MaxValue) continue;
                    for (var l = 0; l < n; ++l)
                    {
                        if ((i & (1 << l)) != 0) continue;
                        var idx = n * (i | (1 << l)) + l;
                        var v = Unsafe.Add(ref dpref, ij) + Unsafe.Add(ref distref, j * n + l);
                        if (Unsafe.Add(ref dpref, idx) > v) Unsafe.Add(ref dpref, idx) = v;
                    }
                }
            }
            ten = (ten - 1) * n;
            var ret = ulong.MaxValue;
            if (loop)
            {
                for (var i = 0; i < n; ++i)
                {
                    var v = Unsafe.Add(ref dpref, ten + i) + dist[i, start];
                    if (ret > v) ret = v;
                }
            }
            else
            {
                for (var i = 0; i < n; ++i)
                {
                    var v = Unsafe.Add(ref dpref, ten + i);
                    if (ret > v) ret = v;
                }
            }
            return ret;
        }
    }
    ////end
}