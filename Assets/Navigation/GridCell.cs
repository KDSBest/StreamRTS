namespace Navigation
{
    public enum Type
    {
        Blocked,
        Walkable
    }
    public struct GridCell
    {
        public int X;
        public int Y;
        public Type Type;
        public int TriangleIndex;

        public GridCell(int x, int y)
        {
            Type = Type.Blocked;
            X = x;
            Y = y;
            TriangleIndex = -1;
        }
    }
}