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
    public class SteerUnitCollisionResolveBehaviour : ISteerBehaviour
    {
        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            if(neighbors.Count == 0)
                return new DeterministicVector2();

            DeterministicVector2 toMove = new DeterministicVector2();

            foreach (var neighbor in neighbors)
            {
                var direction = unit.Position - neighbor.Position;
                var neededDistance = unit.FunnelSize + neighbor.FunnelSize;

                var distance = direction.GetLength();

                if (distance >= neededDistance)
                    continue;

                var toMoveDistance = (neededDistance - distance) / 4;

                toMove += direction.Normalize() * toMoveDistance;
            }

            return toMove;
        }
    }
}
