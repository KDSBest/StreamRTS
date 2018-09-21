using Navigation.DeterministicMath;

namespace Navigation
{
    public struct DeterministicVector3
    {
        public DeterministicFloat X;
        public DeterministicFloat Y;
        public DeterministicFloat Z;

        public DeterministicVector3(DeterministicFloat x, DeterministicFloat y, DeterministicFloat z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public DeterministicVector2 ToVector2XZ()
        {
            return new DeterministicVector2(X, Z);
        }
    }
}