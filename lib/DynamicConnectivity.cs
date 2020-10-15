
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
                    var nextXEdges = edgesi1[x];
                    var xEdges = edgesi[x];
                    return xEdges.SearchAllKey((y, xidx) =>
                    {
                        var yEdges = edgesi[y];
                        if (xEdges.Count == 1) etti.EdgeConnectedUpdate(xidx, false);
                        if (yEdges.Count == 1) etti.EdgeConnectedUpdate((int)y, false);
                        yEdges.Remove((ulong)xidx);
                        if (etti.IsSame(xidx, (int)y))
                        {
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
            static int Tsz = Unsafe.SizeOf<T>();
            int startNodeIndex;
            static int[] nodeChild = new int[6000000];
            static int[] nodeParent = new int[3000000];
            static int[] nodeL = new int[3000000];
            static int[] nodeR = new int[3000000];
            static int[] nodeSz = new int[3000000];
            static T[] nodeVal = new T[3000000];
            static T[] nodeSum = new T[3000000];
            static byte[] nodeFlags = new byte[3000000];
            static int nodeCount = 1;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int NewNode(int l, int r)
            {
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
                ref T nodeValref = ref nodeVal[0];
                ref T nodeSumref = ref nodeSum[0];
                for (var i = 0; i < n; i++)
                {
                    var nodePoint = NewNode(i, i);
                    Unsafe.Add(ref nodeValref, nodePoint) = Unsafe.Add(ref nodeSumref, nodePoint) = ei;
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int Root(int t, ref int nodeParentRef)
            {
                var t2 = nodeParentRef;
                while ((t2 = Unsafe.Add(ref nodeParentRef, t)) != 0) t = t2;
                return t;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsSameNode(int s, int t, ref int nodeChildRef, ref int nodeParentRef)
            {
                Splay(s, ref nodeChildRef, ref nodeParentRef);
                Splay(t, ref nodeChildRef, ref nodeParentRef);
                return Root(s, ref nodeParentRef) == Root(t, ref nodeParentRef);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int ReRoot(int t, ref int nodeChildRef, ref int nodeParentRef)
            {
                if (t == 0) return 0;
                var s = Split(t, ref nodeChildRef, ref nodeParentRef);
                return Merge(s.Item2, s.Item1, ref nodeChildRef, ref nodeParentRef);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int) Split(int s, ref int nodeChildRef, ref int nodeParentRef)
            {
                Splay(s, ref nodeChildRef, ref nodeParentRef);
                ref int ncs = ref Unsafe.Add(ref nodeChildRef, s << 1);
                var t = ncs;
                ncs = Unsafe.Add(ref nodeParentRef, t) = 0;
                return (t, UpdateNode(s));
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int) Split2(int s, ref int nodeChildRef, ref int nodeParentRef)
            {
                if (s == 0) return (0, 0);
                Splay(s, ref nodeChildRef, ref nodeParentRef);
                var ss = s << 1;
                ref int ncss = ref Unsafe.Add(ref nodeChildRef, ss);
                ref int ncss1 = ref Unsafe.Add(ref nodeChildRef, ss | 1);
                var t = ncss;
                var u = ncss1;
                ncss = Unsafe.Add(ref nodeParentRef, t) = ncss1 = Unsafe.Add(ref nodeParentRef, u) = 0;
                return (t, u);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            (int, int, int) Split(int s, int t, ref int nodeChildRef, ref int nodeParentRef)
            {
                var u = Split2(s, ref nodeChildRef, ref nodeParentRef);
                if (IsSameNode(u.Item1, t, ref nodeChildRef, ref nodeParentRef))
                {
                    var r = Split2(t, ref nodeChildRef, ref nodeParentRef);
                    return (r.Item1, r.Item2, u.Item2);
                }
                else
                {
                    var r = Split2(t, ref nodeChildRef, ref nodeParentRef);
                    return (u.Item1, r.Item1, r.Item2);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int Merge(int s, int t, ref int nodeChildRef, ref int nodeParentRef)
            {
                if (s == 0) return t;
                if (t == 0) return s;
                while (Unsafe.Add(ref nodeChildRef, (s << 1) | 1) != 0) s = Unsafe.Add(ref nodeChildRef, (s << 1) | 1);
                Splay(s, ref nodeChildRef, ref nodeParentRef);
                Unsafe.Add(ref nodeChildRef, (s << 1) | 1) = t;
                if (t != 0) Unsafe.Add(ref nodeParentRef, t) = s;
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
                ref byte fl = ref nodeFlags[t];
                var flag = nodeFlags[leftChild] | (fl >> 1) | nodeFlags[rightChild];
                if ((flag & 1) != 0) fl |= 1;
                else fl &= 14;
                if ((flag & 4) != 0) fl |= 4;
                else fl &= 11;
                return t;
            }
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //void Push(int t)
            //{
            //    // todo
            //}
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Rot(int t, int b, ref int nodeChildRef, ref int nodeParentRef)
            {
                if (t == 0) return;
                ref int npt = ref Unsafe.Add(ref nodeParentRef, t);
                var x = npt;
                ref int npx = ref Unsafe.Add(ref nodeParentRef, x);
                var y = npx;
                ref int nctsb = ref Unsafe.Add(ref nodeChildRef, (t << 1) | b);
                if ((Unsafe.Add(ref nodeChildRef, (x << 1) | (1 - b)) = nctsb) != 0) Unsafe.Add(ref nodeParentRef, nctsb) = x;
                nctsb = x;
                npx = t;
                UpdateNode(x);
                UpdateNode(t);
                if ((npt = y) != 0)
                {
                    ref int nodeChildys = ref Unsafe.Add(ref nodeChildRef, y << 1);
                    ref int nodeChildys1 = ref Unsafe.Add(ref nodeChildRef, (y << 1) | 1);
                    if (nodeChildys == x) nodeChildys = t;
                    if (nodeChildys1 == x) nodeChildys1 = t;
                    UpdateNode(y);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Splay(int t, ref int nodeChildRef, ref int nodeParentRef)
            {
                if (t == 0) return;
                //Push(t);
                ref int npt = ref Unsafe.Add(ref nodeParentRef, t);
                while (npt != 0)
                {
                    var q = npt;
                    ref int npq = ref Unsafe.Add(ref nodeParentRef, q);
                    if (npq == 0)
                    {
                        //Push(q);
                        //Push(t);
                        Rot(t, Unsafe.Add(ref nodeChildRef, q << 1) == t ? 1 : 0, ref nodeChildRef, ref nodeParentRef);
                    }
                    else
                    {
                        var r = npq;
                        //Push(r);
                        //Push(q);
                        //Push(t);
                        var b = Unsafe.Add(ref nodeChildRef, r << 1) == q ? 1 : 0;
                        if (Unsafe.Add(ref nodeChildRef, (q << 1) | (1 - b)) == t)
                        {
                            Rot(q, b, ref nodeChildRef, ref nodeParentRef);
                            Rot(t, b, ref nodeChildRef, ref nodeParentRef);
                        }
                        else
                        {
                            Rot(t, 1 - b, ref nodeChildRef, ref nodeParentRef);
                            Rot(t, b, ref nodeChildRef, ref nodeParentRef);
                        }
                    }
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Size(int s)
            {
                var t = startNodeIndex + s;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
                return nodeSz[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSame(int s, int t) => IsSameNode(startNodeIndex + s, startNodeIndex + t, ref nodeChild[0], ref nodeParent[0]);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Get(int s)
            {
                var t = startNodeIndex + s;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
                return nodeVal[t];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(int s, T x)
            {
                var t = startNodeIndex + s;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
                nodeVal[t] = x;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeUpdate(int s, Action<int, int> g)
            {
                var t = startNodeIndex + s;
                ref int nodeChildRef = ref nodeChild[0];
                ref int nodeParentRef = ref nodeParent[0];
                ref int nodeLRef = ref nodeL[0];
                ref int nodeRRef = ref nodeR[0];
                ref byte nodeFlagsRef = ref nodeFlags[0];
                Splay(t, ref nodeChildRef, ref nodeParentRef);
                while ((Unsafe.Add(ref nodeFlagsRef, t) & 4) != 0)
                {
                    var node = t;
                    while (true)
                    {
                        ref byte nodeFlagsNode = ref Unsafe.Add(ref nodeFlagsRef, node);
                        var nodeLNode = Unsafe.Add(ref nodeLRef, node);
                        var nodeRNode = Unsafe.Add(ref nodeRRef, node);
                        if (nodeLNode < nodeRNode && (nodeFlagsNode & 8) != 0)
                        {
                            Splay(node, ref nodeChildRef, ref nodeParentRef);
                            nodeFlagsNode &= 7;
                            g(nodeLNode, nodeRNode);
                            break;
                        }
                        var ns = node << 1;
                        var ncns = Unsafe.Add(ref nodeChildRef, ns);
                        if ((Unsafe.Add(ref nodeFlagsRef, ncns) & 4) != 0) node = ncns;
                        else node = Unsafe.Add(ref nodeChildRef, ns | 1);
                    }
                    Splay(t, ref nodeChildRef, ref nodeParentRef);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryReconnect(int s, Func<int, int, bool> f, int idx)
            {
                var t = startNodeIndex + s;
                ref int nodeChildRef = ref nodeChild[0];
                ref int nodeParentRef = ref nodeParent[0];
                ref int nodeLRef = ref nodeL[0];
                ref byte nodeFlagsRef = ref nodeFlags[0];
                Splay(t, ref nodeChildRef, ref nodeParentRef);
                while ((Unsafe.Add(ref nodeFlagsRef, t) & 1) != 0)
                {
                    var node = t;
                    while (true)
                    {
                        if ((Unsafe.Add(ref nodeFlagsRef, node) & 2) != 0)
                        {
                            Splay(node, ref nodeChildRef, ref nodeParentRef);
                            if (f(Unsafe.Add(ref nodeLRef, node), idx)) return true;
                            break;
                        }
                        var ns = node << 1;
                        var ncns = Unsafe.Add(ref nodeChildRef, ns);
                        if ((Unsafe.Add(ref nodeFlagsRef, ncns) & 1) != 0) node = ncns;
                        else node = Unsafe.Add(ref nodeChildRef, ns | 1);
                    }
                    Splay(t, ref nodeChildRef, ref nodeParentRef);
                }
                return false;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void EdgeConnectedUpdate(int s, bool b)
            {
                var t = startNodeIndex + s;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
                if (b) nodeFlags[t] |= 2;
                else nodeFlags[t] &= 13;
                UpdateNode(t);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Link(int l, int r)
            {
                ref int nodeChildRef = ref nodeChild[0];
                ref int nodeParentRef = ref nodeParent[0];
                if (l > r) { var t = l; l = r; r = t; }
                if (IsSameNode(startNodeIndex + l, startNodeIndex + r, ref nodeChildRef, ref nodeParentRef)) return false;
                var lv = ReRoot(startNodeIndex + l, ref nodeChildRef, ref nodeParentRef);
                var rv = ReRoot(startNodeIndex + r, ref nodeChildRef, ref nodeParentRef);
                var lrnode = ptr.GetOrInsert(((ulong)l << 32) | (uint)r, () =>
                {
                    NewNode(l, r);
                    return NewNode(r, l) - 1;
                });
                Unsafe.Add(ref nodeParentRef, lv) = Unsafe.Add(ref nodeParentRef, rv) = lrnode;
                var lrnodes = lrnode << 1;
                Unsafe.Add(ref nodeChildRef, lrnodes) = lv;
                Unsafe.Add(ref nodeChildRef, lrnodes | 1) = rv;
                UpdateNode(lrnode);
                Merge(lrnode, lrnode + 1, ref nodeChildRef, ref nodeParentRef);
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Cut(int l, int r)
            {
                if (l > r) { var t = l; l = r; r = t; }
                if (!ptr.ContainsKey(((ulong)l << 32) | (uint)r)) return false;
                ref int nodeChildRef = ref nodeChild[0];
                ref int nodeParentRef = ref nodeParent[0];
                var p = ptr.GetAndErase(((ulong)l << 32) | (uint)r);
                var s = Split(p, p + 1, ref nodeChildRef, ref nodeParentRef);
                Merge(s.Item1, s.Item3, ref nodeChildRef, ref nodeParentRef);
                return true;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Query(int p, int v)
            {
                Cut(p, v);
                var t = startNodeIndex + v;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
                var res = nodeSum[t];
                Link(p, v);
                return res;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Query(int s)
            {
                var t = startNodeIndex + s;
                Splay(t, ref nodeChild[0], ref nodeParent[0]);
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
                ref int stref = ref st[0];
                for (var delta = 0; ; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (Unsafe.Add(ref stref, i) != 2)
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
                ref int stref = ref st[0];
                ref (ulong, int) bckref = ref bck[0];
                for (var delta = 0; delta <= prode; ++delta)
                {
                    var i = (h + delta) & mask;
                    var sti = Unsafe.Add(ref stref, i);
                    if (sti == 2)
                    {
                        if (Unsafe.Add(ref bckref, i).Item1 == key) return i;
                    }
                    else if (sti == 0) return -1;
                }
                return -1;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int FindOrAllocate(ulong key)
            {
                var h = Hash(key);
                var hole = -1;
                var delta = 0;
                ref int stref = ref st[0];
                ref (ulong, int) bckref = ref bck[0];
                for (; delta <= prode; ++delta)
                {
                    var i = (h + delta) & mask;
                    var sti = Unsafe.Add(ref stref, i);
                    if (sti == 2)
                    {
                        if (Unsafe.Add(ref bckref, i).Item1 == key) return i;
                    }
                    else if (sti == 0) return i;
                    else
                    {
                        if (hole == -1) hole = i;
                    }
                }
                if (hole != -1) return hole;
                for (; ; ++delta)
                {
                    var i = (h + delta) & mask;
                    if (Unsafe.Add(ref stref, i) != 2)
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
                var nowlen = bck.Length;
                if (requiredCnt > nowlen)
                {
                    nextCnt = 4;
                    while (nextCnt < requiredCnt) nextCnt <<= 1;
                }
                else if (nextCnt <= nowlen >> 2) nextCnt = Max(4, nowlen >> 1);
                else return;
                var oldSt = new int[nextCnt];
                var oldBck = new (ulong, int)[nextCnt];
                { var t = oldSt; oldSt = st; st = t; }
                { var t = oldBck; oldBck = bck; bck = t; }
                mask = nextCnt - 1;
                Count = 0;
                prode = 0;
                minElem = nextCnt - 1;
                if (nowlen == 0) return;
                ref int stref = ref st[0];
                ref (ulong, int) bckref = ref bck[0];
                ref int oldstref = ref oldSt[0];
                ref (ulong, int) oldbckref = ref oldBck[0];
                for (var pos = 0; pos < nowlen; ++pos)
                {
                    if (Unsafe.Add(ref oldstref, pos) == 2)
                    {
                        ref (ulong, int) oldbckpos = ref Unsafe.Add(ref oldbckref, pos);
                        var i = FindEmpty(oldbckpos.Item1);
                        Unsafe.Add(ref stref, i) = 2;
                        Unsafe.Add(ref bckref, i) = oldbckpos;
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
                ref int stref = ref st[0];
                ref (ulong, int) bckref = ref bck[0];
                var stlen = st.Length;
                for (var i = minElem; i < stlen; ++i)
                {
                    ref int sti = ref Unsafe.Add(ref stref, i);
                    if (sti == 2)
                    {
                        minElem = i;
                        var res = func(Unsafe.Add(ref bckref, i).Item1, idx);
                        if ((res & 2) != 0)
                        {
                            sti = 1;
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