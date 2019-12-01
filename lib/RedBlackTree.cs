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
    class LIB_RedBlackTree<Key, Value>
    {
        class Node
        {
            public Node left;
            public Node right;
            public Key key;
            public Value val;
            public bool isBlack;
            public int cnt;
        }
        Comparison<Key> c;
        Node root;
        bool isNeedFix;
        Node lmax;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree(Comparison<Key> _c) { c = _c; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree() : this(Comparer<Key>.Default.Compare) { }
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
        public void Add(Key key, Value val)
        {
            root = Add(root, key, val);
            root.isBlack = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Add(Node n, Key key, Value val)
        {
            if (n == null)
            {
                isNeedFix = true;
                return new Node() { key = key, val = val, cnt = 1 };
            }
            if (c(key, n.key) < 0) n.left = Add(n.left, key, val);
            else n.right = Add(n.right, key, val);
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
        public void Remove(Key key)
        {
            root = Remove(root, key);
            if (root != null) root.isBlack = true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node Remove(Node n, Key key)
        {
            if (n == null)
            {
                isNeedFix = false;
                return n;
            }
            n.cnt--;
            var r = c(key, n.key);
            if (r < 0)
            {
                n.left = Remove(n.left, key);
                return BalanceL(n);
            }
            if (r > 0)
            {
                n.right = Remove(n.right, key);
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
            lmax = n;
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
        public KeyValuePair<Key, Value> this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return At(root, i); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        KeyValuePair<Key, Value> At(Node n, long i)
        {
            if (n.left == null)
            {
                if (i == 0) return new KeyValuePair<Key, Value>(n.key, n.val);
                else return At(n.right, i - 1);
            }
            if (n.left.cnt == i) return new KeyValuePair<Key, Value>(n.key, n.val);
            if (n.left.cnt > i) return At(n.left, i);
            return At(n.right, i - n.left.cnt - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Have(Key key)
        {
            var t = LowerBound(key);
            return t < Count(root) && c(At(root, t).Key, key) == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long UpperBound(Key key) => UpperBound(root, key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long UpperBound(Node n, Key key)
        {
            if (n == null) return 0;
            var r = c(key, n.key);
            if (r < 0) return UpperBound(n.left, key);
            return Count(n.left) + 1 + UpperBound(n.right, key);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LowerBound(Key key) => LowerBound(root, key);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long LowerBound(Node n, Key key)
        {
            if (n == null) return 0;
            var r = c(key, n.key);
            if (r <= 0) return LowerBound(n.left, key);
            return Count(n.left) + 1 + LowerBound(n.right, key);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<Key, Value> Min()
        {
            Node n = root, p = null;
            while (n != null)
            {
                p = n;
                n = n.left;
            }
            return new KeyValuePair<Key, Value>(n.key, n.val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<Key, Value> Max()
        {
            Node n = root, p = null;
            while (n != null)
            {
                p = n;
                n = n.right;
            }
            return new KeyValuePair<Key, Value>(n.key, n.val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => root != null;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Count() => Count(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<KeyValuePair<Key, Value>> List() => L(root);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerable<KeyValuePair<Key, Value>> L(Node n)
        {
            if (n == null) yield break;
            foreach (var i in L(n.left)) yield return i;
            yield return new KeyValuePair<Key, Value>(n.key, n.val);
            foreach (var i in L(n.right)) yield return i;
        }
    }
    class LIB_RedBlackTree<T>
    {
        LIB_RedBlackTree<T, T> tree;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree(Comparison<T> _c) { tree = new LIB_RedBlackTree<T, T>(_c); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_RedBlackTree() : this(Comparer<T>.Default.Compare) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T val) => tree.Add(val, val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(T val) => tree.Remove(val);
        public T this[long i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tree[i].Key; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Have(T val) => tree.Have(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long UpperBound(T val) => tree.UpperBound(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LowerBound(T val) => tree.LowerBound(val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Min() => tree.Min().Key;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Max() => tree.Max().Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => tree.Any();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Count() => tree.Count();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> List() => tree.List().Select(e => e.Key);
    }
    ////end
}