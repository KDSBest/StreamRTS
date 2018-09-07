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
