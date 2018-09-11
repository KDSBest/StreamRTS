using System.Collections.Generic;
using ClipperLib;
using Navigation;

namespace Assets.Navigation.AStar
{
    public class AStarContext
    {
        public DeterministicVector2 A;
        public DeterministicVector2 B;
        public NavigationTriangle ATri;
        public NavigationTriangle BTri;
        public PriorityQueue<AStarNode> NodeQueue = new PriorityQueue<AStarNode>();
        public Dictionary<DeterministicVector2, AStarNode> AllNodes = new Dictionary<DeterministicVector2, AStarNode>();
    }
}