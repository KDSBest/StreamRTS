﻿// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public struct Transition
    {
        public EventNode before;
        public EventNode after;

        public StatusNode prev;
        public StatusNode here;
    }
}