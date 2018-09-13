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
    public class SteeUnitCollisionAvoidanceBehaviour : ISteerBehaviour
    {
        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            if(neighbors.Count == 0)
                return new DeterministicVector2();
            var smallestDistanceFound = new DeterministicFloat(long.MaxValue, false);
            var myNewPosition = unit.Position + unit.LastSteering;
            DeterministicVector2 toMove = new DeterministicVector2();

            foreach (var neighbor in neighbors)
            {
                // we ignore Units that go the same direction like we do
                if(unit.LastSteering.DotProduct(neighbor.LastSteering) > new DeterministicFloat(0))
                    continue;

                var neighborNewPosition = neighbor.Position + neighbor.LastSteering;

                var distance = (myNewPosition - neighborNewPosition).GetLength();
                var fullFunnelSize = unit.FunnelSize + neighbor.FunnelSize;
                if (distance >= fullFunnelSize)
                    continue;

                if(smallestDistanceFound < distance)
                    continue;

                smallestDistanceFound = distance;
                var meToNeighbour = neighborNewPosition - myNewPosition;

                var steeringPerp = unit.LastSteering.PerpendicularClockwise();
                var dotMeToNeighbourAndAvade = steeringPerp.DotProduct(meToNeighbour);

                if (dotMeToNeighbourAndAvade < 0)
                {
                    steeringPerp *= -1;
                }

                var toMoveDistance = fullFunnelSize * 2 - distance;
                toMove = steeringPerp.Normalize() * toMoveDistance;
            }

            return toMove;
        }
    }
}
