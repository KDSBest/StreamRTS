using ClipperLib;

namespace Navigation
{
    public static class NavigationTriangleExtensions
    {
        private static int Sign(IntPoint p1, IntPoint p2, IntPoint p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool IsPointInTriangle(this NavigationTriangle triangle, IntPoint p)
        {
            bool b1 = Sign(p, triangle.U, triangle.V) < 0.0f;
            bool b2 = Sign(p, triangle.V, triangle.W) < 0.0f;

            if (b1 != b2)
                return false;

            bool b3 = Sign(p, triangle.W, triangle.U) < 0.0f;

            return b2 == b3;
        }
    }
}