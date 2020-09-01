
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
    class LIB_DynamicConnectivity<T>
    {
        Func<T, T, T> f;
        T ei;
        int n;
        int dep = 1;
        List<EulerTourTree> ett;
        List<HashSet<int>[]> edges;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DynamicConnectivity(long n, T ei, Func<T, T, T> f)
        {
            this.n = (int)n;
            this.f = f;
            this.ei = ei;
            ett = new List<EulerTourTree>();
            edges = new List<HashSet<int>[]>();
            ett.Add(new EulerTourTree(this.n, ei, f));
            edges.Add(new HashSet<int>[n]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Link(long s, long t)
        {
            if (s == t) return false;
            if (ett[0].Link((int)s, (int)t)) return true;
            var sEdges = edges[0][s];
            var tEdges = edges[0][t];
            if (sEdges == null) edges[0][s] = sEdges = new HashSet<int>();
            if (tEdges == null) edges[0][t] = tEdges = new HashSet<int>();
            sEdges.Add((int)t);
            tEdges.Add((int)s);
            if (sEdges.Count == 1) ett[0].EdgeConnectedUpdate((int)s, true);
            if (tEdges.Count == 1) ett[0].EdgeConnectedUpdate((int)t, true);
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(long s, long t) => ett[0].IsSame((int)s, (int)t);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Size(long s) => ett[0].Size((int)s);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetSum(long s) => ett[0].GetSum((int)s);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Cut(long s, long t)
        {
            if (s == t) return false;
            for (var i = 0; i < dep; i++)
            {
                var sEdges = edges[i][s];
                var tEdges = edges[i][t];
                if (sEdges == null) edges[i][s] = sEdges = new HashSet<int>();
                if (tEdges == null) edges[i][t] = tEdges = new HashSet<int>();
                sEdges.Remove((int)t);
                tEdges.Remove((int)s);
                if (sEdges.Count == 0) ett[i].EdgeConnectedUpdate((int)s, false);
                if (tEdges.Count == 0) ett[i].EdgeConnectedUpdate((int)t, false);
            }
            for (var i = dep - 1; i >= 0; i--)
            {
                if (ett[i].Cut((int)s, (int)t))
                {
                    if (dep - 1 == i)
                    {
                        ++dep;
                        ett.Add(new EulerTourTree(n, ei, f));
                        edges.Add(new HashSet<int>[n]);
                    }
                    return !TryReconnect((int)s, (int)t, i);
                }
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryReconnect(int s, int t, int k)
        {
            for (var i = 0; i < k; i++) ett[i].Cut(s, t);
            for (var i = k; i >= 0; i--)
            {
                var etti = ett[i];
                if (etti.Size(s) > etti.Size(t)) { var temp = s; s = t; t = temp; }
                etti.EdgeUpdate(s, (ss, tt, idx) => ett[idx + 1].Link(ss, tt), i);
                if (etti.TryReconnect(s, (x, idx) =>
                {
                    var xEdges = edges[idx][x];
                    if (xEdges == null) edges[idx][x] = xEdges = new HashSet<int>();
                    foreach (var y in xEdges.ToArray())
                    {
                        xEdges.Remove(y);
                        var yEdges = edges[idx][y];
                        if (yEdges == null) edges[idx][y] = yEdges = new HashSet<int>();
                        yEdges.Remove(x);
                        if (xEdges.Count == 0) ett[idx].EdgeConnectedUpdate(x, false);
                        if (yEdges.Count == 0) ett[idx].EdgeConnectedUpdate(y, false);
                        if (ett[idx].IsSame(x, y))
                        {
                            var nextXEdges = edges[idx + 1][x];
                            var nextYEdges = edges[idx + 1][y];
                            if (nextXEdges == null) edges[idx + 1][x] = nextXEdges = new HashSet<int>();
                            if (nextYEdges == null) edges[idx + 1][y] = nextYEdges = new HashSet<int>();
                            nextXEdges.Add(y);
                            nextYEdges.Add(x);
                            if (nextXEdges.Count == 1) ett[idx + 1].EdgeConnectedUpdate(x, true);
                            if (nextYEdges.Count == 1) ett[idx + 1].EdgeConnectedUpdate(y, true);
                        }
                        else
                        {
                            for (var j = 0; j <= idx; j++) ett[j].Link(x, y);
                            return true;
                        }
                    }
                    return false;
                }, i)) return true;
            }
            return false;
        }
        public T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ett[0].Get((int)index); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { ett[0].Update((int)index, value); }
        }
        class EulerTourTree
        {
            Func<T, T, T> f;
            T ei;
            Dictionary<int, int>[] ptr;
            static int[] nodeChild = new int[32768];
            static int[] nodeParent = new int[16384];
            static int[] nodeL = new int[16384];
            static int[] nodeR = new int[16384];
            static int[] nodeSz = new int[16384];
            static T[] nodeVal = new T[16384];
            static T[] nodeSum = new T[16384];
            static byte[] nodeFlags = new byte[16384];
            static int nodeCount = 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int NewNode()
            {
                if (nodeCount == nodeParent.Length)
                {
                    {
                        var tmp = new int[nodeCount << 2];
                        var cnt = nodeCount << 1;
                        for (var i = 0; i < cnt; i++) tmp[i] = nodeChild[i];
                        nodeChild = tmp;
                    }
                    {
                        var tmp = new int[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeParent[i];
                        nodeParent = tmp;
                    }
                    {
                        var tmp = new int[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeL[i];
                        nodeL = tmp;
                    }
                    {
                        var tmp = new int[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeR[i];
                        nodeR = tmp;
                    }
                    {
                        var tmp = new int[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeSz[i];
                        nodeSz = tmp;
                    }
                    {
                        var tmp = new T[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeVal[i];
                        nodeVal = tmp;
                    }
                    {
                        var tmp = new T[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeSum[i];
                        nodeSum = tmp;
                    }
                    {
                        var tmp = new byte[nodeCount << 1];
                        for (var i = 0; i < nodeCount; i++) tmp[i] = nodeFlags[i];
                        nodeFlags = tmp;
                    }
                }
                return nodeCount++;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int NewNode(int l, int r)
            {
                var id = NewNode();
                nodeL[id] = l;
                nodeR[id] = r;
                nodeSz[id] = l == r ? 1 : 0;
                if (l < r) nodeFlags[id] = 12;
                return id;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EulerTourTree(int n, T ei, Func<T, T, T> f)
            {
                this.ptr = new Dictionary<int, int>[n];
                this.f = f;
                this.ei = ei;
                for (var i = 0; i < n; i++)
                {
                    ptr[i] = new Dictionary<int, int>();
                    var nodePoint = ptr[i][i] = NewNode(i, i);
                    nodeVal[nodePoint] = nodeSum[nodePoint] = ei;
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int GetNode(int l, int r)
            {
                if (!ptr[l].ContainsKey(r)) ptr[l][r] = NewNode(l, r);
                return ptr[l][r];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int Root(int t)
            {
                while (nodeParent[t] != 0) t = nodeParent[t];
                return t;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsSameNode(int s, int t)
            {
                Splay(s);
                Splay(t);
                return Root(s) == Root(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int ReRoot(int t)
            {
                if (t == 0) return 0;
                var s = Split(t);
                return Merge(s.Item2, s.Item1);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int) Split(int s)
            {
                if (s == 0) return (0, 0);
                Splay(s);
                var t = nodeChild[s << 1];
                nodeChild[s << 1] = nodeParent[t] = 0;
                return (t, UpdateNode(s));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int) Split2(int s)
            {
                if (s == 0) return (0, 0);
                Splay(s);
                var t = nodeChild[s << 1];
                var u = nodeChild[(s << 1) | 1];
                nodeChild[s << 1] = nodeParent[t] = nodeChild[(s << 1) | 1] = nodeParent[u] = 0;
                return (t, u);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int, int) Split(int s, int t)
            {
                var u = Split2(s);
                if (IsSameNode(u.Item1, t))
                {
                    var r = Split2(t);
                    return (r.Item1, r.Item2, u.Item2);
                }
                else
                {
                    var r = Split2(t);
                    return (u.Item1, r.Item1, r.Item2);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int Merge(int s, int t)
            {
                if (s == 0) return t;
                if (t == 0) return s;
                while (nodeChild[(s << 1) | 1] != 0) s = nodeChild[(s << 1) | 1];
                Splay(s);
                nodeChild[(s << 1) | 1] = t;
                if (t != 0) nodeParent[t] = s;
                return UpdateNode(s);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int UpdateNode(int t)
            {
                if (t == 0) return 0;
                var sum = ei;
                var leftChild = nodeChild[t << 1];
                var rightChild = nodeChild[(t << 1) | 1];
                if (leftChild != 0) sum = f(sum, nodeSum[leftChild]);
                if (nodeL[t] == nodeR[t]) sum = f(sum, nodeVal[t]);
                if (rightChild != 0) sum = f(sum, nodeSum[rightChild]);
                nodeSum[t] = sum;
                nodeSz[t] = nodeSz[leftChild] + (nodeL[t] == nodeR[t] ? 1 : 0) + nodeSz[rightChild];
                var flag = nodeFlags[leftChild] | (nodeFlags[t] >> 1) | nodeFlags[rightChild];
                if ((flag & 1) != 0) nodeFlags[t] |= 1;
                else nodeFlags[t] &= 14;
                if ((flag & 4) != 0) nodeFlags[t] |= 4;
                else nodeFlags[t] &= 11;
                return t;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Push(int t)
            {
                // todo
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Rot(int t, int b)
            {
                if (t == 0) return;
                var x = nodeParent[t];
                var y = nodeParent[x];
                if ((nodeChild[(x << 1) | (1 - b)] = nodeChild[(t << 1) | b]) != 0) nodeParent[nodeChild[(t << 1) | b]] = x;
                nodeChild[(t << 1) | b] = x;
                nodeParent[x] = t;
                UpdateNode(x);
                UpdateNode(t);
                if ((nodeParent[t] = y) != 0)
                {
                    if (nodeChild[y << 1] == x) nodeChild[y << 1] = t;
                    if (nodeChild[(y << 1) | 1] == x) nodeChild[(y << 1) | 1] = t;
                    UpdateNode(y);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Splay(int t)
            {
                if (t == 0) return;
                Push(t);
                while (nodeParent[t] != 0)
                {
                    var q = nodeParent[t];
                    if (nodeParent[q] == 0)
                    {
                        Push(q);
                        Push(t);
                        Rot(t, nodeChild[q << 1] == t ? 1 : 0);
                    }
                    else
                    {
                        var r = nodeParent[q];
                        Push(r);
                        Push(q);
                        Push(t);
                        var b = nodeChild[r << 1] == q ? 1 : 0;
                        if (nodeChild[(q << 1) | (1 - b)] == t)
                        {
                            Rot(q, b);
                            Rot(t, b);
                        }
                        else
                        {
                            Rot(t, 1 - b);
                            Rot(t, b);
                        }
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Size(int s)
            {
                var t = GetNode(s, s);
                Splay(t);
                return nodeSz[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSame(int s, int t) => IsSameNode(GetNode(s, s), GetNode(t, t));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Get(int s)
            {
                var t = GetNode(s, s);
                Splay(t);
                return nodeVal[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(int s, T x)
            {
                var t = GetNode(s, s);
                Splay(t);
                nodeVal[t] = x;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeUpdate(int s, Action<int, int, int> g, int idx)
            {
                var t = GetNode(s, s);
                Splay(t);
                Action<int> dfs = null;
                dfs = node =>
                {
                    if (nodeL[node] < nodeR[node] && (nodeFlags[node] & 8) != 0)
                    {
                        Splay(node);
                        nodeFlags[node] &= 7;
                        g(nodeL[node], nodeR[node], idx);
                        return;
                    }
                    if ((nodeFlags[nodeChild[node << 1]] & 4) != 0) dfs(nodeChild[node << 1]);
                    else dfs(nodeChild[(node << 1) | 1]);
                };
                while ((nodeFlags[t] & 4) != 0)
                {
                    dfs(t);
                    Splay(t);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryReconnect(int s, Func<int, int, bool> f, int idx)
            {
                var t = GetNode(s, s);
                Splay(t);
                Func<int, bool> dfs = null;
                dfs = node =>
                {
                    if ((nodeFlags[node] & 2) != 0)
                    {
                        Splay(node);
                        return f(nodeL[node], idx);
                    }
                    if ((nodeFlags[nodeChild[node << 1]] & 1) != 0) return dfs(nodeChild[node << 1]);
                    else return dfs(nodeChild[(node << 1) | 1]);
                };
                while ((nodeFlags[t] & 1) != 0)
                {
                    if (dfs(t)) return true;
                    Splay(t);
                }
                return false;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeConnectedUpdate(int s, bool b)
            {
                var t = GetNode(s, s);
                Splay(t);
                if (b) nodeFlags[t] |= 2;
                else nodeFlags[t] &= 13;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Link(int l, int r)
            {
                if (IsSame(l, r)) return false;
                Merge(Merge(Merge(ReRoot(GetNode(l, l)), GetNode(l, r)), ReRoot(GetNode(r, r))), GetNode(r, l));
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Cut(int l, int r)
            {
                if (!ptr[l].ContainsKey(r)) return false;
                var s = Split(GetNode(l, r), GetNode(r, l));
                Merge(s.Item1, s.Item3);
                var p = ptr[l][r];
                var q = ptr[r][l];
                ptr[l].Remove(r);
                ptr[r].Remove(l);
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetSum(int p, int v)
            {
                Cut(p, v);
                var t = GetNode(v, v);
                Splay(t);
                var res = nodeSum[t];
                Link(p, v);
                return res;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetSum(int s)
            {
                var t = GetNode(s, s);
                Splay(t);
                return nodeSum[t];
            }
        }
    }
    ////end
}