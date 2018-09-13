using System.Collections.Generic;
using Assets.Navigation.AStar;
using Gameplay;
using Navigation;

namespace Steering
{
    public interface ISteerBehaviour
    {
        DeterministicVector2 Steer(Unit unit, List<Unit> neighbors, List<NavigationEdge> path, Map map);
    }
}