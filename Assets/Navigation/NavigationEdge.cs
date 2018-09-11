using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using Navigation.DeterministicMath;

namespace Navigation
{
    public class NavigationEdge
    {
        public DeterministicVector2 A;
        public DeterministicVector2 B;

        public NavigationEdge()
        {

        }

        public NavigationEdge(DeterministicVector2 a, DeterministicVector2 b)
        {
            A = a;
            B = b;
        }

        public NavigationPolygon BoundingBox()
        {
            var result = new NavigationPolygon(4);
            result.Add(new DeterministicVector2(A.X, A.Y));
            result.Add(new DeterministicVector2(B.X, A.Y));
            result.Add(new DeterministicVector2(B.X, B.Y));
            result.Add(new DeterministicVector2(A.X, B.Y));

            return result;
        }
        public DeterministicVector2 Midpoint()
        {
            return (A + B) / 2;
        }

        public bool EncroachedUpon(DeterministicVector2 p)
        {
            if (p == A || p == B) return false;
            var radius = (A - B).GetLengthSquared() / 2;
            return (Midpoint() - p).GetLengthSquared() < radius;
        }

        public DeterministicVector2 GetDirection()
        {
            return B - A;
        }

        public DeterministicInt GetLengthSquared()
        {
            var size = this.GetDirection();
            return size.X * size.X + size.Y * size.Y;
        }

        public EdgeIntersectionResult CalculateIntersection(NavigationEdge other)
        {
            var result = new EdgeIntersectionResult();

            var dx12 = this.B.X - this.A.X;
            var dy12 = this.B.Y - this.A.Y;
            var dx34 = other.B.X - other.A.X;
            var dy34 = other.B.Y - other.A.Y;

            var denominator = dy12 * dx34 - dx12 * dy34;

            if (denominator == 0)
            {
                return result;
            }

            // normally t1 is 0 -> 1 (or no hit), but for us it's 0 -> 1
            var t1 = ((this.A.X - other.A.X) * dy34 + (other.A.Y - this.A.Y) * dx34) / denominator;

            result.LinesIntersect = true;

            // normally t1 is 0 -> 1 (or no hit), but for us it's 0 -> 1
            var t2 = ((other.A.X - this.A.X) * dy12 + (this.A.Y - other.A.Y) * dx12) / -denominator;

            result.IntersectionPoint = new DeterministicVector2(this.A.X + dx12 * t1, this.A.Y + dy12 * t1);

            result.SegmentsIntersect = t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
            result.Deltas = new DeterministicVector2(t1, t2);
            // find closes points
            if (t1 < 0)
            {
                t1 = new DeterministicInt(0);
            }
            else if(t1 > 1)
            {
                t1 = new DeterministicInt(1);
            }

            if (t2 < 0)
            {
                t1 = new DeterministicInt(0);
            }
            else if (t2 > 1)
            {
                t2 = new DeterministicInt(1);
            }

            result.SegmentIntersection = new NavigationEdge(new DeterministicVector2(this.A.X + dx12 * t1, this.A.Y + dy12 * t1), new DeterministicVector2(other.A.X + dx34 * t2, other.A.Y + dy34 * t2));
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

        public bool HasPoint(DeterministicVector2 p)
        {
            return A == p || B == p;
        }
    }
}
