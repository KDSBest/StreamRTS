﻿// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public class EventNode
    {
        public bool isStart;
        public Point pt;
        public Segment seg;
        public bool primary;
        public EventNode other;
        public StatusNode status;

        public EventNode next;
        public EventNode prev;

        public void remove()
        {
            prev.next = next;

            if (next != null)
            {
                next.prev = prev;
            }

            prev = null;
            next = null;
        }

        public override string ToString()
        {
            return string.Format("Start={0}, Point={1}, Segment={2}", isStart, pt, seg);
        }
    }
}