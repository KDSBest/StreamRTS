using ClipperLib;

namespace Navigation
{
    public class EdgeIntersectionResult
    {
        public bool LinesIntersect = false;
        public bool SegmentsIntersect = false;

        // public DeterministicVector2 IntersectionPoint;
        // public NavigationEdge SegmentIntersection;
        public DeterministicVector2 Deltas;
    }
}