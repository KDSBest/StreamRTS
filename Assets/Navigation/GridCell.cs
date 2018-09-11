using Navigation.DeterministicMath;

namespace Navigation
{
    public class GridCell
    {
        public DeterministicInt X;
        public DeterministicInt Y;
        public GridCellType Type;

        public GridCell(DeterministicInt x, DeterministicInt y)
        {
            Type = GridCellType.Blocked;
            X = x;
            Y = y;
        }
    }
}