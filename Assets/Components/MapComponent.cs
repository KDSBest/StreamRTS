using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ClipperLib;
using LibTessDotNet;
using Navigation;
using UnityEngine;
using Type = Navigation.Type;

namespace Components.Debug
{
    public class MapComponent : MonoBehaviour
    {
        public bool DebugClipper = true;
        public bool DebugTriangulation = true;
        public bool DebugGrid = true;
        public bool DebugUpdateFrame = true;

        private Map map;

        public void Start()
        {
            NavigationFloorComponent floorObject;
            NavigationStaticObjectComponent[] staticObjects;

            if (!SearchFloorAndStaticObjects(out floorObject, out staticObjects))
                return;

            NavigationPolygons staticObjectsPoly;
            NavigationPolygon floorObjectsPoly;
            ProzessFloorAndStaticObjects(floorObject, staticObjects, out staticObjectsPoly, out floorObjectsPoly);

            map = new Map(floorObjectsPoly, staticObjectsPoly);
        }

        private void ProzessFloorAndStaticObjects(NavigationFloorComponent floorObject,
            NavigationStaticObjectComponent[] staticObjects, out NavigationPolygons staticObjectsPoly, out NavigationPolygon floorObjectsPoly)
        {
            floorObjectsPoly = ToPolygon(floorObject.gameObject, 0);

            staticObjectsPoly = new NavigationPolygons();
            foreach (var staticObj in staticObjects)
            {
                staticObjectsPoly.Add(ToPolygon(staticObj.gameObject, 0));
            }
        }

        private static bool SearchFloorAndStaticObjects(out NavigationFloorComponent floorObject, out NavigationStaticObjectComponent[] staticObjects)
        {
            floorObject = GameObject.FindObjectOfType<NavigationFloorComponent>();
            staticObjects = GameObject.FindObjectsOfType<NavigationStaticObjectComponent>();

            if (floorObject == null)
                return false;

            return true;
        }


        public void Update()
        {
            if (DebugUpdateFrame)
            {
                NavigationFloorComponent floorObject;
                NavigationStaticObjectComponent[] staticObjects;

                if (!SearchFloorAndStaticObjects(out floorObject, out staticObjects))
                    return;

                NavigationPolygons staticObjectsPoly;
                NavigationPolygon floorObjectsPoly;
                ProzessFloorAndStaticObjects(floorObject, staticObjects, out staticObjectsPoly, out floorObjectsPoly);

                map = new Map(floorObjectsPoly, staticObjectsPoly);
            }
        }

        public void OnDrawGizmos()
        {
            if (map == null)
                return;

            if (DebugClipper)
            {
                DrawPolygons(map.FloorWithDynamicObjects, Color.green);
            }

            if (DebugTriangulation)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < map.NavMesh.ElementCount; i++)
                {
                    var v0 = map.NavMesh.Vertices[map.NavMesh.Elements[i * 3]].Position;
                    var v1 = map.NavMesh.Vertices[map.NavMesh.Elements[i * 3 + 1]].Position;
                    var v2 = map.NavMesh.Vertices[map.NavMesh.Elements[i * 3 + 2]].Position;
                    Gizmos.DrawLine(ToVector(v0), ToVector(v1));
                    Gizmos.DrawLine(ToVector(v1), ToVector(v2));
                    Gizmos.DrawLine(ToVector(v2), ToVector(v0));

                }
            }

            if (DebugGrid)
            {
                foreach (var cell in map.Grid.Cells)
                {
                    if (cell.Type == Type.Walkable)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawLine(ToVector(new IntPoint(cell.X, cell.Y)), ToVector(new IntPoint(cell.X + 1, cell.Y)));
                    Gizmos.DrawLine(ToVector(new IntPoint(cell.X, cell.Y)), ToVector(new IntPoint(cell.X, cell.Y + 1)));
                    Gizmos.DrawLine(ToVector(new IntPoint(cell.X + 1, cell.Y + 1)), ToVector(new IntPoint(cell.X + 1, cell.Y)));
                    Gizmos.DrawLine(ToVector(new IntPoint(cell.X + 1, cell.Y + 1)), ToVector(new IntPoint(cell.X, cell.Y + 1)));
                }
            }
        }

        private Vector3 ToVector(Vec3 point)
        {
            return new Vector3(point.X, 0, point.Y);
        }

        private Vector3 ToVector(IntPoint point)
        {
            return new Vector3(point.X, 0, point.Y);
        }

        private static void DrawPolygons(NavigationPolygons polygons, Color color)
        {
            Dictionary<int, List<int>> usedPoints = new Dictionary<int, List<int>>();

            foreach (var polygon in polygons)
            {
                DrawPolygon(polygon, color, usedPoints);
            }
        }

        private static void DrawPolygon(NavigationPolygon region, Color color, Dictionary<int, List<int>> usedPoints)
        {
            Gizmos.color = color;
            var p0 = region[region.Count - 1];
            var p1 = region[0];
            Gizmos.DrawLine(new Vector3(p0.X, 0, p0.Y), new Vector3(p1.X, 0, p1.Y));
            AddPoint(usedPoints, p0);
            for (int i = 1; i < region.Count; i++)
            {
                p0 = region[i - 1];
                p1 = region[i];
                Gizmos.DrawLine(new Vector3(p0.X, 0, p0.Y), new Vector3(p1.X, 0, p1.Y));

                AddPoint(usedPoints, p0);
            }
        }

        private static void AddPoint(Dictionary<int, List<int>> usedPoints, IntPoint p)
        {
            if(!usedPoints.ContainsKey(p.X))
            {
                usedPoints.Add(p.X, new List<int>()
                {
                    p.Y
                });

            }
            else
            {
                if(usedPoints[p.X].Contains(p.Y))
                    // throw new Exception("Found the issue!");

                usedPoints[p.X].Add(p.Y);
            }
        }

        private NavigationPolygon ToPolygon(GameObject go, int funnelSize)
        {
            var points = new NavigationPolygon();

            var mesh = go.GetComponent<BoxCollider>();

            Vector3 p1 = new Vector3(-mesh.size.x, 0, -mesh.size.z) * 0.5f;
            Vector3 p2 = new Vector3(mesh.size.x, 0, -mesh.size.z) * 0.5f;
            Vector3 p3 = new Vector3(mesh.size.x, 0, mesh.size.z) * 0.5f;
            Vector3 p4 = new Vector3(-mesh.size.x, 0, mesh.size.z) * 0.5f;
            p1 = go.transform.TransformPoint(p1);
            p2 = go.transform.TransformPoint(p2);
            p3 = go.transform.TransformPoint(p3);
            p4 = go.transform.TransformPoint(p4);
            points.Add(new IntPoint()
            {
                X = (int)p1.x - funnelSize,
                Y = (int)p1.z - funnelSize
            });
            points.Add(new IntPoint()
            {
                X = (int)p2.x + funnelSize,
                Y = (int)p2.z - funnelSize
            });
            points.Add(new IntPoint()
            {
                X = (int)p3.x + funnelSize,
                Y = (int)p3.z + funnelSize
            });
            points.Add(new IntPoint()
            {
                X = (int)p4.x - funnelSize,
                Y = (int)p4.z + funnelSize
            });

            return points;
        }
    }
}