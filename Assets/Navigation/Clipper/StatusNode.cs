// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public class StatusNode
    {
        public EventNode ev;

        public StatusNode next;
        public StatusNode prev;

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
    }
}