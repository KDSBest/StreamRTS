// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public class EventLinkedList
    {
        private EventNode root = new EventNode();

        public bool isEmpty { get { return root.next == null; } }

        public EventNode head { get { return root.next; } }

        public void insertBefore(EventNode node, Point other_pt)
        {
            var last = root;
            var here = root.next;

            while (here != null)
            {
                if (insertBeforePredicate(here, node, ref other_pt))
                {
                    node.prev = here.prev;
                    node.next = here;
                    here.prev.next = node;
                    here.prev = node;

                    return;
                }

                last = here;
                here = here.next;
            }

            last.next = node;
            node.prev = last;
            node.next = null;
        }

        private bool insertBeforePredicate(EventNode here, EventNode ev, ref Point other_pt)
        {
            // should ev be inserted before here?
            var comp = eventCompare(
                ev.isStart,
                ref ev.pt,
                ref other_pt,
                here.isStart,
                ref here.pt,
                ref here.other.pt
            );

            return comp < 0;
        }

        private int eventCompare(bool p1_isStart, ref Point p1_1, ref Point p1_2, bool p2_isStart, ref Point p2_1, ref Point p2_2)
        {
            // compare the selected points first
            var comp = Epsilon.pointsCompare(p1_1, p2_1);
            if (comp != 0)
                return comp;

            // the selected points are the same

            if (Epsilon.pointsSame(p1_2, p2_2)) // if the non-selected points are the same too...
                return 0; // then the segments are equal

            if (p1_isStart != p2_isStart) // if one is a start and the other isn't...
                return p1_isStart ? 1 : -1; // favor the one that isn't the start

            // otherwise, we'll have to calculate which one is below the other manually
            return Epsilon.pointAboveOrOnLine(
                p1_2,
                p2_isStart ? p2_1 : p2_2, // order matters
                p2_isStart ? p2_2 : p2_1
            ) ? 1 : -1;
        }
    }
}