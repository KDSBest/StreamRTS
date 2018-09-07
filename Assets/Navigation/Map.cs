using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClipperLib;
using LibTessDotNet;
using UnityEngine;

namespace Navigation
{
    public class Map
    {
        private Clipper clipper = new Clipper();

        private NavigationPolygon floor;
        private NavigationPolygons staticObjects;
        private NavigationPolygons dynamicObjects = new NavigationPolygons();
        private NavigationPolygons floorWithStaticObjects = new NavigationPolygons();

        public NavigationPolygons FloorWithDynamicObjects = new NavigationPolygons();
        public Tess NavMesh;
        public Grid Grid = new Grid();

        public Map(NavigationPolygon floor, NavigationPolygons staticObjects)
        {
            this.floor = floor;
            this.staticObjects = staticObjects;

            Initialize();
        }

        private void Initialize()
        {
            Clip(floor, staticObjects);

            Triangulate();

            Grid.Initialize(floor, NavMesh);
        }

        private void Clip(NavigationPolygon floor, NavigationPolygons staticObjects)
        {
            if (!clipper.AddPath(floor, PolyType.ptSubject, true))
                throw new Exception("Can't add Paths (Subject).");
            if (!clipper.AddPaths(staticObjects, PolyType.ptClip, true))
                throw new Exception("Can't add Paths (Clip).");

            // we do it twice (it's easier than deep copy the polygons), since this is loading screen time... We don't mind performance too much here
            if (!clipper.Execute(ClipType.ctDifference, floorWithStaticObjects, PolyFillType.pftNonZero))
                throw new Exception("Can't clip floorWithStaticObjects.");

            if (!clipper.Execute(ClipType.ctDifference, FloorWithDynamicObjects, PolyFillType.pftNonZero))
                throw new Exception("Can't clip floorWithStaticObjects.");

            clipper.Reset();
        }

        private void Triangulate()
        {
            NavMesh = new Tess();

            for (int polyIndex = 0; polyIndex < FloorWithDynamicObjects.Count; polyIndex++)
            {
                var contour = new List<ContourVertex>();
                foreach (var p in FloorWithDynamicObjects[polyIndex])
                {
                    contour.Add(new ContourVertex(new Vec3(p.X, p.Y, 0), polyIndex));
                }
                NavMesh.AddContour(contour, ContourOrientation.Original);
            }

            //NavMesh = polygons[polygons.Count - 1];
            //for (int i = 0; i < polygons.Count - 1; i++)
            //{
            //    NavMesh.AddHole(polygons[i]);
            //}

            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.

            // Tessellate!
            // The winding rule determines how the different contours are combined together.
            // See http://www.glprogramming.com/red/chapter11.html (section "Winding Numbers and Winding Rules") for more information.
            // If you want triangles as output, you need to use "Polygons" type as output and 3 vertices per polygon.
            NavMesh.Tessellate(LibTessDotNet.WindingRule.Positive, LibTessDotNet.ElementType.Polygons, 3);
        }
    }

}
