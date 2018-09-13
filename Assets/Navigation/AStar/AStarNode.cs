using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Navigation;
using Navigation.DeterministicMath;

namespace Assets.Navigation.AStar
{
    public class AStarNode : IPriorityQueueEntry
    {
        public DeterministicVector2 Position;
        private DeterministicVector2 destination;
        public DeterministicVector2 ConstraintedEdgeNormal;


        public AStarNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                // we don't add the start as parent and we don't overwrite the start parent
                // it gets added via constructor
                if (value == null || parent == null)
                    return;

                DeterministicFloat newG = parent.G + (this.Position - parent.Position).ManhattanHeuristic();
                if (newG < this.G)
                {
                    parent = value;
                    this.G = newG;
                }
            }
        }

        // Distance Start to Current
        public DeterministicFloat G = new DeterministicFloat(0, true);
        private AStarNode parent;

        // Heuristic to Destination
        public DeterministicFloat H
        {
            get
            {
                var distance = this.destination - Position;

                return distance.ManhattanHeuristic();
            }
        }

        public AStarNode(DeterministicVector2 position, DeterministicVector2 destination, AStarNode parent = null)
        {
            this.Position = position;
            this.destination = destination;

            // Using the setter of Parent makes an endless loop!
            this.parent = parent;
            if (this.parent != null)
            {
                this.G = parent.G + (this.Position - parent.Position).ManhattanHeuristic();
            }

            ConstraintedEdgeNormal = new DeterministicVector2(0, 0);
        }

        public DeterministicFloat GetCost()
        {
            return G + H;
        }
    }
}
