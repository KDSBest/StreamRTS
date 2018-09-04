// PolygonClipper is a deterministic Version of PolyBoolCS is Copyright(c) 2018 Kevin Setiono
// The PolyBoolCS port is Copyright(c) 2017 StagPoint Software(@DoobiusGanes, web: stagpoint.com)
// polybooljs is (c) Copyright 2016, Sean Connelly (@voidqk), http://syntheti.cc
// MIT License

using System;

namespace Navigation.Clipper
{
    public class PolygonClipper
    {
        public Polygon union(Polygon poly1, Polygon poly2)
        {
            return operate(poly1, poly2, selectUnion);
        }

        public SegmentList selectUnion(CombinedSegmentLists combined)
        {
            var result = SegmentSelector.union(combined.combined);
            result.inverted = combined.inverted1 || combined.inverted2;

            return result;
        }

        public Polygon difference(Polygon poly1, Polygon poly2)
        {
            return operate(poly1, poly2, selectDifference);
        }

        public SegmentList selectDifference(CombinedSegmentLists combined)
        {
            var result = SegmentSelector.difference(combined.combined);
            result.inverted = combined.inverted1 && !combined.inverted2;

            return result;
        }

        public SegmentList segments(Polygon poly)
        {
            var i = new Intersecter(true);

            foreach (var region in poly.regions)
            {
                i.addRegion(region);
            }

            var result = i.calculate(poly.inverted);
            result.inverted = poly.inverted;

            return result;
        }

        public CombinedSegmentLists combine(SegmentList segments1, SegmentList segments2)
        {
            var i = new Intersecter(false);

            return new CombinedSegmentLists()
            {
                combined = i.calculate(
                    segments1, segments1.inverted,
                    segments2, segments2.inverted
                ),
                inverted1 = segments1.inverted,
                inverted2 = segments2.inverted
            };
        }

        public Polygon polygon(SegmentList segments)
        {
            var chain = new SegmentChainer().chain(segments);

            return new Polygon()
            {
                regions = chain,
                inverted = segments.inverted
            };
        }

        private Polygon operate(Polygon poly1, Polygon poly2, Func<CombinedSegmentLists, SegmentList> selector)
        {
            var seg1 = segments(poly1);
            var seg2 = segments(poly2);
            var comb = combine(seg1, seg2);

            var seg3 = selector(comb);

            return polygon(seg3);
        }
    }
}
