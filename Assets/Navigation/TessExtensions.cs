using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using LibTessDotNet;

namespace Navigation
{
    public static class TessExtensions
    {
        public static NavigationTriangle ToNavigationTriangle(this Tess navMesh, int i)
        {
            var v0 = navMesh.Vertices[navMesh.Elements[i * 3]].Position;
            var v1 = navMesh.Vertices[navMesh.Elements[i * 3 + 1]].Position;
            var v2 = navMesh.Vertices[navMesh.Elements[i * 3 + 2]].Position;
            return new NavigationTriangle(new IntPoint(v0.X, v0.Y), new IntPoint(v1.X, v1.Y), new IntPoint(v2.X, v2.Y));
        }
    }
}
