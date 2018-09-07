using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace Navigation
{
    public struct NavigationEdge
    {
        public IntPoint A;
        public IntPoint B;

        public NavigationEdge(IntPoint a, IntPoint b)
        {
            A = a;
            B = b;
        }

        public NavigationPolygon BoundingBox()
        {
            var result = new NavigationPolygon(4);
            result.Add(new IntPoint(A.X, A.Y));
            result.Add(new IntPoint(B.X, A.Y));
            result.Add(new IntPoint(B.X, B.Y));
            result.Add(new IntPoint(A.X, B.Y));

            return result;
        }

        public IntPoint GetSize()
        {
            return B - A;
        }
    }
}
