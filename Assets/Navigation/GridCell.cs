namespace Navigation
{
    public class GridCell
    {
        public int X;
        public int Y;
        public bool IsBuildable = true;

        public GridCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}