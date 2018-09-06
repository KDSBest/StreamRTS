using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Poly2Tri;

namespace Navigation
{
    public class Grid
    {
        public Dictionary<IntPoint, GridCell> Cells = new Dictionary<IntPoint, GridCell>();

        public void Initialize(NavigationPolygon floor, NavMeshPolygon triangulationPoly)
        {
            var bounding = floor.GetBounding();

            for (int x = bounding.A.X; x <= bounding.B.X; x++)
            {
                for (int y = bounding.A.Y; y <= bounding.B.Y; y++)
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
                        IsBuildable = isOnNavMesh
                    });
                }
            }
        }
    }
}
