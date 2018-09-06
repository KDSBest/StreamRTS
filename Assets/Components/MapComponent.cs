using System.Collections.Generic;
using ClipperLib;
using Navigation;
using Poly2Tri;
using UnityEngine;

namespace Components.Debug
{
    public class MapComponent : MonoBehaviour
    {
        public bool DebugClipper = true;
        public bool DebugTriangulation = true;
        public bool DebugGrid = true;

        // TODO: Let Map generate all funnel sizes needed!
        public int FunnelSize = 0;

        private Map map;

        public void Start()
        {
            var floorObject = GameObject.FindObjectOfType<NavigationFloorComponent>();
            if (floorObject == null)
                return;

            var staticObjects = GameObject.FindObjectsOfType<NavigationStaticObjectComponent>();

            var floorObjectsPoly = ToPolygon(floorObject.gameObject, 0);

            var staticObjectsPoly = new NavigationPolygons();
            foreach (var staticObj in staticObjects)
            {
                staticObjectsPoly.Add(ToPolygon(staticObj.gameObject, FunnelSize));
            }


            map = new Map(floorObjectsPoly, staticObjectsPoly);
        }


        public void OnDrawGizmos()
        {
            if (map == null)
                return;

            if (DebugClipper)
                DrawPolygons(map.FloorWithDynamicObjects, Color.green);

            if (DebugTriangulation)
            {
                Gizmos.color = Color.magenta;
                foreach (var triangle in map.NavMesh.Triangles)
                {
                    Gizmos.DrawLine(ToVector(triangle.Points[0]), ToVector(triangle.Points[1]));
                    Gizmos.DrawLine(ToVector(triangle.Points[1]), ToVector(triangle.Points[2]));
                    Gizmos.DrawLine(ToVector(triangle.Points[2]), ToVector(triangle.Points[0]));
                }
            }

            if (DebugGrid)
            {
                foreach (var cell in map.Grid.Cells.Values)
                {
                    if (cell.IsBuildable)
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


        private Vector3 ToVector(IntPoint point)
        {
            return new Vector3(point.X, 0, point.Y);
        }

        private Vector3 ToVector(TriangulationPoint point)
        {
            return new Vector3(point.X, 0, point.Y);
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
            Gizmos.DrawLine(new Vector3(p0.X, 0, p0.Y), new Vector3(p1.X, 0, p1.Y));

            for (int i = 1; i < region.Count; i++)
            {
                p0 = region[i - 1];
                p1 = region[i];
                Gizmos.DrawLine(new Vector3(p0.X, 0, p0.Y), new Vector3(p1.X, 0, p1.Y));
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