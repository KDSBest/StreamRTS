using Navigation.DeterministicMath;

namespace Navigation
{
    public class GridCell
    {
        public DeterministicFloat X;
        public DeterministicFloat Y;
        public GridCellType Type;

        public GridCell(DeterministicFloat x, DeterministicFloat y)
        {
            Type = GridCellType.Blocked;
            X = x;
            Y = y;
        }
    }
}