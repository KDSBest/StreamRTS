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
    }
}
