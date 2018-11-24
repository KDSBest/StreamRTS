using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using ClipperLib;
using LibTessDotNet;
using Navigation;
using UnityEngine;

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

        public List<AStarNode> CalculatePath(DeterministicVector2 a, DeterministicVector2 b)
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

            var resultPath = new List<AStarNode>(path);
            OptimizePath(resultPath);

            return resultPath;
        }

        private void AddFunnelInformation(List<AStarNode> path)
        {
            foreach (var node in path)
            {
                foreach (var polygon in constrainedEdgePolygons)
                {
                    foreach (var edge in polygon.ConstraintedEdges)
                    {
                        if (node.Position == edge.A)
                        {
                            node.ConstraintedEdgeNormal += edge.A - edge.B;
                        }
                        else if(node.Position == edge.B)
                        {
                            node.ConstraintedEdgeNormal += edge.B - edge.A;
                        }
                    }
                }

                if (node.ConstraintedEdgeNormal.X != 0 || node.ConstraintedEdgeNormal.Y != 0)
                {
                    node.ConstraintedEdgeNormal = node.ConstraintedEdgeNormal.Normalize();
                }
            }
        }

        private void OptimizePath(List<AStarNode> path)
        {
            if (path.Count == 0)
                return;

            int currentPosition = 0;
            var edge = new NavigationEdge();
            while (currentPosition < path.Count - 2)
            {
                edge.A = path[currentPosition].Position;
                edge.B = path[currentPosition + 2].Position;

                var result = constrainedEdgePolygons.CalculateMultipleIntersection(edge, 3);
                if (CanNotOptimize(result))
                {
                    currentPosition++;
                    break;
                }
                else
                {
                    path.RemoveAt(currentPosition + 1);
                }
            }
        }

        private bool CanNotOptimize(List<EdgeIntersectionResult> result)
        {
            if (result.Count == 0)
                return false;

            if (result.Count > 2)
                return true;

            return result[0].SegmentsIntersect && !(result[0].Deltas.X == 1 || result[0].Deltas.Y == 0 || result[0].Deltas.X == 0 || result[0].Deltas.Y == 1);
        }

        public List<AStarNode> CalculateNaivePath(AStarContext context)
        {
            context.ATri = navMesh.SearchTriangleForPoint(context.A);
            context.BTri = navMesh.SearchTriangleForPoint(context.B);

            if (context.ATri == null || context.BTri == null)
            {
                return null;
            }

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
                return new List<AStarNode>(pathCache[context.ATri][context.BTri]);
            }

            AddTriangleToQueue(context.ATri, null, context);

            while (context.NodeQueue.Count > 0)
            {
                var node = context.NodeQueue.GetNext();

                var triangles = this.navMesh.GetTrianglesWithPoint(node.Position);

                if (triangles.Contains(context.BTri))
                {
                    var path = new List<AStarNode>(BacktrackFinishPath(node, context));
                    return path;
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

            while (result[0].Parent != null)
            {
                result.Insert(0, result[0].Parent);
            }

            AddFunnelInformation(result);

            pathCache[context.ATri].Add(context.BTri, new List<AStarNode>(result));
            var reversed = new List<AStarNode>(result);
            reversed.Reverse();
            pathCache[context.BTri].Add(context.ATri, reversed);

            return result;
        }

        private void AddTriangleToQueue(NavigationTriangle triangle, AStarNode parent, AStarContext context)
        {
            AddPointToQueue(triangle.U, parent, context);
            AddPointToQueue(triangle.V, parent, context);
            AddPointToQueue(triangle.W, parent, context);
            AddPointToQueue(triangle.S0.Midpoint(), parent, context);
            AddPointToQueue(triangle.S1.Midpoint(), parent, context);
            AddPointToQueue(triangle.S2.Midpoint(), parent, context);
        }

        private void AddPointToQueue(DeterministicVector2 trianglePoint, AStarNode parent, AStarContext context)
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
