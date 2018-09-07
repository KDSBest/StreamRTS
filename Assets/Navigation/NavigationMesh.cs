using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using LibTessDotNet;
using Navigation;

namespace Assets.Navigation
{
    public class NavigationMesh
    {
        private Dictionary<IntPoint, List<NavigationTriangle>> triangleMapping = new Dictionary<IntPoint, List<NavigationTriangle>>();
        private List<NavigationTriangle> allTriangle = new List<NavigationTriangle>();
        private Dictionary<IntPoint, NavigationTriangle> searchCache = new Dictionary<IntPoint, NavigationTriangle>();
        public Tess TesselationAlgorithm;
        private NavigationPolygons polygons;

        public void Initialize(NavigationPolygons polygons)
        {
            Reset();

            this.polygons = polygons;
            Tesselation();

            for (int i = 0; i < this.TesselationAlgorithm.ElementCount; i++)
            {
                var v0 = this.TesselationAlgorithm.Vertices[this.TesselationAlgorithm.Elements[i * 3]].Position.ToIntPoint();
                var v1 = this.TesselationAlgorithm.Vertices[this.TesselationAlgorithm.Elements[i * 3 + 1]].Position.ToIntPoint();
                var v2 = this.TesselationAlgorithm.Vertices[this.TesselationAlgorithm.Elements[i * 3 + 2]].Position.ToIntPoint();

                var triangle = new NavigationTriangle(v0, v1, v2);

                AddTriangleToPointMap(v0, triangle);
                AddTriangleToPointMap(v1, triangle);
                AddTriangleToPointMap(v2, triangle);
                allTriangle.Add(triangle);
            }
        }

        private void Reset()
        {
            TesselationAlgorithm = new Tess();
            triangleMapping.Clear();
            allTriangle.Clear();
            searchCache.Clear();
        }

        public List<NavigationTriangle> GetTrianglesWithPoint(IntPoint p)
        {
            if (triangleMapping.ContainsKey(p))
                return triangleMapping[p];

            return new List<NavigationTriangle>();
        }

        public NavigationTriangle SearchTriangleForPoint(IntPoint p)
        {
            if (searchCache.ContainsKey(p))
                return searchCache[p];

            for (int i = 0; i < allTriangle.Count; i++)
            {
                var tri = allTriangle[i];
                if (tri.IsPointInTriangle(p))
                {
                    searchCache.Add(p, tri);
                    return tri;
                }
            }

            return null;
        }

        private void AddTriangleToPointMap(IntPoint p, NavigationTriangle tri)
        {
            if (!triangleMapping.ContainsKey(p))
                triangleMapping.Add(p, new List<NavigationTriangle>());

            triangleMapping[p].Add(tri);
        }

        private void Tesselation()
        {
            for (int polyIndex = 0; polyIndex < polygons.Count; polyIndex++)
            {
                var contour = new List<ContourVertex>();
                foreach (var p in polygons[polyIndex])
                {
                    contour.Add(new ContourVertex(new Vec3(p.X, p.Y, 0), polyIndex));
                }
                TesselationAlgorithm.AddContour(contour, ContourOrientation.Original);
            }

            TesselationAlgorithm.Tessellate(LibTessDotNet.WindingRule.Positive, LibTessDotNet.ElementType.Polygons, 3);
        }
    }
}
