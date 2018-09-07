using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Navigation;

namespace Assets.Navigation.AStar
{
    public class AStarNode : IPriorityQueueEntry
    {
        public IntPoint Position;
        private IntPoint destination;

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

                int newG = parent.G + (this.Position - parent.Position).ManhattanHeuristic();
                if (newG < this.G)
                {
                    parent = value;
                    this.G = newG;
                }
            }
        }

        // Distance Start to Current
        public int G = 0;
        private AStarNode parent;

        // Heuristic to Destination
        public int H
        {
            get
            {
                var distance = this.destination - Position;

                return distance.ManhattanHeuristic();
            }
        }

        public AStarNode(IntPoint position, IntPoint destination, AStarNode parent = null)
        {
            this.Position = position;
            this.destination = destination;

            // Using the setter of Parent makes an endless loop!
            this.parent = parent;
            if (this.parent != null)
            {
                this.G = parent.G + (this.Position - parent.Position).ManhattanHeuristic();
            }
        }

        public int GetCost()
        {
            return G + H;
        }
    }
}
