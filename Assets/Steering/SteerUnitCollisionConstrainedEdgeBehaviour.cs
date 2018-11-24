using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Navigation.AStar;
using Gameplay;
using Navigation;
using Navigation.DeterministicMath;

namespace Steering
{
    public class SteerUnitCollisionConstrainedEdgeBehaviour : ISteerBehaviour
    {
        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            DeterministicVector2 result = new DeterministicVector2();

            foreach (NavigationPolygon poly in map.FloorWithDynamicObjects)
            {
                var aabbDist = poly.GetBoundingDistance(unit.Position);
                if (aabbDist > unit.FunnelSize)
                {
                    continue;
                }

                foreach (var edge in poly.ConstraintedEdges)
                {
                    var distanceResult = edge.GetDistance(unit.Position);
                    if (distanceResult.Distance < unit.FunnelSize)
                    {
                        var direction = unit.Position - distanceResult.ClosestPoint;
                        var toMove = unit.FunnelSize - distanceResult.Distance;
                        result += direction.Normalize() * toMove;
                    }
                }
            }

            return result;
        }
    }
}
