using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LibTessDotNet;
using Navigation.DeterministicMath;
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

        public void Initialize(NavigationPolygon floor)
        {
            Bounding = floor.GetBounding();

            Cells.Clear();

            for (DeterministicInt y = Bounding.A.Y; y < Bounding.B.Y; y++)
            {
                for (DeterministicInt x = Bounding.A.X; x < Bounding.B.X; x++)
                {
                    Cells.Add(new GridCell(x, y)
                    {
                        Type = GridCellType.Free
                    });
                }
            }
        }

        public void PlaceStaticObjects(List<NavigationEdge> polygons)
        {
            var maxX = Bounding.B.X - Bounding.A.X;

            foreach (var polygon in polygons)
            {
                var polyStartX = DeterministicInt.Max(polygon.A.X, Bounding.A.X) - Bounding.A.X;
                var polyStartY = DeterministicInt.Max(polygon.A.Y, Bounding.A.Y) - Bounding.A.Y;
                var polyLenX = DeterministicInt.Min(polygon.B.X, Bounding.B.X) - Bounding.A.X;
                var polyLenY = DeterministicInt.Min(polygon.B.Y, Bounding.B.Y) - Bounding.A.Y;

                for (DeterministicInt x = polyStartX; x < polyLenX; x++)
                {
                    for (DeterministicInt y = polyStartY; y < polyLenY; y++)
                    {
                        Cells[(x + y * maxX).ToInt()].Type = GridCellType.Blocked;
                    }
                }
            }
        }

        public bool PlaceDynamicObjects(List<NavigationEdge> polygons)
        {
            return SwitchCellType(polygons, GridCellType.Free, GridCellType.Building);
        }

        public bool RemoveDynamicObjects(List<NavigationEdge> polygons)
        {
            return SwitchCellType(polygons, GridCellType.Building, GridCellType.Free);
        }

        private bool SwitchCellType(List<NavigationEdge> polygons, GridCellType expectedType, GridCellType resultType)
        {
            var maxX = Bounding.B.X - Bounding.A.X;

            foreach (var polygon in polygons)
            {
                var polyStartX = DeterministicInt.Max(polygon.A.X, Bounding.A.X) - Bounding.A.X;
                var polyStartY = DeterministicInt.Max(polygon.A.Y, Bounding.A.Y) - Bounding.A.Y;
                var polyLenX = DeterministicInt.Min(polygon.B.X, Bounding.B.X) - Bounding.A.X;
                var polyLenY = DeterministicInt.Min(polygon.B.Y, Bounding.B.Y) - Bounding.A.Y;

                for (DeterministicInt x = polyStartX; x < polyLenX; x++)
                {
                    for (DeterministicInt y = polyStartY; y < polyLenY; y++)
                    {
                        if (Cells[(x + y * maxX).ToInt()].Type != expectedType)
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (var polygon in polygons)
            {
                var polyStartX = DeterministicInt.Max(polygon.A.X, Bounding.A.X) - Bounding.A.X;
                var polyStartY = DeterministicInt.Max(polygon.A.Y, Bounding.A.Y) - Bounding.A.Y;
                var polyLenX = DeterministicInt.Min(polygon.B.X, Bounding.B.X) - Bounding.A.X;
                var polyLenY = DeterministicInt.Min(polygon.B.Y, Bounding.B.Y) - Bounding.A.Y;

                for (DeterministicInt x = polyStartX; x < polyLenX; x++)
                {
                    for (DeterministicInt y = polyStartY; y < polyLenY; y++)
                    {
                        Cells[(x + y * maxX).ToInt()].Type = resultType;
                    }
                }
            }

            return true;
        }
    }
}
