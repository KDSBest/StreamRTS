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
        public bool DebugPath = true;
        public bool DebugMove = false;

        private Map map;
        private List<List<NavigationEdge>> buildings = new List<List<NavigationEdge>>();
        private List<GameObject> buildingGameObjects = new List<GameObject>();

        public GameObject DebugBuilding;

        public GameObject PathA;
        public GameObject PathB;
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
            if (Input.GetMouseButtonUp(0))
            {
                ChangeBuildings(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                ChangeBuildings(false);
            }

            if (DebugMove && path != null)
            {
                int speed = 6;

                var direction = new DeterministicVector2();
                for (int i = 1; i < path.Count; i++)
                {
                    direction = path[i].Position - path[0].Position;
                    direction = direction * speed;
                    var lenSquared = direction.GetLengthSquared();

                    int len = DeterministicInt.Sqrt(lenSquared).IntValue;
                    int moves = len / speed;
                    if (moves != 0)
                    {
                        direction = direction / moves;
                        break;
                    }
                }

                PathA.transform.position += new Vector3(direction.X.ToFloat(), 0, direction.Y.ToFloat()) * Time.deltaTime;
            }
        }

        private void ChangeBuildings(bool isAdd)
        {
            var mainCam = GameObject.FindObjectOfType<Camera>();
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (isAdd)
                {
                    var newBuilding = new List<NavigationEdge>()
                    {
                        new NavigationEdge(new DeterministicVector2((int) hitInfo.point.x, (int) hitInfo.point.z),
                            new DeterministicVector2((int) hitInfo.point.x + 10, (int) hitInfo.point.z + 10))
                    };

                    if (map.AddDynamicObject(newBuilding))
                    {
                        var newBuildingGo = GameObject.Instantiate(DebugBuilding);

                        newBuildingGo.transform.position = new Vector3(
                            (newBuilding[0].A.X + (newBuilding[0].B.X - newBuilding[0].A.X) / 2).ToFloat(), 1,
                            (newBuilding[0].A.Y + (newBuilding[0].B.Y - newBuilding[0].A.Y) / 2).ToFloat());
                        var size = newBuilding[0].GetDirection();
                        newBuildingGo.transform.localScale = new Vector3(size.X.ToFloat(), 2, size.Y.ToFloat());

                        this.buildings.Add(newBuilding);
                        this.buildingGameObjects.Add(newBuildingGo);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Can't Build Here!");
                    }
                }
                else
                {
                    if (buildings.Count > 0)
                    {
                        if (map.RemoveDynamicObject(buildings[0]))
                        {
                            GameObject.Destroy(buildingGameObjects[0]);
                            buildings.RemoveAt(0);
                            buildingGameObjects.RemoveAt(0);
                        }
                    }
                }
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
                for (int i = 0; i < map.NavigationMesh.AllTriangle.Count; i++)
                {
                    Gizmos.DrawLine(ToVector(map.NavigationMesh.AllTriangle[i].U), ToVector(map.NavigationMesh.AllTriangle[i].V));
                    Gizmos.DrawLine(ToVector(map.NavigationMesh.AllTriangle[i].V), ToVector(map.NavigationMesh.AllTriangle[i].W));
                    Gizmos.DrawLine(ToVector(map.NavigationMesh.AllTriangle[i].W), ToVector(map.NavigationMesh.AllTriangle[i].U));

                }
            }

            if (DebugGrid)
            {
                foreach (var cell in map.Grid.Cells)
                {
                    if (cell.Type == GridCellType.Free)
                    {
                        Gizmos.color = Color.green;
                    }
                    else if(cell.Type == GridCellType.Building)
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

            if (DebugPath)
            {
                if (map.Pathfinding != null)
                {
                    var from = new DeterministicVector2((int) PathA.transform.position.x, (int) PathA.transform.position.z);
                    var to = new DeterministicVector2((int) PathB.transform.position.x, (int) PathB.transform.position.z);

                    Gizmos.DrawLine(ToVector(from), ToVector(to));

                    path = map.Pathfinding.CalculatePath(from, to);

                    if (path != null)
                    {
                        Gizmos.color = Color.yellow;
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            Gizmos.DrawLine(ToVector(path[i].Position), ToVector(path[i + 1].Position));
                        }
                    }
                }
            }

        }

        private Vector3 ToVector(Vec3 point)
        {
            return new Vector3(point.X.ToFloat(), 0, point.Y.ToFloat());
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