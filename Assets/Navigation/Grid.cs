using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ClipperLib;
using Poly2Tri;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Navigation
{
    public struct GridFindTrianglesJob : IJobParallelFor
    {
        public NativeArray<GridCell> Cells;

        [ReadOnly]
        public NativeArray<NavigationTriangle> Triangles;

        public void Execute(int i)
        {
            bool isOnNavMesh = false;
            int x = Cells[i].X;
            int y = Cells[i].Y;
            for (int xd = 0; xd <= 1; xd++)
            {
                for (int yd = 0; yd <= 1; yd++)
                {
                    isOnNavMesh = false;
                    for (int ii = 0; ii < Triangles.Length; ii++)
                    {
                        bool isInTriangle = Triangles[ii].IsPointInTriangle(new IntPoint(x + xd, y + yd));
                        if (isInTriangle)
                        {
                            isOnNavMesh = true;
                            break;
                        }
                    }

                    if (!isOnNavMesh)
                    {
                        break;
                    }
                }

                if (!isOnNavMesh)
                {
                    break;
                }
            }

            Cells[i] = new GridCell(x, y)
            {
                Type = isOnNavMesh ? Type.Walkable : Type.Blocked
            };
        }
    }

    public class Grid
    {
        public Dictionary<IntPoint, GridCell> Cells = new Dictionary<IntPoint, GridCell>();

        public void Initialize(NavigationPolygon floor, NavMeshPolygon triangulationPoly)
        {
            var bounding = floor.GetBounding();
            Debug.Log("Bounding Size " + bounding.B.X + ", " + bounding.B.Y);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int x = bounding.A.X; x < bounding.B.X; x++)
            {
                for (int y = bounding.A.Y; y < bounding.B.Y; y++)
                {
                    bool isOnNavMesh = false;

                    for (int xd = 0; xd <= 1; xd++)
                    {
                        for (int yd = 0; yd <= 1; yd++)
                        {
                            isOnNavMesh = false;
                            foreach (var triangle in triangulationPoly.Triangles)
                            {
                                bool isInTriangle = triangle.IsPointInTriangle(new IntPoint(x + xd, y + yd));
                                if (isInTriangle)
                                {
                                    isOnNavMesh = true;
                                    break;
                                }
                            }

                            if (!isOnNavMesh)
                            {
                                break;
                            }
                        }

                        if (!isOnNavMesh)
                        {
                            break;
                        }
                    }

                    Cells.Add(new IntPoint(x, y), new GridCell(x, y)
                    {
                        Type = isOnNavMesh ? Type.Walkable : Type.Blocked
                    });
                }
            }
            sw.Stop();
            Debug.Log("None Job: " + sw.ElapsedTicks + " ticks " + sw.ElapsedMilliseconds + " ms");
            sw.Reset();
            Cells.Clear();
            sw.Start();
            GridFindTrianglesJob job = new GridFindTrianglesJob();
            List<NavigationTriangle> triables = new List<NavigationTriangle>();
            foreach (var tri in triangulationPoly.Triangles)
            {
                triables.Add(tri.ToNavigationTriangle());
            }
            job.Triangles = new NativeArray<NavigationTriangle>(triables.ToArray(), Allocator.Persistent);
            var positions = new List<GridCell>();
            for (int x = bounding.A.X; x < bounding.B.X; x++)
            {
                for (int y = bounding.A.Y; y < bounding.B.Y; y++)
                {
                    positions.Add(new GridCell(x, y));
                }
            }

            job.Cells = new NativeArray<GridCell>(positions.ToArray(), Allocator.Persistent);
            var handle = job.Schedule(positions.Count, 32);
            handle.Complete();

            foreach (var cell in job.Cells)
            {
                Cells.Add(new IntPoint(cell.X, cell.Y), cell);
            }
            job.Triangles.Dispose();
            job.Cells.Dispose();
            sw.Stop();
            Debug.Log("Job: " + sw.ElapsedTicks + " ticks " + sw.ElapsedMilliseconds + " ms");

        }
    }
}
