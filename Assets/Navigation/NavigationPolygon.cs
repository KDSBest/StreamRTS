using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Navigation.DeterministicMath;
using UnityEngine;

namespace Navigation
{
    public class NavigationPolygon : List<DeterministicVector2>
    {
        public List<NavigationEdge> ConstraintedEdges = new List<NavigationEdge>();
        private NavigationEdge cachedBoundingBox = null;

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

            cachedBoundingBox = null;
        }

        public NavigationEdge GetBounding()
        {
            if (cachedBoundingBox == null)
            {
                DeterministicVector2 min = new DeterministicVector2(0, 0);
                DeterministicVector2 max = new DeterministicVector2(0, 0);

                if (this.Count > 0)
                {
                    var p = this[0];
                    min.X = p.X;
                    max.X = p.X;
                    min.Y = p.Y;
                    max.Y = p.Y;
                }

                for (var i = 1; i < this.Count; i++)
                {
                    var p = this[i];
                    min.X = DeterministicFloat.Min(p.X, min.X);
                    min.Y = DeterministicFloat.Min(p.Y, min.Y);
                    max.X = DeterministicFloat.Max(p.X, max.X);
                    max.Y = DeterministicFloat.Max(p.Y, max.Y);
                }

                cachedBoundingBox = new NavigationEdge(min, max);
            }

            return cachedBoundingBox;
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

        public bool GetBoundingIntersection(NavigationEdge edge)
        {
            if (GetBoundingDistance(edge.A) == 0)
                return true;

            if (GetBoundingDistance(edge.B) == 0)
                return true;

            var bounding = this.GetBounding();
            var otherBounding = new NavigationEdge(new DeterministicVector2(bounding.A.X, bounding.B.Y), new DeterministicVector2(bounding.B.X, bounding.A.Y));
            if (edge.CalculateIntersection(new NavigationEdge(bounding.A, otherBounding.B)).SegmentsIntersect)
                return true;

            if (edge.CalculateIntersection(new NavigationEdge(otherBounding.B, bounding.B)).SegmentsIntersect)
                return true;

            if (edge.CalculateIntersection(new NavigationEdge(bounding.B, otherBounding.A)).SegmentsIntersect)
                return true;

            if (edge.CalculateIntersection(new NavigationEdge(otherBounding.A, bounding.A)).SegmentsIntersect)
                return true;

            return false;
        }

        public DeterministicFloat GetBoundingDistance(DeterministicVector2 unitPosition)
        {
            var aabb = GetBounding();
            DeterministicFloat wHalf = (aabb.B.X - aabb.A.X) / 2;
            DeterministicFloat hHalf = (aabb.B.Y - aabb.A.Y) / 2;
            DeterministicFloat dx = DeterministicFloat.Max(DeterministicFloat.Abs(unitPosition.X - (aabb.A.X + wHalf)) - wHalf, 0);
            DeterministicFloat dy = DeterministicFloat.Max(DeterministicFloat.Abs(unitPosition.Y - (aabb.A.Y + hHalf)) - hHalf, 0);

            return DeterministicFloat.Sqrt(dx * dx + dy * dy);
        }
    }
}
