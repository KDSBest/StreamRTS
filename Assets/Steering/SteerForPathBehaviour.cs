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
    public class SteerForPathBehaviour : ISteerBehaviour
    {
        public DeterministicFloat PathSize = new DeterministicFloat(0.05);

        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            if(path == null || path.Count == 0)
                return new DeterministicVector2();

            bool isLastMovementPossible = unit.LastSteering.GetLengthSquared() != 0;
            DeterministicVector2 naiveNewPosition = unit.Position;
            DeterministicVector2 lastSteering = unit.LastSteering;
            if (isLastMovementPossible)
            {
                lastSteering = lastSteering.Normalize();
                naiveNewPosition += lastSteering;
            }

            DeterministicFloat shortestDistance = new DeterministicFloat(long.MaxValue, false);

            NavigationEdge nearestEdge = new NavigationEdge();
            DeterministicVector2 closestPoint = new DeterministicVector2();

            for (var i = 0; i < path.Count; i++)
            {
                var pathEdge = path[i];
                var distanceResult = pathEdge.GetDistance(naiveNewPosition);
                if (distanceResult.Distance < shortestDistance)
                {
                    shortestDistance = distanceResult.Distance;
                    nearestEdge = pathEdge;
                    closestPoint = distanceResult.ClosestPoint;

                    if (i > 0)
                    {
                        unit.RecalcPathOnNextUpdate = true;
                    }

                    // naive positioning is on path, we don't need to steer
                    if (isLastMovementPossible && shortestDistance <= PathSize)
                    {
                        return lastSteering;
                    }
                }
            }

            var dir = nearestEdge.B - nearestEdge.A;
            return (closestPoint + dir.Normalize() - unit.Position);
        }
    }
}
