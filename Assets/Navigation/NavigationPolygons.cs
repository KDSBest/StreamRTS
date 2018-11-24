using System.Collections.Generic;

namespace Navigation
{
    public class NavigationPolygons : List<NavigationPolygon>
    {
        public NavigationPolygons()
        {

        }

        public NavigationPolygons(int cnt) : base(cnt)
        {

        }

        public NavigationPolygons(IEnumerable<NavigationPolygon> polygons)
        {
            this.AddRange(polygons);
        }

        public NavigationPolygons DeepCopy()
        {
            NavigationPolygons copy = new NavigationPolygons(this.Count);

            for (var i = 0; i < this.Count; i++)
            {
                copy.Add(this[i].DeepCopy());
            }

            return copy;
        }

        public List<EdgeIntersectionResult> CalculateMultipleIntersection(NavigationEdge edge, int maximum)
        {
            List<EdgeIntersectionResult> result = new List<EdgeIntersectionResult>();
            foreach (var poly in this)
            {
                if (poly.GetBoundingIntersection(edge))
                {
                    foreach (var constrainedEdge in poly.ConstraintedEdges)
                    {
                        var r = edge.CalculateIntersection(constrainedEdge);
                        if (r.SegmentsIntersect)
                            result.Add(r);

                        if (result.Count > maximum)
                            return result;
                    }
                }
            }

            return result;
        }

        public EdgeIntersectionResult CalculateAnyIntersection(NavigationEdge edge)
        {
            foreach (var poly in this)
            {
                foreach (var constrainedEdge in poly.ConstraintedEdges)
                {
                    var result = edge.CalculateIntersection(constrainedEdge);
                    if (result.SegmentsIntersect)
                        return result;
                }
            }

            return new EdgeIntersectionResult();
        }

        public void CalculateConstrainedEdges()
        {
            foreach (var polygon in this)
            {
                polygon.CalculateConstraintedEdges();
            }
        }
    }
}