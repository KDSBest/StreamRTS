using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClipperLib;
using Poly2Tri;
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
        public NavMeshPolygon NavMesh;
        public Grid Grid = new Grid();

        public Map(NavigationPolygon floor, NavigationPolygons staticObjects)
        {
            this.floor = floor;
            this.staticObjects = staticObjects;

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

            Triangulate();

            Grid.Initialize(floor, NavMesh);
        }

        private void Triangulate()
        {
            var polygons = new List<NavMeshPolygon>();

            foreach (var region in FloorWithDynamicObjects)
            {
                var polygonPoints = new List<PolygonPoint>();
                foreach (var p in region)
                {
                    polygonPoints.Add(new PolygonPoint(p.X, p.Y));
                }
                polygons.Add(new NavMeshPolygon(polygonPoints));
            }

            NavMesh = polygons[polygons.Count - 1];
            for (int i = 0; i < polygons.Count - 1; i++)
            {
                NavMesh.AddHole(polygons[i]);
            }

            P2T.Triangulate(NavMesh);
        }
    }

}
