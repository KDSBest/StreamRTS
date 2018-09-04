﻿// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

using System;

namespace Navigation.Clipper
{
    /// <summary>
    /// Provides the raw computation functions that takes epsilon into account.
    /// zero is defined to be between (-epsilon, epsilon) exclusive
    /// </summary>
    public class Epsilon
    {
        public static bool pointAboveOrOnLine(Point pt, Point left, Point right)
        {
            var Ax = left.x;
            var Ay = left.y;
            var Bx = right.x;
            var By = right.y;
            var Cx = pt.x;
            var Cy = pt.y;

            return (Bx - Ax) * (Cy - Ay) - (By - Ay) * (Cx - Ax) >= 0;
        }

        public static bool pointBetween(Point pt, Point left, Point right)
        {
            // p must be collinear with left->right
            // returns false if p == left, p == right, or left == right
            var d_py_ly = pt.y - left.y;
            var d_rx_lx = right.x - left.x;
            var d_px_lx = pt.x - left.x;
            var d_ry_ly = right.y - left.y;

            var dot = d_px_lx * d_rx_lx + d_py_ly * d_ry_ly;

            // if `dot` is 0, then `p` == `left` or `left` == `right` (reject)
            // if `dot` is less than 0, then `p` is to the left of `left` (reject)
            if (dot <= 0)
                return false;

            var sqlen = d_rx_lx * d_rx_lx + d_ry_ly * d_ry_ly;

            // if `dot` > `sqlen`, then `p` is to the right of `right` (reject)
            // therefore, if `dot - sqlen` is greater than 0, then `p` is to the right of `right` (reject)
            if (dot - sqlen > 0)
                return false;

            return true;
        }

        public static bool pointsSameX(Point p1, Point p2)
        {
            return Math.Abs(p1.x - p2.x) == 0;
        }

        public static bool pointsSameY(Point p1, Point p2)
        {
            return Math.Abs(p1.y - p2.y) == 0;
        }

        public static bool pointsSame(Point p1, Point p2)
        {
            return
                Math.Abs(p1.x - p2.x) == 0 &&
                Math.Abs(p1.y - p2.y) == 0;
        }

        public static int pointsCompare(Point p1, Point p2)
        {
            // returns -1 if p1 is smaller, 1 if p2 is smaller, 0 if equal
            if (pointsSameX(p1, p2))
                return pointsSameY(p1, p2) ? 0 : (p1.y < p2.y ? -1 : 1);

            return p1.x < p2.x ? -1 : 1;
        }

        public static bool pointsCollinear(Point p1, Point p2, Point p3)
        {
            // does pt1->pt2->pt3 make a straight line?
            // essentially this is just checking to see if the slope(pt1->pt2) === slope(pt2->pt3)
            // if slopes are equal, then they must be collinear, because they share pt2
            var dx1 = p1.x - p2.x;
            var dy1 = p1.y - p2.y;
            var dx2 = p2.x - p3.x;
            var dy2 = p2.y - p3.y;

            return Math.Abs(dx1 * dy2 - dx2 * dy1) == 0;
        }

        public static bool linesIntersect(Point a0, Point a1, Point b0, Point b1, out Intersection intersection)
        {
            // returns false if the lines are coincident (e.g., parallel or on top of each other)
            //
            // returns an object if the lines intersect:
            //   {
            //     pt: [x, y],    where the intersection point is at
            //     alongA: where intersection point is along A,
            //     alongB: where intersection point is along B
            //   }
            //
            //  alongA and alongB will each be one of: -2, -1, 0, 1, 2
            //
            //  with the following meaning:
            //
            //    -2   intersection point is before segment's first point
            //    -1   intersection point is directly on segment's first point
            //     0   intersection point is between segment's first and second points (exclusive)
            //     1   intersection point is directly on segment's second point
            //     2   intersection point is after segment's second point

            var adx = a1.x - a0.x;
            var ady = a1.y - a0.y;
            var bdx = b1.x - b0.x;
            var bdy = b1.y - b0.y;

            var axb = adx * bdy - ady * bdx;
            if (Math.Abs(axb) == 0)
            {
                intersection = Intersection.Empty;
                return false; // lines are coincident
            }

            var dx = a0.x - b0.x;
            var dy = a0.y - b0.y;

            var A = (bdx * dy - bdy * dx) * 10 / axb;
            var B = (adx * dy - ady * dx) * 10 / axb;

            intersection = new Intersection()
            {
                alongA = 0,
                alongB = 0,
                pt = new Point()
                {
                    x = a0.x + A * adx / 10,
                    y = a0.y + A * ady / 10
                }
            };

            // categorize where intersection point is along A and B

            if (A < 0)
                intersection.alongA = -2;
            else if (A == 0)
                intersection.alongA = -1;
            else if (A >= 1 && A <= 9)
                intersection.alongA = 0;
            else if (A == 10)
                intersection.alongA = 1;
            else
                intersection.alongA = 2;

            if (B < 0)
                intersection.alongB = -2;
            else if (B == 0)
                intersection.alongB = -1;
            else if (B >= 1 && B <= 9)
                intersection.alongB = 0;
            else if (B == 10)
                intersection.alongB = 1;
            else
                intersection.alongB = 2;

            return true;
        }
    }
}