using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Navigation.Clipper;
using Poly2Tri;
using UnityEngine;
using Polygon = Navigation.Clipper.Polygon;

public class GizmoShowClipper : MonoBehaviour
{
    public GameObject[] Meshes;

    public bool DebugPolygonStartPoly = false;
    public bool DebugDiffPolygons = false;
    public bool DebugPolygonResult = false;
    public bool DebugTriangulation = false;
    public bool DebugTriangulation2 = true;
    public int FunnelSize = 0;
    public void OnDrawGizmos()
    {
        if (Meshes.Length < 1)
            return;

        var polygonMain = ToPolygon(Meshes[0], 0);

        if (DebugPolygonStartPoly)
            DrawPolygon(polygonMain, Color.green);

        PolygonClipper clipper = new PolygonClipper();

        Polygon difPoly = null;
        if (Meshes.Length > 1)
        {
            difPoly = ToPolygon(Meshes[1], FunnelSize);

            for (int i = 2; i < Meshes.Length; i++)
            {
                var difPoly2 = ToPolygon(Meshes[i], FunnelSize);
                difPoly = clipper.union(difPoly, difPoly2);
            }
        }

        if (difPoly != null)
        {
            if (DebugDiffPolygons)
                DrawPolygon(difPoly, Color.red);

            polygonMain = clipper.difference(polygonMain, difPoly);
        }

        if (DebugPolygonResult)
            DrawPolygon(polygonMain, Color.blue);

        List<TriangulationPoint> points = new List<TriangulationPoint>();
        List<int> edges = new List<int>();
        foreach (var region in polygonMain.regions)
        {
            if (region.Count < 1)
                continue;

            points.Add(new TriangulationPoint(region[0].x, region[0].y));
            int firstPointIndex = points.Count - 1;
            for (int i = 1; i < region.Count; i++)
            {
                points.Add(new TriangulationPoint(region[i].x, region[i].y));
                edges.Add(points.Count - 2);
                edges.Add(points.Count - 1);
            }

            if (region.Count > 1)
            {
                edges.Add(points.Count - 1);
                edges.Add(firstPointIndex);
            }
        }

        ConstrainedPointSet pointSet = new ConstrainedPointSet(points, edges.ToArray());
        P2T.Triangulate(pointSet);

        if (DebugTriangulation)
        {
            Gizmos.color = Color.cyan;
            foreach (var triangle in pointSet.Triangles)
            {
                Gizmos.DrawLine(ToVector(triangle.Points[0]), ToVector(triangle.Points[1]));
                Gizmos.DrawLine(ToVector(triangle.Points[1]), ToVector(triangle.Points[2]));
                Gizmos.DrawLine(ToVector(triangle.Points[2]), ToVector(triangle.Points[0]));
            }
        }

        var polygons = new List<Poly2Tri.Polygon>();

        foreach (var region in polygonMain.regions)
        {
            var polygonPoints = new List<PolygonPoint>();
            foreach (var p in region)
            {
                polygonPoints.Add(new PolygonPoint(p.x, p.y));
            }
            polygons.Add(new Poly2Tri.Polygon(polygonPoints));
        }

        var triangulationPoly = polygons[polygons.Count - 1];
        for (int i = 0; i < polygons.Count - 1; i++)
        {
            triangulationPoly.AddHole(polygons[i]);
        }

        P2T.Triangulate(triangulationPoly);
        Gizmos.color = Color.cyan;
        if (DebugTriangulation2)
        {
            foreach (var triangle in triangulationPoly.Triangles)
            {
                Gizmos.DrawLine(ToVector(triangle.Points[0]), ToVector(triangle.Points[1]));
                Gizmos.DrawLine(ToVector(triangle.Points[1]), ToVector(triangle.Points[2]));
                Gizmos.DrawLine(ToVector(triangle.Points[2]), ToVector(triangle.Points[0]));
            }
        }
    }

    private Vector3 ToVector(TriangulationPoint point)
    {
        return new Vector3(point.Xf, 0, point.Yf);
    }

    private static void DrawPolygon(Polygon polygonMain, Color color)
    {
        Gizmos.color = color;
        foreach (var region in polygonMain.regions)
        {
            if (region.Count < 2)
                continue;

            var p0 = region[region.Count - 1];
            var p1 = region[0];
            Gizmos.DrawLine(new Vector3(p0.x, 0, p0.y), new Vector3(p1.x, 0, p1.y));

            for (int i = 1; i < region.Count; i++)
            {
                p0 = region[i - 1];
                p1 = region[i];
                Gizmos.DrawLine(new Vector3(p0.x, 0, p0.y), new Vector3(p1.x, 0, p1.y));
            }
        }
    }

    private Polygon ToPolygon(GameObject go, int funnelSize)
    {
        Polygon ret = new Polygon();
        PointList points = new PointList();

        var mesh = go.GetComponent<BoxCollider>();

        Vector3 p1 = new Vector3(-mesh.size.x, 0, -mesh.size.z) * 0.5f;
        Vector3 p2 = new Vector3(mesh.size.x, 0, -mesh.size.z) * 0.5f;
        Vector3 p3 = new Vector3(mesh.size.x, 0, mesh.size.z) * 0.5f;
        Vector3 p4 = new Vector3(-mesh.size.x, 0, mesh.size.z) * 0.5f;
        p1 = go.transform.TransformPoint(p1);
        p2 = go.transform.TransformPoint(p2);
        p3 = go.transform.TransformPoint(p3);
        p4 = go.transform.TransformPoint(p4);
        points.Add(new Point()
        {
            x = (int)p1.x - funnelSize,
            y = (int)p1.z - funnelSize
        });
        points.Add(new Point()
        {
            x = (int)p2.x + funnelSize,
            y = (int)p2.z - funnelSize
        });
        points.Add(new Point()
        {
            x = (int)p3.x + funnelSize,
            y = (int)p3.z + funnelSize
        });
        points.Add(new Point()
        {
            x = (int)p4.x - funnelSize,
            y = (int)p4.z + funnelSize
        });


        ret.regions = new List<PointList>()
        {
            points
        };

        return ret;
    }
}
