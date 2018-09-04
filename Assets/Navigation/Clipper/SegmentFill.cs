// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public class SegmentFill
    {
        // NOTE: This is kind of asinine, but the original javascript code used (below === null) to determine that the edge had not 
        // yet been processed, and treated below as a standard true/false in every other case, necessitating the use of a nullable 
        // bool here.

        public bool above;
        public bool? below;
    }
}