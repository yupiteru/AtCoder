using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Library
{
    ////start
    class LIB_ConvexHullTrickRangeQuery
    {
        const long INF = 1000000000000000000;
        struct Line : IEquatable<Line>
        {
            public long a;
            public long b;
            public Line(long a, long b)
            {
                this.a = a;
                this.b = b;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long f(long x)
            {
                return a * x + b;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(long c)
            {
                if (b >= INF || c >= INF)
                {
                    a = 0;
                    b = INF;
                }
                else
                {
                    b += c;
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Line other) => a == other.a && b == other.b;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override bool Equals(object obj) => this.Equals((Line)obj);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode() => (a * 31 + b).GetHashCode();
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Line lhs, Line rhs) => lhs.Equals(rhs);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Line lhs, Line rhs) => !(lhs == rhs);
        }
        static readonly Line INF_LINE = new Line(0, INF);
        struct Node
        {
            public int left;
            public int right;
            public Line line;
            public long lazy;
            public long min;
        }
        static Node[] nodeList = new Node[10000000];
        static int nodeCount = 0;
        static LIB_ConvexHullTrickRangeQuery()
        {
            nodeList[0].min = INF;
            nodeList[0].line = INF_LINE;
        }
        int root;
        long xl;
        long xr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Compare(Line l0, Line l1, long x) => l0.f(x) <= l1.f(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushLine(int nodeIdx, long l, long r)
        {
            ref var node = ref nodeList[nodeIdx];
            if (node.line == INF_LINE) return;
            var m = (l + r) / 2;
            node.left = AddLine(node.left, node.line, l, m);
            node.right = AddLine(node.right, node.line, m, r);
            node.line = INF_LINE;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddLazy(int nodeIdx, long c)
        {
            if (nodeIdx == 0) return;
            ref var node = ref nodeList[nodeIdx];
            node.line.Add(c);
            node.lazy = Min(node.lazy + c, INF);
            node.min = Min(node.min + c, INF);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushLazy(int nodeIdx, long l, long r)
        {
            ref var node = ref nodeList[nodeIdx];
            if (node.lazy == 0) return;
            AddLazy(node.left, node.lazy);
            AddLazy(node.right, node.lazy);
            node.lazy = 0;
        }
        int AddLine(int nodeIdx, Line line, long l, long r)
        {
            if (r <= l) return 0;
            if (nodeIdx == 0)
            {
                nodeIdx = ++nodeCount;
                ref var node = ref nodeList[nodeIdx];
                node.line = line;
                node.min = Min(line.f(l), line.f(r - 1));
            }
            else
            {
                var m = (l + r) / 2;
                PushLazy(nodeIdx, l, r);
                ref var node = ref nodeList[nodeIdx];
                var left = Compare(line, node.line, l);
                var mid = Compare(line, node.line, m);
                var right = Compare(line, node.line, r);
                if (mid)
                {
                    node.min = Min(Min(line.f(l), line.f(r - 1)), Min(nodeList[node.right].min, nodeList[node.left].min));
                    var t = node.line; node.line = line; line = t;
                }
                if (r - l > 1 && left != right)
                {
                    if (left != mid)
                    {
                        node.left = AddLine(node.left, line, l, m);
                        node.min = Min(node.min, nodeList[node.left].min);
                    }
                    else
                    {
                        node.right = AddLine(node.right, line, m, r);
                        node.min = Min(node.min, nodeList[node.right].min);
                    }
                }
            }
            return nodeIdx;
        }
        int AddValue(long a, long b, int nodeIdx, long c, long l, long r)
        {
            if (r <= a || b <= l)
            {
                return nodeIdx;
            }
            if (a <= l && r <= b)
            {
                AddLazy(nodeIdx, c);
                return nodeIdx;
            }
            PushLazy(nodeIdx, l, r);
            PushLine(nodeIdx, l, r);
            if (nodeIdx == 0)
            {
                nodeIdx = ++nodeCount;
                nodeList[nodeIdx].line = INF_LINE;
            }
            ref var node = ref nodeList[nodeIdx];
            var m = (l + r) / 2;
            node.left = AddValue(a, b, node.left, c, l, m);
            node.right = AddValue(a, b, node.right, c, m, r);
            node.min = Min(Min(node.line.f(l), node.line.f(r - 1)), Min(nodeList[node.right].min, nodeList[node.left].min));
            return nodeIdx;
        }
        int AddSegmentLine(long a, long b, int nodeIdx, Line line, long l, long r)
        {
            if (r <= a || b <= l) return nodeIdx;
            if (a <= l && r <= b) return AddLine(nodeIdx, line, l, r);
            PushLazy(nodeIdx, l, r);
            if (nodeIdx == 0)
            {
                nodeIdx = ++nodeCount;
                nodeList[nodeIdx].line = INF_LINE;
            }
            ref var node = ref nodeList[nodeIdx];
            var m = (l + r) / 2;
            node.left = AddSegmentLine(a, b, node.left, line, l, m);
            node.right = AddSegmentLine(a, b, node.right, line, m, r);
            node.min = Min(Min(node.line.f(l), node.line.f(r - 1)), Min(nodeList[node.right].min, nodeList[node.left].min));
            return nodeIdx;
        }
        long QueryMin(long a, long b, int nodeIdx, long l, long r)
        {
            if (nodeIdx == 0) return INF;
            if (r <= a || b <= l) return INF;
            if (a <= l && r <= b) return nodeList[nodeIdx].min;
            PushLazy(nodeIdx, l, r);
            var m = (l + r) / 2;
            ref var node = ref nodeList[nodeIdx];
            return Min(Min(node.line.f(Max(a, l)), node.line.f(Min(b, r) - 1)), Min(QueryMin(a, b, node.left, l, m), QueryMin(a, b, node.right, m, r)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_ConvexHullTrickRangeQuery(long leftLimit, long rightLimit)
        {
            xl = leftLimit;
            xr = rightLimit;
        }
        /// <summary>
        /// a_i := min(a_i, a*i + b) for i in [xl, xr)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLine(long a, long b)
        {
            root = AddLine(root, new Line(a, b), xl, xr);
        }
        /// <summary>
        /// a_i := min(a_i, a*i + b) for i in [l, r)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSegmentLine(long a, long b, long left, long right)
        {
            root = AddSegmentLine(left, right, root, new Line(a, b), xl, xr);
        }
        /// <summary>
        /// a_i := a_i + (a*i + b) for i in [l, r)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValue(long c, long left, long right)
        {
            root = AddValue(left, right, root, c, xl, xr);
        }
        /// <summary>
        /// a_i := +∞ for i in [l, r)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetValue(long left, long right)
        {
            root = AddValue(left, right, root, INF, xl, xr);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long QueryMin(long l, long r)
        {
            return QueryMin(l, r, root, xl, xr);
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
