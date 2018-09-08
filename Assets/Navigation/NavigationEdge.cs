using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace Navigation
{
    public class NavigationEdge
    {
        public IntPoint A;
        public IntPoint B;

        public NavigationEdge()
        {

        }

        public NavigationEdge(IntPoint a, IntPoint b)
        {
            A = a;
            B = b;
        }

        public NavigationPolygon BoundingBox()
        {
            var result = new NavigationPolygon(4);
            result.Add(new IntPoint(A.X, A.Y));
            result.Add(new IntPoint(B.X, A.Y));
            result.Add(new IntPoint(B.X, B.Y));
            result.Add(new IntPoint(A.X, B.Y));

            return result;
        }
        public IntPoint Midpoint()
        {
            return (A + B) / 2;
        }

        public bool EncroachedUpon(IntPoint p)
        {
            if (p == A || p == B) return false;
            var radius = (A - B).GetLengthSquared() / 2;
            return (Midpoint() - p).GetLengthSquared() < radius;
        }

        public IntPoint GetDirection()
        {
            return B - A;
        }

        public int GetLengthSquared()
        {
            var size = this.GetDirection();
            return size.X * size.X + size.Y * size.Y;
        }

        public EdgeIntersectionResult CalculateIntersection(NavigationEdge other)
        {
            var result = new EdgeIntersectionResult();

            int dx12 = this.B.X - this.A.X;
            int dy12 = this.B.Y - this.A.Y;
            int dx34 = other.B.X - other.A.X;
            int dy34 = other.B.Y - other.A.Y;

            int denominator = dy12 * dx34 - dx12 * dy34;

            if (denominator == 0)
            {
                return result;
            }

            // normally t1 is 0 -> 1 (or no hit), but for us it's 0 -> 100
            int t1 = ((this.A.X - other.A.X) * dy34 + (other.A.Y - this.A.Y) * dx34) * 100 / denominator;

            result.LinesIntersect = true;

            // normally t1 is 0 -> 1 (or no hit), but for us it's 0 -> 100
            int t2 = ((other.A.X - this.A.X) * dy12 + (this.A.Y - other.A.Y) * dx12) * 100 / -denominator;

            result.IntersectionPoint = new IntPoint(this.A.X + dx12 * t1 / 100, this.A.Y + dy12 * t1 / 100);

            result.SegmentsIntersect = t1 >= 0 && t1 <= 100 && t2 >= 0 && t2 <= 100;

            // find closes points
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if(t1 > 100)
            {
                t1 = 100;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 100)
            {
                t2 = 100;
            }

            result.SegmentIntersection = new NavigationEdge(new IntPoint(this.A.X + dx12 * t1 / 100, this.A.Y + dy12 * t1 / 100), new IntPoint(other.A.X + dx34 * t2 / 100, other.A.Y + dy34 * t2 / 100));
            return result;
        }

        public static bool operator ==(NavigationEdge a, NavigationEdge b)
        {
            if (((object) a) == null)
            {
                return ((object) b) == null;
            }

            if (((object)b) == null)
            {
                return false;
            }

            return (a.A == b.A && a.B == b.B) || (a.A == b.B && a.B == b.A);
        }

        public static bool operator !=(NavigationEdge a, NavigationEdge b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            NavigationEdge other = obj as NavigationEdge;
            if (other == null)
                return false;

            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool HasPoint(IntPoint p)
        {
            return A == p || B == p;
        }
    }
}
