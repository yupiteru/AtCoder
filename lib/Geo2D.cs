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
    class LIB_Geo2D
    {
        static readonly public double EPS = 1e-10;
        public struct Vec
        {
            public double x;
            public double y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vec(double x, double y) { this.x = x; this.y = y; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator +(Vec a, Vec b) => new Vec() { x = a.x + b.x, y = a.y + b.y };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator -(Vec a, Vec b) => new Vec() { x = a.x - b.x, y = a.y - b.y };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator *(Vec p, double v) => new Vec() { x = p.x * v, y = p.y * v };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator *(double v, Vec p) => p * v;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator /(Vec p, double v) => new Vec() { x = p.x / v, y = p.y / v };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Vec operator /(double v, Vec p) => p / v;
        }
        public struct Line
        {
            public double a;
            public double b;
            public double c;
            /// <summary>
            /// ax + by + c = 0
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Line(double a, double b, double c) { this.a = a; this.b = b; this.c = c; }
        }
        public struct Circle
        {
            public Vec p;
            /// <summary>
            /// 半径の2乗
            /// </summary>
            public double r2;
            /// <summary>
            /// 半径
            /// </summary>
            public double r
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return Sqrt(r2); }
                private set { }
            }
        }
        /// <summary>
        /// 点と点の距離の2乗を求める
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Distance(Vec p1, Vec p2) => Norm(p2 - p1);
        /// <summary>
        /// 点と円の距離の2乗を求める。負の場合は円の内側にいる（絶対値は一番近い円の縁までの距離）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Distance(Vec p, Circle c) => Norm(p - c.p) - c.r2;
        /// <summary>
        /// 点と円の距離の2乗を求める。負の場合は円の内側にいる（絶対値は一番近い円の縁までの距離）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Distance(Circle c, Vec p) => Norm(p - c.p) - c.r2;
        /// <summary>
        /// 円と直線の交点を求める
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Vec[] Intersection(Line l, Circle c) => Intersection(c, l);
        /// <summary>
        /// 円と直線の交点を求める
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Vec[] Intersection(Circle c, Line l)
        {
            var d = l.a * c.p.x + l.b * c.p.y + l.c;
            var ab2 = (l.a * l.a + l.b * l.b);
            var d2 = ab2 * c.r2 - d * d;
            if (d2 < -EPS) return new Vec[0];
            if (d2 > EPS)
            {
                var rd = Sqrt(d2);
                var p1 = new Vec((-l.a * d + l.b * rd) / ab2 + c.p.x, (-l.b * d - l.a * rd) / ab2 + c.p.y);
                var p2 = new Vec((-l.a * d - l.b * rd) / ab2 + c.p.x, (-l.b * d + l.a * rd) / ab2 + c.p.y);
                return new[] { p1, p2 };
            }
            return new[] { new Vec(-l.a * d / ab2 + c.p.x, -l.b * d / ab2 + c.p.y) };
        }
        /// <summary>
        /// 円と円の交点を求める
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Vec[] Intersection(Circle c1, Circle c2)
        {
            var p = c2.p - c1.p;
            var b = p.x * p.x + p.y * p.y;
            var a = (b + c1.r2 - c2.r2) / 2;
            var d = b * c1.r2 - a * a;
            if (d < -EPS) return new Vec[0];
            if (d > EPS)
            {
                var c = Sqrt(d);
                var p1 = new Vec((a * p.x + p.y * c) / b + c1.p.x, (a * p.y - p.x * c) / b + c1.p.y);
                var p2 = new Vec((a * p.x - p.y * c) / b + c1.p.x, (a * p.y + p.x * c) / b + c1.p.y);
                return new[] { p1, p2 };
            }
            return new[] { new Vec(a * p.x / b + c1.p.x, a * p.y / b + c1.p.y) };
        }
        /// <summary>
        /// pの長さの2乗を得る
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Norm(Vec p) => p.x * p.x + p.y * p.y;
        /// <summary>
        /// 内積
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Dot(Vec p1, Vec p2) => p1.x * p2.x + p1.y * p2.y;
        /// <summary>
        /// 外積（平行四辺形の面積）負になり得る
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public double Cross(Vec p1, Vec p2) => p1.x * p2.y - p1.y * p2.x;
        /// <summary>
        /// 3点を通る円を返す
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Circle GetCircleFrom3Vec(Vec p1, Vec p2, Vec p3)
        {
            var A = Distance(p2, p3);
            var B = Distance(p1, p3);
            var C = Distance(p1, p2);
            var S = Abs(Cross(p2 - p1, p3 - p1));
            var p = (A * (B + C - A) * p1 + B * (C + A - B) * p2 + C * (A + B - C) * p3) / (4 * S * S);
            var r = Distance(p, p1);
            return new Circle() { p = p, r2 = r };
        }
        /// <summary>
        /// 2点を通る円を返す
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Circle GetCircleFrom2Vec(Vec p1, Vec p2)
        {
            var c = (p1 + p2) / 2;
            return new Circle() { p = c, r2 = Distance(p1, c) };
        }
        /// <summary>
        /// 最小包含円を返す
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public Circle GetEncloseCircle(IEnumerable<Vec> pointList)
        {
            var pList = pointList.ToArray();
            if (pList.Length < 2) throw new Exception();
            var rnd = new Random();
            var list = pList.OrderBy(_ => rnd.Next()).ToArray();
            Func<Vec, Circle, bool> inCircle = null;
            inCircle = (p, c) => Distance(p, c) < EPS;
            {
                var c = GetCircleFrom2Vec(list[0], list[1]);
                for (var i = 2; i < list.Length; i++)
                {
                    if (!inCircle(list[i], c))
                    {
                        c = GetCircleFrom2Vec(list[0], list[i]);
                        for (var j = 1; j < i; j++)
                        {
                            if (!inCircle(list[j], c))
                            {
                                c = GetCircleFrom2Vec(list[i], list[j]);
                                for (var k = 0; k < j; k++)
                                {
                                    if (!inCircle(list[k], c))
                                    {
                                        c = GetCircleFrom3Vec(list[i], list[j], list[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                return c;
            }
        }
    }
    ////end
}