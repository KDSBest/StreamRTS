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
    public class SteerAlignmentBehaviour : ISteerBehaviour
    {
        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            if (neighbors.Count == 0)
                return new DeterministicVector2();

            DeterministicVector2 toMove = new DeterministicVector2();

            foreach (var neighbor in neighbors)
            {
                toMove += neighbor.LastSteering;
            }

            toMove /= neighbors.Count;
            return toMove;
        }

    }
}
