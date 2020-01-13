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
        static readonly double EPS = 1e-10;
        public struct Point
        {
            public double x;
            public double y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator +(Point a, Point b) => new Point() { x = a.x + b.x, y = a.y + b.y };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator -(Point a, Point b) => new Point() { x = a.x - b.x, y = a.y - b.y };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator *(Point p, double v) => new Point() { x = p.x * v, y = p.y * v };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator *(double v, Point p) => p * v;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator /(Point p, double v) => new Point() { x = p.x / v, y = p.y / v };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static public Point operator /(double v, Point p) => p / v;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double Norm(Point p) => p.x * p.x + p.y * p.y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double Cross(Point p1, Point p2) => p1.x * p2.y - p1.y * p2.x;
        public struct Circle
        {
            public Point p;
            public double r;
        }
        List<Point> pointList;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LIB_Geo2D()
        {
            pointList = new List<Point>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPoint(double x, double y)
        {
            pointList.Add(new Point() { x = x, y = y });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Circle GetEncloseCircle()
        {
            if (pointList.Count < 2) throw new Exception();
            var rnd = new Random();
            var list = pointList.OrderBy(_ => rnd.Next()).ToArray();
            Func<Point, Point, Point, Circle> mc3 = null;
            mc3 = (a, b, c) =>
            {
                var A = Norm(b - c);
                var B = Norm(c - a);
                var C = Norm(a - b);
                var S = Abs(Cross(b - a, c - a));
                var p = (A * (B + C - A) * a + B * (C + A - B) * b + C * (A + B - C) * c) / (4 * S * S);
                var r = Norm(p - a);
                return new Circle() { p = p, r = r };
            };
            Func<Point, Point, Circle> mc2 = null;
            mc2 = (a, b) =>
            {
                var c = (a + b) / 2;
                return new Circle() { p = c, r = Norm(a - c) };
            };
            Func<Point, Circle, bool> inCircle = null;
            inCircle = (p, c) => Norm(p - c.p) <= c.r + EPS;
            {
                var c = mc2(list[0], list[1]);
                for (var i = 2; i < list.Length; i++)
                {
                    if (!inCircle(list[i], c))
                    {
                        c = mc2(list[0], list[i]);
                        for (var j = 1; j < i; j++)
                        {
                            if (!inCircle(list[j], c))
                            {
                                c = mc2(list[i], list[j]);
                                for (var k = 0; k < j; k++)
                                {
                                    if (!inCircle(list[k], c))
                                    {
                                        c = mc3(list[i], list[j], list[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                return new Circle() { p = c.p, r = Sqrt(c.r) };
            }
        }
    }
    ////end
}