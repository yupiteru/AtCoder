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
    class LIB_InsertableList
    {
        static public LIB_InsertableList<long, long> CreateRangeUpdateRangeMin() => new LIB_InsertableList<long, long>(long.MaxValue, long.MinValue + 100, Math.Min, (x, y, c) => y, (x, y) => y);
        static public LIB_InsertableList<long, long> CreateRangeAddRangeMin() => new LIB_InsertableList<long, long>(long.MaxValue, 0, Math.Min, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_InsertableList<long, long> CreateRangeUpdateRangeMax() => new LIB_InsertableList<long, long>(long.MinValue, long.MaxValue - 100, Math.Max, (x, y, c) => y, (x, y) => y);
        static public LIB_InsertableList<long, long> CreateRangeAddRangeMax() => new LIB_InsertableList<long, long>(long.MinValue, 0, Math.Max, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_InsertableList<long, long> CreateRangeUpdateRangeSum() => new LIB_InsertableList<long, long>(0, long.MaxValue, (x, y) => x + y, (x, y, c) => y * c, (x, y) => y);
        static public LIB_InsertableList<long, long> CreateRangeAddRangeSum() => new LIB_InsertableList<long, long>(0, 0, (x, y) => x + y, (x, y, c) => x + y * c, (x, y) => x + y);
    }
    class LIB_InsertableList<ValueT, ValueE> where ValueE : IEquatable<ValueE>
    {
        bool ope;
        class Node
        {
            public Node left;
            public Node right;
            public ValueT val;
            public ValueT dat;
            public ValueE lazy;
            public bool isBlack;
            public int cnt;
            public bool needRecalc;
        }
        Func<ValueT, ValueT, ValueT> f;
        Func<ValueT, ValueE, int, ValueT> g;
        Func<ValueE, ValueE, ValueE> h;
        ValueT ti;
        ValueE ei;
        Node root;
        bool isNeedFix;
        Node lmax;
        List<Node> pool = new List<Node>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_InsertableList(ValueT ti, ValueE ei, Func<ValueT, ValueT, ValueT> f, Func<ValueT, ValueE, int, ValueT> g, Func<ValueE, ValueE, ValueE> h, bool ope = true)
        {
            this.ti = ti;
            this.ei = ei;
            this.f = f;
            this.g = g;
            this.h = h;
            this.ope = ope;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsRed(Node n) => n != null && !n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsBlack(Node n) => n != null && n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Cnt(Node n) => n == null ? 0 : n.cnt;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Eval(Node n)
        {
            if (n == null || ei.Equals(n.lazy)) return;
            n.val = g(n.val, n.lazy, 1);
            if (!n.needRecalc) n.dat = g(n.dat, n.lazy, Cnt(n));
            if (n.left != null) n.left.lazy = h(n.left.lazy, n.lazy);
            if (n.right != null) n.right.lazy = h(n.right.lazy, n.lazy);
            n.lazy = ei;
        }
        void Recalc(Node n)
        {
            Eval(n);
            if (!n.needRecalc) return;
            n.needRecalc = false;
            n.dat = n.val;
            if (n.left != null)
            {
                Recalc(n.left);
                n.dat = f(n.left.dat, n.dat);
            }
            if (n.right != null)
            {
                Recalc(n.right);
                n.dat = f(n.dat, n.right.dat);
            }
        }
        Node RotateL(Node n)
        {
            if (ope && n != null) { Eval(n); Eval(n.right); }
            Node m = n.right, t = m.left;
            m.left = n; n.right = t;
            n.cnt -= m.cnt - Cnt(t);
            m.cnt += n.cnt - Cnt(t);
            n.needRecalc = true; m.needRecalc = true;
            return m;
        }
        Node RotateR(Node n)
        {
            if (ope && n != null) { Eval(n); Eval(n.left); }
            Node m = n.left, t = m.right;
            m.right = n; n.left = t;
            n.cnt -= m.cnt - Cnt(t);
            m.cnt += n.cnt - Cnt(t);
            n.needRecalc = true; m.needRecalc = true;
            return m;
        }
        Node RotateLR(Node n)
        {
            n.left = RotateL(n.left);
            return RotateR(n);
        }
        Node RotateRL(Node n)
        {
            n.right = RotateR(n.right);
            return RotateL(n);
        }
        ValueT val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ValueT val)
        {
            this.val = val;
            root = Insert(root, Cnt(root));
            root.isBlack = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(long index, ValueT val)
        {
            this.val = val;
            root = Insert(root, (int)index);
            root.isBlack = true;
        }
        Node Insert(Node n, int index)
        {
            if (n == null)
            {
                isNeedFix = true;
                Node ret;
                if (pool.Count > 0)
                {
                    ret = pool[pool.Count - 1];
                    pool.RemoveAt(pool.Count - 1);
                    ret.left = ret.right = null;
                    ret.val = val;
                    ret.dat = val;
                    ret.lazy = ei;
                    ret.isBlack = false;
                    ret.cnt = 1;
                    ret.needRecalc = false;
                }
                else
                {
                    ret = new Node() { val = val, dat = val, lazy = ei, cnt = 1 };
                }
                return ret;
            }
            if (ope) Eval(n);
            var lc = Cnt(n.left);
            if (index <= lc) n.left = Insert(n.left, index);
            else n.right = Insert(n.right, index - lc - 1);
            n.needRecalc = true;
            ++n.cnt;
            return Balance(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Balance(Node n)
        {
            if (!isNeedFix || !IsBlack(n)) return n;
            if (IsRed(n.left) && IsRed(n.left.left))
            {
                n = RotateR(n);
                n.left.isBlack = true;
            }
            else if (IsRed(n.left) && IsRed(n.left.right))
            {
                n = RotateLR(n);
                n.left.isBlack = true;
            }
            else if (IsRed(n.right) && IsRed(n.right.left))
            {
                n = RotateRL(n);
                n.right.isBlack = true;
            }
            else if (IsRed(n.right) && IsRed(n.right.right))
            {
                n = RotateL(n);
                n.right.isBlack = true;
            }
            else isNeedFix = false;
            return n;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(long index)
        {
            root = RemoveAt(root, index);
            if (root != null) root.isBlack = true;
        }
        Node RemoveAt(Node n, long index)
        {
            if (ope) Eval(n);
            --n.cnt;
            var r = index.CompareTo(Cnt(n?.left));
            if (r < 0)
            {
                n.left = RemoveAt(n.left, index);
                n.needRecalc = true;
                return BalanceL(n);
            }
            if (r > 0)
            {
                n.right = RemoveAt(n.right, index - Cnt(n?.left) - 1);
                n.needRecalc = true;
                return BalanceR(n);
            }
            if (n.left == null)
            {
                isNeedFix = n.isBlack;
                return n.right;
            }
            n.left = RemoveMax(n.left);
            n.val = lmax.val;
            n.needRecalc = true;
            return BalanceL(n);
        }
        Node RemoveMax(Node n)
        {
            if (ope) Eval(n);
            --n.cnt;
            if (n.right != null)
            {
                n.right = RemoveMax(n.right);
                n.needRecalc = true;
                return BalanceR(n);
            }
            lmax = n;
            pool.Add(lmax);
            isNeedFix = n.isBlack;
            return n.left;
        }
        Node BalanceL(Node n)
        {
            if (!isNeedFix) return n;
            if (IsBlack(n.right) && IsRed(n.right.left))
            {
                var b = n.isBlack;
                n = RotateRL(n);
                n.isBlack = b;
                n.left.isBlack = true;
                isNeedFix = false;
            }
            else if (IsBlack(n.right) && IsRed(n.right.right))
            {
                var b = n.isBlack;
                n = RotateL(n);
                n.isBlack = b;
                n.right.isBlack = true;
                n.left.isBlack = true;
                isNeedFix = false;
            }
            else if (IsBlack(n.right))
            {
                isNeedFix = n.isBlack;
                n.isBlack = true;
                n.right.isBlack = false;
            }
            else
            {
                n = RotateL(n);
                n.isBlack = true;
                n.left.isBlack = false;
                n.left = BalanceL(n.left);
                isNeedFix = false;
            }
            return n;
        }
        Node BalanceR(Node n)
        {
            if (!isNeedFix) return n;
            if (IsBlack(n.left) && IsRed(n.left.right))
            {
                var b = n.isBlack;
                n = RotateLR(n);
                n.isBlack = b; n.right.isBlack = true;
                isNeedFix = false;
            }
            else if (IsBlack(n.left) && IsRed(n.left.left))
            {
                var b = n.isBlack;
                n = RotateR(n);
                n.isBlack = b;
                n.left.isBlack = true;
                n.right.isBlack = true;
                isNeedFix = false;
            }
            else if (IsBlack(n.left))
            {
                isNeedFix = n.isBlack;
                n.isBlack = true;
                n.left.isBlack = false;
            }
            else
            {
                n = RotateR(n);
                n.isBlack = true;
                n.right.isBlack = false;
                n.right = BalanceR(n.right);
                isNeedFix = false;
            }
            return n;
        }
        public ValueT this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return At(root, i); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { ChangeValue(i, value); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ChangeValue(long i, ValueT v)
        {
            var n = root;
            while (true)
            {
                if (ope) Eval(n);
                if (n.left == null)
                {
                    if (i == 0) break;
                    else
                    {
                        n = n.right;
                        --i;
                    }
                }
                else if (n.left.cnt == i) break;
                else if (n.left.cnt > i) n = n.left;
                else
                {
                    i = i - n.left.cnt - 1;
                    n = n.right;
                }
            }
            n.needRecalc = true;
            n.val = v;
        }
        ValueT At(Node n, long i)
        {
            while (true)
            {
                if (ope) Eval(n);
                if (n.left == null)
                {
                    if (i == 0) return n.val;
                    else
                    {
                        n = n.right;
                        --i;
                    }
                }
                else if (n.left.cnt == i) return n.val;
                else if (n.left.cnt > i) n = n.left;
                else
                {
                    i = i - n.left.cnt - 1;
                    n = n.right;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, ValueE val) => Update(root, l, r, val);
        void Update(Node n, long l, long r, ValueE val)
        {
            if (n == null) return;
            Eval(n);
            n.needRecalc = true;
            var lc = Cnt(n.left);
            if (lc < l) Update(n.right, l - lc - 1, r - lc - 1, val);
            else if (r <= lc) Update(n.left, l, r, val);
            else if (l <= 0 && Cnt(n) <= r) n.lazy = val;
            else
            {
                n.val = g(n.val, val, 1);
                if (l < lc) Update(n.left, l, lc, val);
                if (lc + 1 < r) Update(n.right, 0, r - lc - 1, val);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueT Query(long l, long r) => root == null ? ti : Query(root, l, r);
        ValueT Query(Node n, long l, long r)
        {
            var v1 = ti; var v2 = ti; var v3 = ti;
            Eval(n);
            var lc = Cnt(n.left);
            if (lc < l) v3 = n.right == null ? ti : Query(n.right, l - lc - 1, r - lc - 1);
            else if (r <= lc) v1 = n.left == null ? ti : Query(n.left, l, r);
            else if (l <= 0 && Cnt(n) <= r)
            {
                Recalc(n);
                v2 = n.dat;
            }
            else
            {
                if (l < lc) v1 = n.left == null ? ti : Query(n.left, l, lc);
                if (lc + 1 < r) v3 = n.right == null ? ti : Query(n.right, 0, r - lc - 1);
                v2 = n.val;
            }
            return f(f(v1, v2), v3);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => root != null;
        public long Count => Cnt(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueT[] List()
        {
            var ret = new List<ValueT>();
            var stack = new LIB_Deque<Node>();
            var node = root;
            while (true)
            {
                if (node != null)
                {
                    stack.PushBack(node);
                    node = node.left;
                }
                else
                {
                    if (stack.Count == 0) break;
                    ret.Add(stack.Back.val);
                    node = stack.PopBack().right;
                }
            }
            return ret.ToArray();
        }
    }
    class LIB_InsertableList<T>
    {
        LIB_InsertableList<T, int> tree;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_InsertableList() { tree = new LIB_InsertableList<T, int>(default(T), 0, null, null, null, false); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T val) => tree.Add(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertAt(long index, T val) => tree.Insert(index, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(long index) => tree.RemoveAt(index);
        public T this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tree[i]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { tree[i] = value; }
        }
        public long Count => tree.Count;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] List() => tree.List();
    }
    ////end
}