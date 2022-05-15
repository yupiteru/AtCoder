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
    class LIB_ConvexHullTrick
    {
        class Node
        {
            public Node left;
            public Node right;
            public long a;
            public long b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node(long a, long b)
            {
                this.a = a;
                this.b = b;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long f(long x)
            {
                return a * x + b;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Swap(Node x, Node y)
        {
            var t = x.a; x.a = y.a; y.a = t;
            var t2 = x.b; x.b = y.b; y.b = t2;
        }
        void AddLine(Node cur, Node nw, long l, long r)
        {
            while (true)
            {
                if (nw.f(l) < cur.f(l)) Swap(cur, nw);
                if (cur.f(r - 1) <= nw.f(r - 1)) break;
                var mid = (l + r) / 2;
                if (cur.f(mid) <= nw.f(mid))
                {
                    if (cur.right == null)
                    {
                        cur.right = nw;
                        break;
                    }
                    else
                    {
                        cur = cur.right;
                        l = mid;
                    }
                }
                else
                {
                    Swap(cur, nw);
                    if (cur.left == null)
                    {
                        cur.left = nw;
                        break;
                    }
                    else
                    {
                        cur = cur.left;
                        r = mid;
                    }
                }
            }
        }
        long Query(Node cur, long k, long l, long r)
        {
            var ans = long.MaxValue;
            while (cur != null)
            {
                ans = Min(ans, cur.f(k));
                var mid = (l + r) / 2;
                if (k < mid)
                {
                    cur = cur.left;
                    r = mid;
                }
                else
                {
                    cur = cur.right;
                    l = mid;
                }
            }
            return ans;
        }
        long lpos;
        long rpos;
        Node root;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ConvexHullTrick(long leftLimit, long rightLimit)
        {
            root = new Node(0, long.MaxValue);
            lpos = leftLimit;
            rpos = rightLimit;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLine(long a, long b)
        {
            var nw = new Node(a, b);
            AddLine(root, nw, lpos, rpos);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Query(long x)
        {
            return Query(root, x, lpos, rpos);
        }
    }
    class LIB_ConvexHullTrickDouble
    {
        class Node
        {
            public Node left;
            public Node right;
            public double a;
            public double b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public double f(double x)
            {
                return a * x + b;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Swap(Node x, Node y)
        {
            var t = x.a; x.a = y.a; y.a = t;
            var t2 = x.b; x.b = y.b; y.b = t2;
        }
        void AddLine(Node cur, Node nw, double l, double r)
        {
            while (true)
            {
                if (nw.f(l) < cur.f(l)) Swap(cur, nw);
                if (cur.f(r - 1) <= nw.f(r - 1)) break;
                var mid = (l + r) / 2;
                if (cur.f(mid) <= nw.f(mid))
                {
                    if (cur.right == null)
                    {
                        cur.right = nw;
                        break;
                    }
                    else
                    {
                        cur = cur.right;
                        l = mid;
                    }
                }
                else
                {
                    Swap(cur, nw);
                    if (cur.left == null)
                    {
                        cur.left = nw;
                        break;
                    }
                    else
                    {
                        cur = cur.left;
                        r = mid;
                    }
                }
            }
        }
        double Query(Node cur, double k, double l, double r)
        {
            var ans = double.MaxValue;
            while (cur != null)
            {
                ans = Min(ans, cur.f(k));
                var mid = (l + r) / 2;
                if (k < mid)
                {
                    cur = cur.left;
                    r = mid;
                }
                else
                {
                    cur = cur.right;
                    l = mid;
                }
            }
            return ans;
        }
        double lpos;
        double rpos;
        Node root;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ConvexHullTrickDouble(double leftLimit, double rightLimit)
        {
            root = new Node(0, double.MaxValue);
            lpos = leftLimit;
            rpos = rightLimit;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLine(double a, double b)
        {
            var nw = new Node(a, b);
            AddLine(root, nw, lpos, rpos);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Query(double x)
        {
            return Query(root, x, lpos, rpos);
        }
    }
    ////end
}
