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
    public class SteerCoherenceBehaviour : ISteerBehaviour
    {
        public DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map)
        {
            if (neighbors.Count == 0)
                return new DeterministicVector2();

            DeterministicVector2 position = unit.Position;

            foreach (var neighbor in neighbors)
            {
                position += neighbor.Position;
            }

            position /= (neighbors.Count + 1);

            return position - unit.Position;
        }
    }
}
