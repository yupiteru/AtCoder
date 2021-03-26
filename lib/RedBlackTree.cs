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
    class LIB_RedBlackTree
    {
        static public LIB_RedBlackTree<long, long, long> CreateRangeUpdateRangeMin() => new LIB_RedBlackTree<long, long, long>(long.MaxValue, long.MinValue + 100, Math.Min, (x, y, c) => y, (x, y) => y);
        static public LIB_RedBlackTree<long, long, long> CreateRangeAddRangeMin() => new LIB_RedBlackTree<long, long, long>(long.MaxValue, 0, Math.Min, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_RedBlackTree<long, long, long> CreateRangeUpdateRangeMax() => new LIB_RedBlackTree<long, long, long>(long.MinValue, long.MaxValue - 100, Math.Max, (x, y, c) => y, (x, y) => y);
        static public LIB_RedBlackTree<long, long, long> CreateRangeAddRangeMax() => new LIB_RedBlackTree<long, long, long>(long.MinValue, 0, Math.Max, (x, y, c) => x + y, (x, y) => x + y);
        static public LIB_RedBlackTree<long, long, long> CreateRangeUpdateRangeSum() => new LIB_RedBlackTree<long, long, long>(0, long.MaxValue, (x, y) => x + y, (x, y, c) => y * c, (x, y) => y);
        static public LIB_RedBlackTree<long, long, long> CreateRangeAddRangeSum() => new LIB_RedBlackTree<long, long, long>(0, 0, (x, y) => x + y, (x, y, c) => x + y * c, (x, y) => x + y);
    }
    class LIB_RedBlackTree<Key, ValueT, ValueE> where ValueE : IEquatable<ValueE>
    {
        bool ope;
        class Node
        {
            public Node left;
            public Node right;
            public Key key;
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
        Comparison<Key> c;
        Node root;
        bool isNeedFix;
        Node lmax;
        public LIB_RedBlackTree(ValueT ti, ValueE ei, Func<ValueT, ValueT, ValueT> f, Func<ValueT, ValueE, int, ValueT> g, Func<ValueE, ValueE, ValueE> h, Comparison<Key> c, bool ope = true)
        {
            this.ti = ti;
            this.ei = ei;
            this.f = f;
            this.g = g;
            this.h = h;
            this.c = c;
            this.ope = ope;
        }
        public LIB_RedBlackTree(ValueT ti, ValueE ei, Func<ValueT, ValueT, ValueT> f, Func<ValueT, ValueE, int, ValueT> g, Func<ValueE, ValueE, ValueE> h) : this(ti, ei, f, g, h, Comparer<Key>.Default.Compare) { }
        bool IsRed(Node n) => n != null && !n.isBlack;
        bool IsBlack(Node n) => n != null && n.isBlack;
        int Cnt(Node n) => n == null ? 0 : n.cnt;
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
        public void Add(Key key, ValueT val)
        {
            root = Add(root, key, val);
            root.isBlack = true;
        }
        Node Add(Node n, Key key, ValueT val)
        {
            if (n == null)
            {
                isNeedFix = true;
                return new Node() { key = key, val = val, dat = val, lazy = ei, cnt = 1 };
            }
            if (ope) Eval(n);
            if (c(key, n.key) < 0) n.left = Add(n.left, key, val);
            else n.right = Add(n.right, key, val);
            n.needRecalc = true;
            ++n.cnt;
            return Balance(n);
        }
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
            n.key = lmax.key;
            n.val = lmax.val;
            n.needRecalc = true;
            return BalanceL(n);
        }
        public void Remove(Key key)
        {
            root = Remove(root, key);
            if (root != null) root.isBlack = true;
        }
        Node Remove(Node n, Key key)
        {
            if (ope) Eval(n);
            --n.cnt;
            var r = c(key, n.key);
            if (r < 0)
            {
                n.left = Remove(n.left, key);
                n.needRecalc = true;
                return BalanceL(n);
            }
            if (r > 0)
            {
                n.right = Remove(n.right, key);
                n.needRecalc = true;
                return BalanceR(n);
            }
            if (n.left == null)
            {
                isNeedFix = n.isBlack;
                return n.right;
            }
            n.left = RemoveMax(n.left);
            n.key = lmax.key;
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
        public KeyValuePair<Key, ValueT> this[long i]
        {
            get { return At(root, i); }
        }
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
        KeyValuePair<Key, ValueT> At(Node n, long i)
        {
            while (true)
            {
                if (ope) Eval(n);
                if (n.left == null)
                {
                    if (i == 0) return new KeyValuePair<Key, ValueT>(n.key, n.val);
                    else
                    {
                        n = n.right;
                        --i;
                    }
                }
                else if (n.left.cnt == i) return new KeyValuePair<Key, ValueT>(n.key, n.val);
                else if (n.left.cnt > i) n = n.left;
                else
                {
                    i = i - n.left.cnt - 1;
                    n = n.right;
                }
            }
        }
        public bool ContainsKey(Key key)
        {
            var t = LowerBound(key);
            return t < Cnt(root) && c(At(root, t).Key, key) == 0;
        }
        public long UpperBound(Key key)
        {
            var n = root;
            var ret = 0L;
            while (true)
            {
                if (n == null) return ret;
                if (c(key, n.key) < 0) n = n.left;
                else
                {
                    ret += Cnt(n.left) + 1;
                    n = n.right;
                }
            }
        }
        public long LowerBound(Key key)
        {
            var n = root;
            var ret = 0L;
            while (true)
            {
                if (n == null) return ret;
                if (c(key, n.key) <= 0) n = n.left;
                else
                {
                    ret += Cnt(n.left) + 1;
                    n = n.right;
                }
            }
        }
        public KeyValuePair<Key, ValueT> Min()
        {
            Node n = root.left, p = root;
            while (n != null)
            {
                p = n;
                if (ope) Eval(p);
                n = n.left;
            }
            return new KeyValuePair<Key, ValueT>(p.key, p.val);
        }
        public KeyValuePair<Key, ValueT> Max()
        {
            Node n = root.right, p = root;
            while (n != null)
            {
                p = n;
                if (ope) Eval(p);
                n = n.right;
            }
            return new KeyValuePair<Key, ValueT>(p.key, p.val);
        }
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
        public bool Any() => root != null;
        public long Count => Cnt(root);
        public IEnumerable<KeyValuePair<Key, ValueT>> List() => L(root);
        IEnumerable<KeyValuePair<Key, ValueT>> L(Node n)
        {
            if (n == null) yield break;
            foreach (var i in L(n.left)) yield return i;
            yield return new KeyValuePair<Key, ValueT>(n.key, n.val);
            foreach (var i in L(n.right)) yield return i;
        }
    }
    class LIB_RedBlackTree<Key, Value>
    {
        LIB_RedBlackTree<Key, Value, int> tree;
        public LIB_RedBlackTree(Comparison<Key> c) { tree = new LIB_RedBlackTree<Key, Value, int>(default(Value), 0, null, null, null, c, false); }
        public LIB_RedBlackTree() : this(Comparer<Key>.Default.Compare) { }
        public void Add(Key key, Value val) => tree.Add(key, val);
        public void Remove(Key key) => tree.Remove(key);
        public void RemoveAt(long index) => tree.RemoveAt(index);
        public KeyValuePair<Key, Value> this[long i]
        {
            get { return tree[i]; }
        }
        public bool ContainsKey(Key key) => tree.ContainsKey(key);
        public long UpperBound(Key key) => tree.UpperBound(key);
        public long LowerBound(Key key) => tree.LowerBound(key);
        public KeyValuePair<Key, Value> Min() => tree.Min();
        public KeyValuePair<Key, Value> Max() => tree.Max();
        public bool Any() => tree.Any();
        public long Count => tree.Count;
        public IEnumerable<KeyValuePair<Key, Value>> List() => tree.List();
    }
    class LIB_RedBlackTree<T>
    {
        LIB_RedBlackTree<T, int, int> tree;
        public LIB_RedBlackTree(Comparison<T> c) { tree = new LIB_RedBlackTree<T, int, int>(0, 0, null, null, null, c, false); }
        public LIB_RedBlackTree() : this(Comparer<T>.Default.Compare) { }
        public void Add(T val) => tree.Add(val, 0);
        public void Remove(T val) => tree.Remove(val);
        public void RemoveAt(long index) => tree.RemoveAt(index);
        public T this[long i]
        {
            get { return tree[i].Key; }
        }
        public bool ContainsKey(T val) => tree.ContainsKey(val);
        public long UpperBound(T val) => tree.UpperBound(val);
        public long LowerBound(T val) => tree.LowerBound(val);
        public T Min() => tree.Min().Key;
        public T Max() => tree.Max().Key;
        public bool Any() => tree.Any();
        public long Count => tree.Count;
        public IEnumerable<T> List() => tree.List().Select(e => e.Key);
    }
    ////end
}