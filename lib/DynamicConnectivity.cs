
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
        List<EulerTourTree> ett;
        List<HashMap[]> edges;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_DynamicConnectivity(long n, T ei, Func<T, T, T> f)
        {
            this.n = (int)n;
            this.f = f;
            this.ei = ei;
            ett = new List<EulerTourTree>();
            edges = new List<HashMap[]>();
            ett.Add(new EulerTourTree(this.n, ei, f));
            edges.Add(new HashMap[n]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Link(long s, long t)
        {
            if (s == t) return;
            if (ett[0].Link((int)s, (int)t)) return;
            var sEdges = edges[0][s];
            var tEdges = edges[0][t];
            if (sEdges == null) edges[0][s] = sEdges = new HashMap();
            if (tEdges == null) edges[0][t] = tEdges = new HashMap();
            sEdges.Add((ulong)t, 0);
            tEdges.Add((ulong)s, 0);
            if (sEdges.Count == 1) ett[0].EdgeConnectedUpdate((int)s, true);
            if (tEdges.Count == 1) ett[0].EdgeConnectedUpdate((int)t, true);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(long s, long t) => ett[0].IsSame((int)s, (int)t);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Size(long s) => ett[0].Size((int)s);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long s) => ett[0].Query((int)s);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Cut(long s, long t)
        {
            if (s == t) return false;
            for (var i = 0; i < edges.Count; i++)
            {
                var sEdges = edges[i][s];
                var tEdges = edges[i][t];
                if (sEdges == null) edges[i][s] = sEdges = new HashMap();
                if (tEdges == null) edges[i][t] = tEdges = new HashMap();
                sEdges.Remove((ulong)t);
                tEdges.Remove((ulong)s);
                if (sEdges.Count == 0) ett[i].EdgeConnectedUpdate((int)s, false);
                if (tEdges.Count == 0) ett[i].EdgeConnectedUpdate((int)t, false);
            }
            for (var i = ett.Count - 1; i >= 0; i--)
            {
                if (ett[i].Cut((int)s, (int)t))
                {
                    if (ett.Count - 1 == i)
                    {
                        ett.Add(new EulerTourTree(n, ei, f));
                        edges.Add(new HashMap[n]);
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
                var etti1 = ett[i + 1];
                var edgesi = edges[i];
                var edgesi1 = edges[i + 1];
                if (etti.Size(s) > etti.Size(t)) { var temp = s; s = t; t = temp; }
                etti.EdgeUpdate(s, (ss, tt) => etti1.Link(ss, tt));
                if (etti.TryReconnect(s, (x, idx) =>
                {
                    return edgesi[x].SearchAllKey((y, xidx) =>
                    {
                        if (edgesi[xidx].Count == 1) etti.EdgeConnectedUpdate(xidx, false);
                        if (edgesi[y].Count == 1) etti.EdgeConnectedUpdate((int)y, false);
                        edgesi[y].Remove((ulong)xidx);
                        if (etti.IsSame(xidx, (int)y))
                        {
                            var nextXEdges = edgesi1[xidx];
                            var nextYEdges = edgesi1[y];
                            if (nextXEdges == null) edgesi1[xidx] = nextXEdges = new HashMap();
                            if (nextYEdges == null) edgesi1[y] = nextYEdges = new HashMap();
                            nextXEdges.Add(y, 0);
                            nextYEdges.Add((ulong)xidx, 0);
                            if (nextXEdges.Count == 1) etti1.EdgeConnectedUpdate(xidx, true);
                            if (nextYEdges.Count == 1) etti1.EdgeConnectedUpdate((int)y, true);
                        }
                        else
                        {
                            for (var j = 0; j <= idx; j++) ett[j].Link(xidx, (int)y);
                            return 7;
                        }
                        return 2;
                    }, x);
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
            HashMap ptr;
            int startNodeIndex;
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
            static int NewNode(int l, int r)
            {
                if (nodeCount == nodeParent.Length)
                {
                    {
                        var tmp = new int[nodeChild.Length << 1];
                        Array.Copy(nodeChild, tmp, nodeChild.Length);
                        nodeChild = tmp;
                    }
                    {
                        var tmp = new int[nodeParent.Length << 1];
                        Array.Copy(nodeParent, tmp, nodeParent.Length);
                        nodeParent = tmp;
                    }
                    {
                        var tmp = new int[nodeL.Length << 1];
                        Array.Copy(nodeL, tmp, nodeL.Length);
                        nodeL = tmp;
                    }
                    {
                        var tmp = new int[nodeR.Length << 1];
                        Array.Copy(nodeR, tmp, nodeR.Length);
                        nodeR = tmp;
                    }
                    {
                        var tmp = new int[nodeSz.Length << 1];
                        Array.Copy(nodeSz, tmp, nodeSz.Length);
                        nodeSz = tmp;
                    }
                    {
                        var tmp = new T[nodeVal.Length << 1];
                        Array.Copy(nodeVal, tmp, nodeVal.Length);
                        nodeVal = tmp;
                    }
                    {
                        var tmp = new T[nodeSum.Length << 1];
                        Array.Copy(nodeSum, tmp, nodeSum.Length);
                        nodeSum = tmp;
                    }
                    {
                        var tmp = new byte[nodeFlags.Length << 1];
                        Array.Copy(nodeFlags, tmp, nodeFlags.Length);
                        nodeFlags = tmp;
                    }
                }
                var id = nodeCount++;
                nodeL[id] = l;
                nodeR[id] = r;
                nodeSz[id] = l == r ? 1 : 0;
                if (l < r) nodeFlags[id] = 12;
                return id;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EulerTourTree(int n, T ei, Func<T, T, T> f)
            {
                this.ptr = new HashMap();
                this.f = f;
                this.ei = ei;
                this.startNodeIndex = nodeCount;
                for (var i = 0; i < n; i++)
                {
                    var nodePoint = NewNode(i, i);
                    nodeVal[nodePoint] = nodeSum[nodePoint] = ei;
                }
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
                var t = startNodeIndex + s;
                Splay(t);
                return nodeSz[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSame(int s, int t) => IsSameNode(startNodeIndex + s, startNodeIndex + t);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Get(int s)
            {
                var t = startNodeIndex + s;
                Splay(t);
                return nodeVal[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(int s, T x)
            {
                var t = startNodeIndex + s;
                Splay(t);
                nodeVal[t] = x;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeUpdate(int s, Action<int, int> g)
            {
                var t = startNodeIndex + s;
                Splay(t);
                while ((nodeFlags[t] & 4) != 0)
                {
                    var node = t;
                    while (true)
                    {
                        if (nodeL[node] < nodeR[node] && (nodeFlags[node] & 8) != 0)
                        {
                            Splay(node);
                            nodeFlags[node] &= 7;
                            g(nodeL[node], nodeR[node]);
                            break;
                        }
                        if ((nodeFlags[nodeChild[node << 1]] & 4) != 0) node = nodeChild[node << 1];
                        else node = nodeChild[(node << 1) | 1];
                    }
                    Splay(t);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryReconnect(int s, Func<int, int, bool> f, int idx)
            {
                var t = startNodeIndex + s;
                Splay(t);
                while ((nodeFlags[t] & 1) != 0)
                {
                    var node = t;
                    while (true)
                    {
                        if ((nodeFlags[node] & 2) != 0)
                        {
                            Splay(node);
                            if (f(nodeL[node], idx)) return true;
                            break;
                        }
                        if ((nodeFlags[nodeChild[node << 1]] & 1) != 0) node = nodeChild[node << 1];
                        else node = nodeChild[(node << 1) | 1];
                    }
                    Splay(t);
                }
                return false;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeConnectedUpdate(int s, bool b)
            {
                var t = startNodeIndex + s;
                Splay(t);
                if (b) nodeFlags[t] |= 2;
                else nodeFlags[t] &= 13;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Link(int l, int r)
            {
                if (l > r) { var t = l; l = r; r = t; }
                if (IsSameNode(startNodeIndex + l, startNodeIndex + r)) return false;
                var lv = ReRoot(startNodeIndex + l);
                var rv = ReRoot(startNodeIndex + r);
                var lrnode = ptr.GetOrInsert(((ulong)l << 32) | (uint)r, () =>
                {
                    NewNode(l, r);
                    return NewNode(r, l) - 1;
                });
                nodeParent[lv] = nodeParent[rv] = lrnode;
                nodeChild[lrnode << 1] = lv;
                nodeChild[(lrnode << 1) | 1] = rv;
                UpdateNode(lrnode);
                Merge(lrnode, lrnode + 1);
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Cut(int l, int r)
            {
                if (l > r) { var t = l; l = r; r = t; }
                if (!ptr.ContainsKey(((ulong)l << 32) | (uint)r)) return false;
                var p = ptr.GetAndErase(((ulong)l << 32) | (uint)r);
                var s = Split(p, p + 1);
                Merge(s.Item1, s.Item3);
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Query(int p, int v)
            {
                Cut(p, v);
                var t = startNodeIndex + v;
                Splay(t);
                var res = nodeSum[t];
                Link(p, v);
                return res;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Query(int s)
            {
                var t = startNodeIndex + s;
                Splay(t);
                return nodeSum[t];
            }
        }
        class HashMap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int Hash(ulong key)
            {
                unchecked
                {
                    key = (~key) + (key << 18);
                    key = key ^ (key >> 31);
                    key = key * 21;
                    key = key ^ (key >> 11);
                    key = key + (key << 6);
                    key = key ^ (key >> 22);
                    return (int)key;
                }
            }
            int[] st;
            (ulong, int)[] bck;
            int mask;
            int prode;
            public int Count
            {
                get;
                private set;
            }
            int minElem;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public HashMap()
            {
                prode = -1;
                st = new int[0];
                bck = new (ulong, int)[0];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int FindEmpty(ulong key)
            {
                var h = Hash(key);
                for (var delta = 0; ; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (st[i] != 2)
                    {
                        if (prode < delta) prode = delta;
                        return i;
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int FindFilled(ulong key)
            {
                if (Count == 0) return -1;
                var h = Hash(key);
                for (var delta = 0; delta <= prode; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (st[i] == 2)
                    {
                        if (bck[i].Item1 == key) return i;
                    }
                    else if (st[i] == 0) return -1;
                }
                return -1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int FindOrAllocate(ulong key)
            {
                var h = Hash(key);
                var hole = -1;
                var delta = 0;
                for (; delta <= prode; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (st[i] == 2)
                    {
                        if (bck[i].Item1 == key) return i;
                    }
                    else if (st[i] == 0) return i;
                    else
                    {
                        if (hole == -1) hole = i;
                    }
                }
                if (hole != -1) return hole;
                for (; ; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (st[i] != 2)
                    {
                        prode = delta;
                        return i;
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Reserve(int nextCnt)
            {
                var requiredCnt = nextCnt + (nextCnt >> 1) + 1;
                if (requiredCnt > bck.Length)
                {
                    nextCnt = 4;
                    while (nextCnt < requiredCnt) nextCnt <<= 1;
                }
                else if (nextCnt <= bck.Length >> 2) nextCnt = Max(4, bck.Length >> 1);
                else return;
                var oldSt = new int[nextCnt];
                var oldBck = new (ulong, int)[nextCnt];
                { var t = oldSt; oldSt = st; st = t; }
                { var t = oldBck; oldBck = bck; bck = t; }
                mask = nextCnt - 1;
                Count = 0;
                prode = 0;
                minElem = nextCnt - 1;
                for (var pos = 0; pos < oldBck.Length; ++pos)
                {
                    if (oldSt[pos] == 2)
                    {
                        var i = FindEmpty(oldBck[pos].Item1);
                        st[i] = 2;
                        bck[i] = oldBck[pos];
                        minElem = Min(minElem, i);
                        ++Count;
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(ulong key, int val)
            {
                Reserve(Count + 1);
                var i = FindOrAllocate(key);
                if (st[i] != 2)
                {
                    st[i] = 2;
                    bck[i] = (key, val);
                    minElem = Min(minElem, i);
                    ++Count;
                }
                else bck[i] = (key, val);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Remove(ulong key)
            {
                var i = FindFilled(key);
                if (i == -1) return false;
                st[i] = 1;
                --Count;
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsKey(ulong key) => FindFilled(key) != -1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetOrInsert(ulong key, Func<int> val)
            {
                Reserve(Count + 1);
                var i = FindOrAllocate(key);
                if (st[i] != 2)
                {
                    st[i] = 2;
                    bck[i] = (key, val());
                    minElem = Min(minElem, i);
                    ++Count;
                }
                return bck[i].Item2;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetAndErase(ulong key)
            {
                var i = FindFilled(key);
                st[i] = 1;
                --Count;
                return bck[i].Item2;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool SearchAllKey(Func<ulong, int, int> func, int idx)
            {
                for (var i = minElem; i < bck.Length; ++i)
                {
                    if (st[i] == 2)
                    {
                        minElem = i;
                        var res = func(bck[i].Item1, idx);
                        if ((res & 2) != 0)
                        {
                            st[i] = 1;
                            --Count;
                        }
                        if ((res & 1) != 0) return (res & 4) != 0;
                    }
                }
                return false;
            }
        }
    }
    ////end
}