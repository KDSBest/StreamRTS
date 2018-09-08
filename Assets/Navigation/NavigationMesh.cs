﻿using System;
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
        public List<NavigationTriangle> AllTriangle = new List<NavigationTriangle>();
        private Dictionary<IntPoint, NavigationTriangle> searchCache = new Dictionary<IntPoint, NavigationTriangle>();
        private Tess tesselationAlgorithm;

        private NavigationPolygons polygons;

        public void Initialize(NavigationPolygons polygons)
        {
            Reset();

            this.polygons = polygons;

            Tesselation();
            for (int i = 0; i < this.tesselationAlgorithm.ElementCount; i++)
            {
                var v0 = this.tesselationAlgorithm.Vertices[this.tesselationAlgorithm.Elements[i * 3]].Position.ToIntPoint();
                var v1 = this.tesselationAlgorithm.Vertices[this.tesselationAlgorithm.Elements[i * 3 + 1]].Position.ToIntPoint();
                var v2 = this.tesselationAlgorithm.Vertices[this.tesselationAlgorithm.Elements[i * 3 + 2]].Position.ToIntPoint();

                var triangle = new NavigationTriangle(v0, v1, v2);
                AllTriangle.Add(triangle);
            }

            Refine();

            foreach (var triangle in AllTriangle)
            {
                AddTriangleToPointMap(triangle.U, triangle);
                AddTriangleToPointMap(triangle.V, triangle);
                AddTriangleToPointMap(triangle.W, triangle);
            }
        }

        void UpdateTriangulation(NavigationTriangle t, IntPoint p)
        {
            var v = new IntPoint(p);

            AllTriangle.Add(new NavigationTriangle(t.U, t.V, v));
            AllTriangle.Add(new NavigationTriangle(t.V, t.W, v));
            AllTriangle.Add(new NavigationTriangle(t.W, t.U, v));
            AllTriangle.Remove(t);
        }


        private void Refine()
        {
            RefineSubRoutine();
        }

        void SplitTriangle(NavigationTriangle t)
        {
            if (t.S0.GetLengthSquared() <= 4 || t.S1.GetLengthSquared() <= 4 || t.S2.GetLengthSquared() <= 4)
                return;

            var c = t.GetMiddlePoint();

            if (t.IsPointInTriangle(c))
            {
                UpdateTriangulation(t, c);
            }
        }


        private void RefineSubRoutine()
        {
            var refineTriangles = AllTriangle.ToArray().ToList();

            foreach (var triangle in refineTriangles)
            {
                SplitTriangle(triangle);
            }
        }


        private void Reset()
        {
            tesselationAlgorithm = new Tess();
            triangleMapping.Clear();
            AllTriangle.Clear();
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

            for (int i = 0; i < AllTriangle.Count; i++)
            {
                var tri = AllTriangle[i];
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
                tesselationAlgorithm.AddContour(contour, ContourOrientation.Original);
            }

            tesselationAlgorithm.Tessellate(LibTessDotNet.WindingRule.Positive, LibTessDotNet.ElementType.Polygons, 3);
        }
    }
}
