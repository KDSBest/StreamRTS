// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public struct Intersection
    {
        public static readonly Intersection Empty = new Intersection();

        //  alongA and alongB will each be one of: -2, -1, 0, 1, 2
        //
        //  with the following meaning:
        //
        //    -2   intersection point is before segment's first point
        //    -1   intersection point is directly on segment's first point
        //     0   intersection point is between segment's first and second points (exclusive)
        //     1   intersection point is directly on segment's second point
        //     2   intersection point is after segment's second point

        /// <summary>
        /// where the intersection point is at
        /// </summary>
        public Point pt;

        /// <summary>
        /// where intersection point is along A
        /// </summary>
        public int alongA;

        /// <summary>
        /// where intersection point is along B
        /// </summary>
        public int alongB;
    }
}