namespace Navigation
{
    public class GridCell
    {
        public int X;
        public int Y;
        public GridCellType Type;

        public GridCell(int x, int y)
        {
            Type = GridCellType.Blocked;
            X = x;
            Y = y;
        }
    }
}