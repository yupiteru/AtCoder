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
    class LIB_DynamicLazySegTree<T, E> where E : IEquatable<E>
    {
        class Node
        {
            public Node left;
            public Node right;
            public E val;
            public T dat;
            public E lazy;
            public long nodeL;
            public long subtreeL;
            public long nodeR;
            public long subtreeR;
            public bool isBlack;
            public bool needRecalc;
        }
        Func<T, T, T> f;
        Func<T, E, long, long, T> g;
        Func<E, E, E> h;
        T ti;
        E ei;
        Node root;
        bool isNeedFix;
        Node lmax;
        Node[] pool;
        int poolLength;
        public long MaxKey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }
        public long MinKey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }
        public LIB_DynamicLazySegTree(T ti, E ei, Func<T, T, T> f, Func<T, E, long, long, T> g, Func<E, E, E> h)
        {
            MinKey = long.MaxValue;
            MaxKey = long.MinValue;
            this.ti = ti;
            this.ei = ei;
            this.f = f;
            this.g = g;
            this.h = h;
            pool = new Node[32];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsRed(Node n) => n != null && !n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsBlack(Node n) => n != null && n.isBlack;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Eval(Node n)
        {
            if (n == null || ei.Equals(n.lazy)) return;
            n.val = h(n.val, n.lazy);
            if (!n.needRecalc) n.dat = g(n.dat, n.lazy, n.subtreeL, n.subtreeR);
            if (n.left != null) n.left.lazy = h(n.left.lazy, n.lazy);
            if (n.right != null) n.right.lazy = h(n.right.lazy, n.lazy);
            n.lazy = ei;
        }
        void Recalc(Node n)
        {
            Eval(n);
            if (!n.needRecalc) return;
            n.needRecalc = false;
            n.dat = g(ti, n.val, n.nodeL, n.nodeR);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateL(Node n)
        {
            if (n != null) { Eval(n); Eval(n.right); }
            Node m = n.right, t = m.left;
            m.left = n; n.right = t;
            n.subtreeR = t?.subtreeR ?? n.nodeR;
            m.subtreeL = n.subtreeL;
            n.needRecalc = true; m.needRecalc = true;
            return m;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node RotateR(Node n)
        {
            if (n != null) { Eval(n); Eval(n.left); }
            Node m = n.left, t = m.right;
            m.right = n; n.left = t;
            n.subtreeL = t?.subtreeL ?? n.nodeL;
            m.subtreeR = n.subtreeR;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Add(long l, long r, E val)
        {
            root = Add(root, l, r, val);
            root.isBlack = true;
        }
        Node Add(Node n, long l, long r, E val)
        {
            if (n == null)
            {
                isNeedFix = true;
                Node ret;
                if (poolLength == 0) ret = new Node() { nodeL = l, subtreeL = l, nodeR = r, subtreeR = r, val = val, dat = g(ti, val, l, r), lazy = ei };
                else
                {
                    ret = pool[--poolLength];
                    ret.nodeL = l;
                    ret.subtreeL = l;
                    ret.nodeR = r;
                    ret.subtreeR = r;
                    ret.val = val;
                    ret.dat = g(ti, val, l, r);
                    ret.lazy = ei;
                    ret.isBlack = false;
                    ret.needRecalc = false;
                    ret.left = ret.right = null;
                }
                return ret;
            }
            Eval(n);
            if (l < n.nodeL) n.left = Add(n.left, l, r, val);
            else n.right = Add(n.right, l, r, val);
            n.subtreeR = n.right?.subtreeR ?? n.nodeR;
            n.subtreeL = n.left?.subtreeL ?? n.nodeL;
            n.needRecalc = true;
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
        void Remove(long l)
        {
            root = Remove(root, l);
            if (root != null) root.isBlack = true;
        }
        Node Remove(Node n, long l)
        {
            Eval(n);
            if (l < n.nodeL)
            {
                n.left = Remove(n.left, l);
                n.needRecalc = true;
                return BalanceL(n);
            }
            if (l > n.nodeL)
            {
                n.right = Remove(n.right, l);
                n.needRecalc = true;
                return BalanceR(n);
            }
            if (n.left == null)
            {
                isNeedFix = n.isBlack;
                return n.right;
            }
            n.left = RemoveMax(n.left);
            n.nodeL = lmax.nodeL;
            n.nodeR = lmax.nodeR;
            n.val = lmax.val;
            n.lazy = ei;
            n.needRecalc = true;
            pool[poolLength++] = lmax;
            if (pool.Length == poolLength)
            {
                var tmp = new Node[pool.Length << 1];
                for (var i = 0; i < pool.Length; ++i) tmp[i] = pool[i];
                pool = tmp;
            }
            return BalanceL(n);
        }
        Node RemoveMax(Node n)
        {
            Eval(n);
            if (n.right != null)
            {
                n.right = RemoveMax(n.right);
                n.subtreeR = n.right?.subtreeR ?? n.nodeR;
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
        List<long> removeList;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForceUpdate(long l, long r, E val)
        {
            if (r <= l) return;
            removeList = new List<long>();
            cnt = 0;
            GetRemoveKeyList(root, l, r);
            if (cnt > 0) Add(l1, r1, val1);
            foreach (var key in removeList) Remove(key);
            Add(l, r, val);
        }
        void GetRemoveKeyList(Node n, long l, long r)
        {
            if (r <= l) return;
            while (n != null)
            {
                Eval(n);
                n.needRecalc = true;
                if (r <= n.nodeL) n = n.left;
                else if (n.nodeR <= l) n = n.right;
                else break;
            }
            if (n == null) return;
            if (l < n.nodeL) GetRemoveKeyList(n.left, l, n.nodeL);
            if (n.nodeR < r) GetRemoveKeyList(n.right, n.nodeR, r);
            l = Max(l, n.nodeL);
            r = Min(r, n.nodeR);
            if (n.nodeL == l && r == n.nodeR)
            {
                removeList.Add(n.nodeL);
            }
            else if (n.nodeL < l && r == n.nodeR)
            {
                n.nodeR = l;
            }
            else if (n.nodeL == l && r < n.nodeR)
            {
                cnt = 1;
                l1 = r; r1 = n.nodeR; val1 = n.val;
                removeList.Add(n.nodeL);
            }
            else if (n.nodeL < l && r < n.nodeR)
            {
                cnt = 1;
                l1 = r; r1 = n.nodeR; val1 = n.val;
                n.nodeR = l;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(long l, long r, E val)
        {
            if (r <= l) return;
            if (l < MinKey) MinKey = l;
            if (MaxKey < r) MaxKey = r;
            if (root == null) Add(l, r, val);
            else if (r < root.subtreeL)
            {
                Add(r, root.subtreeL, ei);
                Add(l, r, val);
            }
            else if (r == root.subtreeL)
            {
                Add(l, r, val);
            }
            else if (root.subtreeR == l)
            {
                Add(l, r, val);
            }
            else if (root.subtreeR < l)
            {
                Add(root.subtreeR, l, ei);
                Add(l, r, val);
            }
            else
            {
                cnt = 0;
                Update(root, l, r, val);
                if (cnt > 0) Add(l1, r1, val1);
                if (cnt > 1) Add(l2, r2, val2);
            }
        }
        long l1; long r1; E val1;
        long l2; long r2; E val2;
        long cnt;
        void Update(Node n, long l, long r, E val)
        {
            if (r <= l) return;
            while (n != null)
            {
                Eval(n);
                n.needRecalc = true;
                if (r <= n.nodeL) n = n.left;
                else if (n.nodeR <= l) n = n.right;
                else break;
            }
            if (n == null)
            {
                if (++cnt == 1)
                {
                    l1 = l; r1 = r; val1 = val;
                }
                else
                {
                    l2 = l; r2 = r; val2 = val;
                }
                return;
            }
            if (l == n.subtreeL && r == n.subtreeR) n.lazy = val;
            else
            {
                if (l < n.nodeL) Update(n.left, l, n.nodeL, val);
                if (n.nodeR < r) Update(n.right, n.nodeR, r, val);
                l = Max(l, n.nodeL);
                r = Min(r, n.nodeR);
                if (n.nodeL == l && r == n.nodeR)
                {
                    n.val = h(n.val, val);
                }
                else if (n.nodeL < l && r == n.nodeR)
                {
                    if (++cnt == 1)
                    {
                        l1 = l; r1 = r; val1 = h(n.val, val);
                    }
                    else
                    {
                        l2 = l; r2 = r; val2 = h(n.val, val);
                    }
                    n.nodeR = l;
                }
                else if (n.nodeL == l && r < n.nodeR)
                {
                    if (++cnt == 1)
                    {
                        l1 = r; r1 = n.nodeR; val1 = n.val;
                    }
                    else
                    {
                        l2 = r; r2 = n.nodeR; val2 = n.val;
                    }
                    n.val = h(n.val, val);
                    n.nodeR = r;
                }
                else if (n.nodeL < l && r < n.nodeR)
                {
                    l1 = r; r1 = n.nodeR; val1 = n.val;
                    l2 = l; r2 = r; val2 = h(n.val, val);
                    cnt = 2;
                    n.nodeR = l;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Query(long l, long r)
        {
            if (root == null) return ti;
            return Query(root, l, r);
        }
        T Query(Node n, long l, long r)
        {
            if (r <= l) return ti;
            while (n != null)
            {
                Recalc(n);
                if (r <= n.nodeL) n = n.left;
                else if (n.nodeR <= l) n = n.right;
                else break;
            }
            if (n == null) return ti;
            if (l == n.subtreeL && r == n.subtreeR)
            {
                return n.dat;
            }
            else
            {
                var v1 = l >= n.nodeL ? ti : Query(n.left, l, n.nodeL);
                var v3 = n.nodeR >= r ? ti : Query(n.right, n.nodeR, r);
                l = Max(l, n.nodeL);
                r = Min(r, n.nodeR);
                var v2 = g(ti, n.val, l, r);
                return f(f(v1, v2), v3);
            }
        }
    }
    ////end
}