using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Navigation.DeterministicMath;

namespace Navigation
{
    public class NavigationPolygon : List<DeterministicVector2>
    {
        public List<NavigationEdge> ConstraintedEdges = new List<NavigationEdge>();

        public NavigationPolygon()
        {

        }

        public NavigationPolygon(int cnt) : base(cnt)
        {

        }

        public void CalculateConstraintedEdges()
        {
            ConstraintedEdges.Clear();
            if (this.Count < 2)
                return;

            var p0 = this[this.Count - 1];
            var p1 = this[0];
            ConstraintedEdges.Add(new NavigationEdge(p0, p1));
            for (int i = 1; i < this.Count; i++)
            {
                p0 = this[i - 1];
                p1 = this[i];
                ConstraintedEdges.Add(new NavigationEdge(p0, p1));
            }
        }

        public NavigationEdge GetBounding()
        {
            var min = new DeterministicVector2(int.MaxValue, int.MaxValue);
            var max = new DeterministicVector2(int.MinValue, int.MinValue);

            foreach (var p in this)
            {
                min.X = DeterministicFloat.Min(p.X, min.X);
                min.Y = DeterministicFloat.Min(p.Y, min.Y);
                max.X = DeterministicFloat.Max(p.X, max.X);
                max.Y = DeterministicFloat.Max(p.Y, max.Y);
            }

            return new NavigationEdge(min, max);
        }

        public NavigationPolygon DeepCopy()
        {
            NavigationPolygon copy = new NavigationPolygon(this.Count);

            for (var i = 0; i < this.Count; i++)
            {
                copy.Add(this[i].DeepCopy());
            }

            return copy;
        }
    }
}
