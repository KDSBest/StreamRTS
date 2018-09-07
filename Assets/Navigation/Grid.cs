using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Poly2Tri;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Navigation
{
    public class Grid
    {
        public List<GridCell> Cells = new List<GridCell>();
        public NavigationEdge Bounding;

        public void Initialize(NavigationPolygon floor, NavMeshPolygon triangulationPoly)
        {
            Bounding = floor.GetBounding();

            Stopwatch sw = new Stopwatch();

            Cells.Clear();

            sw.Start();
            GridFindTrianglesJob job = new GridFindTrianglesJob();

            int jobCount = (Bounding.B.X - Bounding.A.X + 1) * (Bounding.B.Y - Bounding.A.Y + 1);
            int cellCount = (Bounding.B.X - Bounding.A.X + 1) * (Bounding.B.Y - Bounding.A.Y + 1);
            var triangles = new NavigationTriangle[triangulationPoly.Triangles.Count];
            for(int i = 0; i < triangulationPoly.Triangles.Count; i++)
            {
                triangles[i] = triangulationPoly.Triangles[i].ToNavigationTriangle();
            }

            job.StartX = Bounding.A.X;
            job.StartY = Bounding.A.Y;
            job.MaxX = (Bounding.B.X - Bounding.A.X + 1);
            job.Triangles = new NativeArray<NavigationTriangle>(triangles, Allocator.TempJob);
            job.TriangleIndexes = new NativeArray<int>(jobCount, Allocator.Persistent);
            var handle = job.Schedule(jobCount, 32);
            handle.Complete();

            for (int x = 0; x < Bounding.B.X - Bounding.A.X; x++)
            {
                for (int y = 0; y < Bounding.B.Y - Bounding.A.Y; y++)
                {
                    int index = y * job.MaxX + x;
                    int index1 = (y+ 1) * job.MaxX + x;

                    int triIdx = job.TriangleIndexes[index];
                    int triIdx1 = job.TriangleIndexes[index + 1];
                    int triIdx2 = job.TriangleIndexes[index1];
                    int triIdx3 = job.TriangleIndexes[index1 + 1];

                    Cells.Add(new GridCell(x + job.StartY, y + job.StartY)
                    {
                        Type = triIdx == -1 || triIdx1 == -1 || triIdx2 == -1 || triIdx3 == -1 ? Type.Blocked : Type.Walkable,
                        TriangleIndex = triIdx
                    });
                }
            }
            job.Triangles.Dispose();
            job.TriangleIndexes.Dispose();
            sw.Stop();
            Debug.Log("Job: " + sw.ElapsedTicks + " ticks " + sw.ElapsedMilliseconds + " ms Triangles: " + triangulationPoly.Triangles.Count + " GridSize: " + (Bounding.B.X - Bounding.A.X) * (Bounding.B.Y - Bounding.A.Y));

        }
    }
}
