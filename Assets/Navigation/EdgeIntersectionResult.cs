using ClipperLib;

namespace Navigation
{
    public class EdgeIntersectionResult
    {
        public bool LinesIntersect = false;
        public bool SegmentsIntersect = false;

        public IntPoint IntersectionPoint;
        public NavigationEdge SegmentIntersection;
    }
}