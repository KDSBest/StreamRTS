using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Navigation;
using Assets.Navigation.AStar;
using ClipperLib;
using LibTessDotNet;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Navigation
{
    public class Map
    {
        private NavigationPolygon floor;
        private NavigationPolygons staticObjects;
        private List<List<NavigationEdge>> dynamicObjects = new List<List<NavigationEdge>>();
        private NavigationPolygons floorWithStaticObjects = new NavigationPolygons();
        
        public AStar Pathfinding;
        public NavigationPolygons FloorWithDynamicObjects = new NavigationPolygons();
        public NavigationMesh NavigationMesh = new NavigationMesh();
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

            OnFloorWithDynamicObjectsChanged();

            Grid.Initialize(floor);

            Grid.PlaceStaticObjects(staticObjects.ConvertAll(x => x.GetBounding()).ToList());
        }

        /// <summary>
        /// Adds the dynamic object.
        /// </summary>
        /// <param name="polygons">The polygons:
        /// has to be aligned with x- and y-axies</param>
        /// <returns></returns>
        public bool AddDynamicObject(List<NavigationEdge> polygons)
        {
            bool isSuccessful = Grid.PlaceDynamicObjects(polygons);

            if (isSuccessful)
            {
                dynamicObjects.Add(polygons);

                Clip(FloorWithDynamicObjects, polygons);

                OnFloorWithDynamicObjectsChanged();
            }

            return isSuccessful;
        }

        public bool RemoveDynamicObject(List<NavigationEdge> polygons)
        {
            bool isSuccessful = Grid.RemoveDynamicObjects(polygons);

            if (isSuccessful)
            {
                dynamicObjects.Remove(polygons);

                Union(FloorWithDynamicObjects, polygons);

                OnFloorWithDynamicObjectsChanged();
            }

            return isSuccessful;
        }

        private void OnFloorWithDynamicObjectsChanged()
        {
            FloorWithDynamicObjects.CalculateConstrainedEdges();
            NavigationMesh.Initialize(FloorWithDynamicObjects);
            Pathfinding = new AStar(NavigationMesh, FloorWithDynamicObjects);
        }

        private void Clip(NavigationPolygons floor, List<NavigationEdge> dynamicObject)
        {
            if (dynamicObject.Count == 0)
            {
                FloorWithDynamicObjects = floorWithStaticObjects.DeepCopy();
            }
            else
            {
                Clipper clipper = new Clipper();
                if (!clipper.AddPaths(floor, PolyType.ptSubject, true))
                    throw new Exception("Can't add Paths (Subject).");

                if (!clipper.AddPaths(new NavigationPolygons(dynamicObject.ConvertAll(x => x.BoundingBox())), PolyType.ptClip, true))
                    throw new Exception("Can't add Paths (Clip).");

                NavigationPolygons result = new NavigationPolygons();
                if (!clipper.Execute(ClipType.ctDifference, result, PolyFillType.pftNonZero))
                    throw new Exception("Can't clip dynamicObjects.");
                FloorWithDynamicObjects = result;
            }
        }

        private void Union(NavigationPolygons floor, List<NavigationEdge> dynamicObject)
        {
            if (dynamicObject.Count == 0)
            {
                return;
            }

            Clipper clipper = new Clipper();
            if (!clipper.AddPaths(floor, PolyType.ptSubject, true))
                throw new Exception("Can't add Paths (Subject).");

            if (!clipper.AddPaths(new NavigationPolygons(dynamicObject.ConvertAll(x => x.BoundingBox())), PolyType.ptClip, true))
                throw new Exception("Can't add Paths (Clip).");

            NavigationPolygons result = new NavigationPolygons();
            if (!clipper.Execute(ClipType.ctUnion, result, PolyFillType.pftNonZero))
                throw new Exception("Can't clip dynamicObjects.");
            FloorWithDynamicObjects = result;
        }

        private void Clip(NavigationPolygon floor, NavigationPolygons staticObjects)
        {
            Clipper clipper = new Clipper();
            if (!clipper.AddPath(floor, PolyType.ptSubject, true))
                throw new Exception("Can't add Paths (Subject).");

            if (staticObjects.Count > 0 && !clipper.AddPaths(staticObjects, PolyType.ptClip, true))
                throw new Exception("Can't add Paths (Clip).");

            // we do it twice (it's easier than deep copy the polygons), since this is loading screen time... We don't mind performance too much here
            if (!clipper.Execute(ClipType.ctDifference, floorWithStaticObjects, PolyFillType.pftNonZero))
                throw new Exception("Can't clip floorWithStaticObjects.");

            if (!clipper.Execute(ClipType.ctDifference, FloorWithDynamicObjects, PolyFillType.pftNonZero))
                throw new Exception("Can't clip floorWithStaticObjects.");

            clipper.Reset();
        }
    }

}
