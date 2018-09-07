using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using LibTessDotNet;
using Navigation;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Navigation.AStar
{
    public class AStar
    {
        private NavigationMesh navMesh;
        private NavigationPolygons constrainedEdgePolygons;

        private Dictionary<NavigationTriangle, Dictionary<NavigationTriangle, List<AStarNode>>> pathCache = new Dictionary<NavigationTriangle, Dictionary<NavigationTriangle, List<AStarNode>>>();

        public AStar(NavigationMesh navMesh, NavigationPolygons constrainedEdgePolygons)
        {
            this.navMesh = navMesh;
            this.constrainedEdgePolygons = constrainedEdgePolygons;
        }

        public List<AStarNode> CalculatePath(IntPoint a, IntPoint b)
        {
            AStarContext context = new AStarContext()
            {
                A = a,
                B = b
            };

            var result = constrainedEdgePolygons.CalculateAnyIntersection(new NavigationEdge(context.A, context.B));
            if (!result.SegmentsIntersect)
            {
                return new List<AStarNode>()
                {
                    new AStarNode(a, b),
                    new AStarNode(b, b),
                };
            }

            List<AStarNode> path = CalculateNaivePath(context);

            if (path == null)
                return null;

            path.Add(new AStarNode(b, b));
            path.Insert(0, new AStarNode(a, b));

            OptimizePath(path);
            return path;
        }

        private void OptimizePath(List<AStarNode> path)
        {
            int currentPosition = 0;
            var edge = new NavigationEdge();
            while (currentPosition < path.Count - 2)
            {
                edge.A = path[currentPosition].Position;
                edge.B = path[currentPosition + 2].Position;

                var result = constrainedEdgePolygons.CalculateAnyIntersection(edge);
                if (result.SegmentsIntersect)
                {
                    currentPosition++;
                }
                else
                {
                    path.RemoveAt(currentPosition + 1);
                }
            }
        }

        public List<AStarNode> CalculateNaivePath(AStarContext context)
        {
            context.ATri = navMesh.SearchTriangleForPoint(context.A);
            context.BTri = navMesh.SearchTriangleForPoint(context.B);

            if (!pathCache.ContainsKey(context.ATri))
                pathCache.Add(context.ATri, new Dictionary<NavigationTriangle, List<AStarNode>>());
            if (!pathCache.ContainsKey(context.BTri))
                pathCache.Add(context.BTri, new Dictionary<NavigationTriangle, List<AStarNode>>());

            if (context.ATri == null || context.BTri == null)
            {
                // TODO: Search nearest on NavMesh just make a ray hit to vs all Constrainted edges
                return null;
            }

            if (context.ATri == context.BTri)
            {
                return new List<AStarNode>()
                {
                };
            }

            if (pathCache[context.ATri].ContainsKey(context.BTri))
            {
                return pathCache[context.ATri][context.BTri].ToArray().ToList();
            }

            AddTriangleToQueue(context.ATri, null, context);

            while (context.NodeQueue.Count > 0)
            {
                var node = context.NodeQueue.GetNext();

                var triangles = this.navMesh.GetTrianglesWithPoint(node.Position);

                if (triangles.Contains(context.BTri))
                {
                    return BacktrackFinishPath(node, context);
                }

                foreach (var triangle in triangles)
                {
                    AddTriangleToQueue(triangle, node, context);
                }
            }

            return null;
        }

        private List<AStarNode> BacktrackFinishPath(AStarNode end, AStarContext context)
        {
            List<AStarNode> result = new List<AStarNode>()
            {
                end
            };

            List<AStarNode> resultReversed = new List<AStarNode>()
            {
                end
            };

            while (result[0].Parent != null)
            {
                result.Insert(0, result[0].Parent);
                resultReversed.Add(result[0].Parent);
            }

            pathCache[context.ATri].Add(context.BTri, result);
            pathCache[context.BTri].Add(context.ATri, resultReversed);

            return result.ToArray().ToList();
        }

        private void AddTriangleToQueue(NavigationTriangle triangle, AStarNode parent, AStarContext context)
        {
            AddPointToQueue(triangle.U, parent, context);
            AddPointToQueue(triangle.V, parent, context);
            AddPointToQueue(triangle.W, parent, context);
        }

        private void AddPointToQueue(IntPoint trianglePoint, AStarNode parent, AStarContext context)
        {
            if (context.AllNodes.ContainsKey(trianglePoint))
            {
                bool readdNeeded = context.NodeQueue.Remove(context.AllNodes[trianglePoint]);

                context.AllNodes[trianglePoint].Parent = parent;
                if (readdNeeded)
                {
                    context.NodeQueue.Add(context.AllNodes[trianglePoint]);
                }
                return;
            }

            var newNode = new AStarNode(trianglePoint, context.B, parent);

            context.AllNodes.Add(trianglePoint, newNode);
            context.NodeQueue.Add(newNode);
        }
    }
}
