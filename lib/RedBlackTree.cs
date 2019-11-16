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
    class LIB_RedBlackTree<T>
    {
        class Node
        {
            public Node left;
            public Node right;
            public T val;
            public bool isBlack;
            public int cnt;
        }
        Comparison<T> c;
        Node root;
        bool isNeedFix;
        T lmax;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree(Comparison<T> _c) { c = _c; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree() : this(Comparer<T>.Default.Compare) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsRed(Node n) => n != null && !n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsBlack(Node n) => n != null && n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long Count(Node n) => n?.cnt ?? 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateL(Node n)
        {
            Node m = n.right, t = m.left;
            m.left = n; n.right = t;
            n.cnt -= m.cnt - (t?.cnt ?? 0);
            m.cnt += n.cnt - (t?.cnt ?? 0);
            return m;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateR(Node n)
        {
            Node m = n.left, t = m.right;
            m.right = n; n.left = t;
            n.cnt -= m.cnt - (t?.cnt ?? 0);
            m.cnt += n.cnt - (t?.cnt ?? 0);
            return m;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateLR(Node n)
        {
            n.left = RotateL(n.left);
            return RotateR(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateRL(Node n)
        {
            n.right = RotateR(n.right);
            return RotateL(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T x)
        {
            root = Add(root, x);
            root.isBlack = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Add(Node n, T x)
        {
            if (n == null)
            {
                isNeedFix = true;
                return new Node() { val = x, cnt = 1 };
            }
            if (c(x, n.val) < 0) n.left = Add(n.left, x);
            else n.right = Add(n.right, x);
            n.cnt++;
            return Balance(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Balance(Node n)
        {
            if (!isNeedFix) return n;
            if (!IsBlack(n)) return n;
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
        public void Remove(T x)
        {
            root = Remove(root, x);
            if (root != null) root.isBlack = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Remove(Node n, T x)
        {
            if (n == null)
            {
                isNeedFix = false;
                return n;
            }
            n.cnt--;
            var r = c(x, n.val);
            if (r < 0)
            {
                n.left = Remove(n.left, x);
                return BalanceL(n);
            }
            if (r > 0)
            {
                n.right = Remove(n.right, x);
                return BalanceR(n);
            }
            if (n.left == null)
            {
                isNeedFix = n.isBlack;
                return n.right;
            }
            n.left = RemoveMax(n.left);
            n.val = lmax;
            return BalanceL(n);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RemoveMax(Node n)
        {
            n.cnt--;
            if (n.right != null)
            {
                n.right = RemoveMax(n.right);
                return BalanceR(n);
            }
            lmax = n.val;
            isNeedFix = n.isBlack;
            return n.left;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public T this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return At(root, i); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T At(Node n, long i)
        {
            if (n == null) return default(T);
            if (n.left == null)
                if (i == 0) return n.val;
                else return At(n.right, i - 1);
            if (n.left.cnt == i) return n.val;
            if (n.left.cnt > i) return At(n.left, i);
            return At(n.right, i - n.left.cnt - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Have(T x)
        {
            var t = FindUpper(x);
            return t < Count(root) && c(At(root, t), x) == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long FindUpper(T x, bool allowSame = true) => allowSame ? FindLower(root, x) + 1 : FindUpper(root, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long FindUpper(Node n, T x)
        {
            if (n == null) return 0;
            var r = c(x, n.val);
            if (r < 0) return FindUpper(n.left, x);
            return Count(n.left) + 1 + FindUpper(n.right, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long FindLower(T x, bool allowSame = true)
        {
            var t = FindLower(root, x);
            if (allowSame) return t + 1 < Count(root) && c(At(root, t + 1), x) == 0 ? t + 1 : t < 0 ? Count(root) : t;
            return t < 0 ? Count(root) : t;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long FindLower(Node n, T x)
        {
            if (n == null) return -1;
            var r = c(x, n.val);
            if (r > 0) return Count(n.left) + 1 + FindLower(n.right, x);
            return FindLower(n.left, x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Min()
        {
            Node n = root, p = null;
            while (n != null)
            {
                p = n;
                n = n.left;
            }
            return p == null ? default(T) : p.val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Max()
        {
            Node n = root, p = null;
            while (n != null)
            {
                p = n;
                n = n.right;
            }
            return p == null ? default(T) : p.val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => root != null;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Count() => Count(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> List() => L(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<T> L(Node n)
        {
            if (n == null) yield break;
            foreach (var i in L(n.left)) yield return i;
            yield return n.val;
            foreach (var i in L(n.right)) yield return i;
        }
    }
    class LIB_RedBlackTree<TK, TV>
    {
        LIB_RedBlackTree<KeyValuePair<TK, TV>> tree;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree(Comparison<TK> _c) { tree = new LIB_RedBlackTree<KeyValuePair<TK, TV>>((x, y) => _c(x.Key, y.Key)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree() : this(Comparer<TK>.Default.Compare) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TK k, TV v) => tree.Add(new KeyValuePair<TK, TV>(k, v));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TK k) => tree.Remove(new KeyValuePair<TK, TV>(k, default(TV)));
        public KeyValuePair<TK, TV> this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tree[i]; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Have(TK k) => tree.Have(new KeyValuePair<TK, TV>(k, default(TV)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long FindUpper(TK k, bool allowSame = true) => tree.FindUpper(new KeyValuePair<TK, TV>(k, default(TV)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long FindLower(TK k, bool allowSame = true) => tree.FindLower(new KeyValuePair<TK, TV>(k, default(TV)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TK, TV> Min() => tree.Min();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<TK, TV> Max() => tree.Max();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => tree.Any();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Count() => tree.Count();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<KeyValuePair<TK, TV>> List() => tree.List();
    }
    ////end
}