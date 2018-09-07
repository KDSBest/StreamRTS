using System.Collections.Generic;
using ClipperLib;
using Navigation;

namespace Assets.Navigation.AStar
{
    public class AStarContext
    {
        public IntPoint A;
        public IntPoint B;
        public NavigationTriangle ATri;
        public NavigationTriangle BTri;
        public PriorityQueue<AStarNode> NodeQueue = new PriorityQueue<AStarNode>();
        public Dictionary<IntPoint, AStarNode> AllNodes = new Dictionary<IntPoint, AStarNode>();
    }
}