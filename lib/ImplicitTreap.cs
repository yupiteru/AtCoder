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

    class LIB_ImplicitTreep<T, E> where T : IEquatable<T> where E : IEquatable<E>
    {
        class Node
        {
            static uint xorshift { get { _xsi.MoveNext(); return _xsi.Current; } }
            static IEnumerator<uint> _xsi = _xsc();
            static IEnumerator<uint> _xsc() { uint x = 123456789, y = 362436069, z = 521288629, w = (uint)(DateTime.Now.Ticks & 0xffffffff); while (true) { var t = x ^ (x << 11); x = y; y = z; z = w; w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)); yield return w; } }

            public T value;
            public T acc;
            public E lazy;
            public uint priority;
            public int cnt;
            public bool rev;
            public Node l;
            public Node r;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node(T val, E ei, T ti)
            {
                priority = xorshift;
                value = val;
                acc = ti;
                lazy = ei;
                cnt = 1;
                rev = false;
                l = null;
                r = null;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node CreateNode(T val) => new Node(val, ei, ti);
        T ti;
        E ei;
        Func<T, T, T> f;
        Func<T, E, T> g;
        Func<E, E, E> h;
        Node root = null;
        bool ope;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ImplicitTreep(T _ti, E _ei, Func<T, T, T> _f, Func<T, E, T> _g, Func<E, E, E> _h, bool _ope = true)
        {
            ope = _ope;
            ti = _ti;
            ei = _ei;
            f = _f;
            g = _g;
            h = _h;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Cnt(Node n) => n?.cnt ?? 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Acc(Node n) => n == null ? ti : n.acc;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UpdateAcc(Node n)
        {
            if (n != null) n.acc = f(Acc(n.l), f(n.value, Acc(n.r)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Pushup(Node n)
        {
            if (n != null) n.cnt = 1 + Cnt(n.l) + Cnt(n.r);
            if (ope) UpdateAcc(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Pushdown(Node n)
        {
            if (n?.rev ?? false)
            {
                n.rev = false;
                var t = n.l; n.l = n.r; n.r = t;
                if (n.l != null) n.l.rev ^= true;
                if (n.r != null) n.r.rev ^= true;
            }
            if (ope && n != null && !n.lazy.Equals(ei))
            {
                if (n.l != null) n.l.lazy = h(n.l.lazy, n.lazy);
                if (n.r != null) n.r.lazy = h(n.r.lazy, n.lazy);
                n.acc = g(n.acc, n.lazy);
                n.value = g(n.value, n.lazy);
                n.lazy = ei;
            }
            Pushup(n);
        }
        void Split(Node n, long key, out Node l, out Node r)
        {
            if (n == null)
            {
                l = r = null;
                return;
            }
            Pushdown(n);
            var implicitKey = Cnt(n.l) + 1;
            if (key < implicitKey)
            {
                Split(n.l, key, out l, out n.l);
                r = n;
            }
            else
            {
                Split(n.r, key - implicitKey, out n.r, out r);
                l = n;
            }
            Pushup(n);
        }
        void Insert(ref Node n, long key, Node x)
        {
            if (n == null)
            {
                n = x;
                return;
            }
            Pushdown(n);
            if (Cnt(n.l) == key || x.priority > n.priority)
            {
                Split(n, key, out x.l, out x.r);
                n = x;
            }
            else
            {
                if (Cnt(n.l) > key)
                {
                    Insert(ref n.l, key, x);
                }
                else
                {
                    Insert(ref n.r, key - Cnt(n.l) - 1, x);
                }
            }
            Pushup(n);
        }
        void Merge(out Node n, Node l, Node r)
        {
            Pushdown(l);
            Pushdown(r);
            if (l == null || r == null)
            {
                n = l != null ? l : r;
            }
            else if (l.priority > r.priority)
            {
                Merge(out l.r, l.r, r);
                n = l;
            }
            else
            {
                Merge(out r.l, l, r.l);
                n = r;
            }
            Pushup(n);
        }
        void Remove(ref Node n, long key)
        {
            Pushdown(n);
            if (Cnt(n.l) == key)
            {
                Merge(out n, n.l, n.r);
            }
            else
            {
                if (Cnt(n.l) > key)
                {
                    Remove(ref n.l, key);
                }
                else
                {
                    Remove(ref n.r, key - Cnt(n.l) - 1);
                }
            }
            Pushup(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T At(Node n, long i)
        {
            while (true)
            {
                Pushdown(n);
                if (n.l == null)
                {
                    if (i == 0) return n.value;
                    else
                    {
                        n = n.r;
                        --i;
                    }
                }
                else if (n.l.cnt == i) return n.value;
                else if (n.l.cnt > i) n = n.l;
                else
                {
                    i = i - n.l.cnt - 1;
                    n = n.r;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Update(Node n, long l, long r, E val)
        {
            if (n == null) return;
            Pushdown(n);
            var lc = Cnt(n.l);
            if (lc < l) Update(n.r, l - lc - 1, r - lc - 1, val);
            else if (r <= lc) Update(n.l, l, r, val);
            else if (l <= 0 && Cnt(n) <= r) n.lazy = val;
            else
            {
                n.value = g(n.value, val);
                if (l < lc) Update(n.l, l, lc, val);
                if (lc + 1 < r) Update(n.r, 0, r - lc - 1, val);
            }
            Pushup(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Query(Node n, long l, long r)
        {
            var v1 = ti; var v2 = ti; var v3 = ti;
            Pushdown(n);
            var lc = Cnt(n.l);
            if (lc < l) v3 = n.r == null ? ti : Query(n.r, l - lc - 1, r - lc - 1);
            else if (r <= lc) v1 = n.l == null ? ti : Query(n.l, l, r);
            else if (l <= 0 && Cnt(n) <= r)
            {
                v2 = n.acc;
            }
            else
            {
                if (l < lc) v1 = n.l == null ? ti : Query(n.l, l, lc);
                if (lc + 1 < r) v3 = n.r == null ? ti : Query(n.r, 0, r - lc - 1);
                v2 = n.value;
            }
            return f(f(v1, v2), v3);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long Find(Node n, T x, long offset, bool left = true)
        {
            if (f(Acc(n), x).Equals(x))
            {
                return -1;
            }
            else
            {
                if (left)
                {
                    if (n.l != null && !f(n.l.acc, x).Equals(x))
                    {
                        return Find(n.l, x, offset, left);
                    }
                    else
                    {
                        return (!f(n.value, x).Equals(x)) ? offset + Cnt(n.l) : Find(n.r, x, offset + Cnt(n.l) + 1, left);
                    }
                }
                else
                {
                    if (n.r != null && !f(n.r.acc, x).Equals(x))
                    {
                        return Find(n.r, x, offset + Cnt(n.l) + 1, left);
                    }
                    else
                    {
                        return (!f(n.value, x).Equals(x)) ? offset + Cnt(n.l) : Find(n.l, x, offset, left);
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Reverse(Node n, long l, long r)
        {
            if (l >= r) return;
            Node n1, n2, n3;
            Split(n, l, out n1, out n2);
            Split(n2, r - l, out n2, out n3);
            n2.rev ^= true;
            Merge(out n2, n2, n3);
            Merge(out n, n1, n2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Rotate(Node n, long l, long m, long r)
        {
            Reverse(n, l, r);
            Reverse(n, l, l + r - m);
            Reverse(n, l + r - m, r);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(long idx, T x) => Insert(ref root, idx, CreateNode(x));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T x) => Insert(Count, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, E x) => Update(root, l, r, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r) => Query(root, l, r);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Find(long l, long r, T x, bool left = true)
        {
            Node n1, n2, n3;
            Split(root, l, out n1, out n2);
            Split(n2, r - l, out n2, out n3);
            var ret = Find(n2, x, l, left);
            Merge(out n2, n2, n3);
            Merge(out root, n1, n2);
            return ret < 0 ? r : ret;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(long idx) => Remove(ref root, idx);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(long l, long r) => Reverse(root, l, r);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(long l, long m, long r) => Rotate(root, l, m, r);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> List() => L(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<T> L(Node n)
        {
            if (n == null) yield break;
            Pushdown(n);
            foreach (var i in L(n.l)) yield return i;
            yield return n.value;
            foreach (var i in L(n.r)) yield return i;
        }
        public int Count => Cnt(root);
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return At(root, idx);
            }
            private set { }
        }
    }
    class LIB_ImplicitTreep<T> where T : IEquatable<T>
    {
        LIB_ImplicitTreep<T, int> tree;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ImplicitTreep() { tree = new LIB_ImplicitTreep<T, int>(default(T), 0, null, null, null, false); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(long idx, T x) => tree.Insert(idx, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T x) => tree.Add(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(long idx) => tree.RemoveAt(idx);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(long l, long r) => tree.Reverse(l, r);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(long l, long m, long r) => tree.Rotate(l, m, r);
        public int Count => tree.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> List() => tree.List();
        public T this[long idx]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return tree[idx];
            }
            private set { }
        }
    }
    ////end
}