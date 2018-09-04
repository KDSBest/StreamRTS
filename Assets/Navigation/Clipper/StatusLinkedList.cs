// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

namespace Navigation.Clipper
{
    public class StatusLinkedList
    {
        private StatusNode root = new StatusNode();

        public bool isEmpty { get { return root.next == null; } }

        public StatusNode head { get { return root.next; } }

        public bool exists(StatusNode node)
        {
            if (node == null || node == root)
                return false;

            return true;
        }

        public Transition findTransition(EventNode ev)
        {
            var prev = root;
            var here = root.next;

            while (here != null)
            {
                if (findTransitionPredicate(ev, here))
                    break;

                prev = here;
                here = here.next;
            }

            return new Transition()
            {
                before = prev == root ? null : prev.ev,
                after = here != null ? here.ev : null,
                here = here,
                prev = prev
            };
        }

        public StatusNode insert(Transition surrounding, EventNode ev)
        {
            var prev = surrounding.prev;
            var here = surrounding.here;

            var node = new StatusNode() { ev = ev };

            node.prev = prev;
            node.next = here;
            prev.next = node;

            if (here != null)
            {
                here.prev = node;
            }

            return node;
        }

        private bool findTransitionPredicate(EventNode ev, StatusNode here)
        {
            var comp = statusCompare(ev, here.ev);
            return comp > 0;
        }

        private int statusCompare(EventNode ev1, EventNode ev2)
        {
            var a1 = ev1.seg.start;
            var a2 = ev1.seg.end;
            var b1 = ev2.seg.start;
            var b2 = ev2.seg.end;

            if (Epsilon.pointsCollinear(a1, b1, b2))
            {
                if (Epsilon.pointsCollinear(a2, b1, b2))
                    return 1;//eventCompare(true, a1, a2, true, b1, b2);

                return Epsilon.pointAboveOrOnLine(a2, b1, b2) ? 1 : -1;
            }

            return Epsilon.pointAboveOrOnLine(a1, b1, b2) ? 1 : -1;
        }
    }
}