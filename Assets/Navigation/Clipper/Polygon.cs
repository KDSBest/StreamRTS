﻿// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

using System.Collections.Generic;

namespace Navigation.Clipper
{
    public class Polygon
    {
        public List<PointList> regions = null;
        public bool inverted = false;
    }
}