using Navigation.DeterministicMath;

namespace Navigation
{
    public struct NavigationEdgeDistanceResult
    {
        public DeterministicFloat Distance;

        public DeterministicVector2 ClosestPoint;

        public bool IsOnLine;
    }
}