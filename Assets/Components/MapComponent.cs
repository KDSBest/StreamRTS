using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Assets.Navigation.AStar;
using ClipperLib;
using LibTessDotNet;
using Navigation;
using Navigation.DeterministicMath;
using UnityEditor;
using UnityEngine;

namespace Components.Debug
{
    public class MapComponent : MonoBehaviour
    {
        public bool DebugClipper = true;
        public bool DebugTriangulation = true;
        public bool DebugGrid = true;

        public Map Map;

        private List<AStarNode> path;

        public void Start()
        {
            NavigationFloorComponent floorObject;
            NavigationStaticObjectComponent[] staticObjects;

            if (!SearchFloorAndStaticObjects(out floorObject, out staticObjects))
                return;

            NavigationPolygons staticObjectsPoly;
            NavigationPolygon floorObjectsPoly;
            ProzessFloorAndStaticObjects(floorObject, staticObjects, out staticObjectsPoly, out floorObjectsPoly);

            Map = new Map(floorObjectsPoly, staticObjectsPoly);
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

        public void OnDrawGizmos()
        {
            if (Map == null)
                return;

            if (DebugClipper)
            {
                DrawPolygons(Map.FloorWithDynamicObjects, Color.green);
            }

            if (DebugTriangulation)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < Map.NavigationMesh.AllTriangle.Count; i++)
                {
                    Gizmos.DrawLine(ToVector(Map.NavigationMesh.AllTriangle[i].U), ToVector(Map.NavigationMesh.AllTriangle[i].V));
                    Gizmos.DrawLine(ToVector(Map.NavigationMesh.AllTriangle[i].V), ToVector(Map.NavigationMesh.AllTriangle[i].W));
                    Gizmos.DrawLine(ToVector(Map.NavigationMesh.AllTriangle[i].W), ToVector(Map.NavigationMesh.AllTriangle[i].U));

                }
            }

            if (DebugGrid)
            {
                foreach (var cell in Map.Grid.Cells)
                {
                    if (cell.Type == GridCellType.Free)
                    {
                        Gizmos.color = Color.green;
                    }
                    else if (cell.Type == GridCellType.Building)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawLine(ToVector(new DeterministicVector2(cell.X, cell.Y)), ToVector(new DeterministicVector2(cell.X + 1, cell.Y)));
                    Gizmos.DrawLine(ToVector(new DeterministicVector2(cell.X, cell.Y)), ToVector(new DeterministicVector2(cell.X, cell.Y + 1)));
                    Gizmos.DrawLine(ToVector(new DeterministicVector2(cell.X + 1, cell.Y + 1)), ToVector(new DeterministicVector2(cell.X + 1, cell.Y)));
                    Gizmos.DrawLine(ToVector(new DeterministicVector2(cell.X + 1, cell.Y + 1)), ToVector(new DeterministicVector2(cell.X, cell.Y + 1)));
                }
            }
        }

        private Vector3 ToVector(DeterministicVector2 point)
        {
            return new Vector3(point.X.ToFloat(), 0, point.Y.ToFloat());
        }

        private static void DrawPolygons(NavigationPolygons polygons, Color color)
        {
            foreach (var polygon in polygons)
            {
                DrawPolygon(polygon, color);
            }
        }

        private static void DrawPolygon(NavigationPolygon region, Color color)
        {
            Gizmos.color = color;
            var p0 = region[region.Count - 1];
            var p1 = region[0];
            Gizmos.DrawLine(new Vector3(p0.X.ToFloat(), 0, p0.Y.ToFloat()), new Vector3(p1.X.ToFloat(), 0, p1.Y.ToFloat()));
            for (int i = 1; i < region.Count; i++)
            {
                p0 = region[i - 1];
                p1 = region[i];
                Gizmos.DrawLine(new Vector3(p0.X.ToFloat(), 0, p0.Y.ToFloat()), new Vector3(p1.X.ToFloat(), 0, p1.Y.ToFloat()));
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
            points.Add(new DeterministicVector2()
            {
                X = (int)p1.x - funnelSize,
                Y = (int)p1.z - funnelSize
            });
            points.Add(new DeterministicVector2()
            {
                X = (int)p2.x + funnelSize,
                Y = (int)p2.z - funnelSize
            });
            points.Add(new DeterministicVector2()
            {
                X = (int)p3.x + funnelSize,
                Y = (int)p3.z + funnelSize
            });
            points.Add(new DeterministicVector2()
            {
                X = (int)p4.x - funnelSize,
                Y = (int)p4.z + funnelSize
            });

            return points;
        }
    }
}