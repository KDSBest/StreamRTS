using ClipperLib;
using Unity.Collections;
using Unity.Jobs;

namespace Navigation
{
    public struct GridFindTrianglesJob : IJobParallelFor
    {
        public NativeArray<int> TriangleIndexes;

        [ReadOnly]
        public NativeArray<NavigationTriangle> Triangles;

        public int MaxX;
        public int StartX;
        public int StartY;

        public void Execute(int i)
        {
            TriangleIndexes[i] = -1;
            int x = StartX + i % MaxX;
            int y = StartY + i / MaxX;

            for (int ii = 0; ii < Triangles.Length; ii++)
            {
                bool isInTriangle = Triangles[ii].IsPointInTriangle(new IntPoint(x, y));
                if (isInTriangle)
                {
                    TriangleIndexes[i] = ii;
                    break;
                }
            }

        }
    }
}