using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Poly2Tri;

namespace Navigation
{
    public static class DelaunayTriangleExtensions
    {
        private static int Sign(IntPoint p1, TriangulationPoint p2, TriangulationPoint p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool IsPointInTriangle(this DelaunayTriangle triangle, IntPoint p)
        {
            bool b1 = Sign(p, triangle.Points[0], triangle.Points[1]) < 0.0f;
            bool b2 = Sign(p, triangle.Points[1], triangle.Points[2]) < 0.0f;
            bool b3 = Sign(p, triangle.Points[2], triangle.Points[0]) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }
    }
}
