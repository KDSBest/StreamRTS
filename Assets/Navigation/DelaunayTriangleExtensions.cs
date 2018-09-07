using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Poly2Tri;

namespace Navigation
{
    public struct NavigationTriangle
    {
        public IntPoint U;
        public IntPoint V;
        public IntPoint W;
    }

    public static class NavigationTriangleExtensions
    {
        private static int Sign(IntPoint p1, IntPoint p2, IntPoint p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool IsPointInTriangle(this NavigationTriangle triangle, IntPoint p)
        {
            bool b1 = Sign(p, triangle.U, triangle.V) < 0.0f;
            bool b2 = Sign(p, triangle.V, triangle.W) < 0.0f;
            bool b3 = Sign(p, triangle.W, triangle.U) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }
    }

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

        public static NavigationTriangle ToNavigationTriangle(this DelaunayTriangle triangle)
        {
            return new NavigationTriangle()
            {
                U = new IntPoint(triangle.Points[0].X, triangle.Points[0].Y),
                V = new IntPoint(triangle.Points[1].X, triangle.Points[1].Y),
                W = new IntPoint(triangle.Points[2].X, triangle.Points[2].Y)
            };
        }
    }
}
