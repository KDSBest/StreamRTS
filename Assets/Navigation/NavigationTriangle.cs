using ClipperLib;
using JetBrains.Annotations;

namespace Navigation
{
    public class NavigationTriangle
    {
        public IntPoint U;
        public IntPoint V;
        public IntPoint W;

        public IntPoint GetMiddlePoint()
        {
            return (U + V + W) / 3;
        }

        public NavigationTriangle()
        {
            U = new IntPoint();
            V = new IntPoint();
            W = new IntPoint();
        }

        public NavigationTriangle(IntPoint u, IntPoint v, IntPoint w)
        {
            U = u;
            V = v;
            W = w;
        }

        public static bool operator ==(NavigationTriangle a, NavigationTriangle b)
        {
            if (((object) a) == null)
            {
                return ((object) b) == null;
            }
            return a.Equals(b);
        }

        public static bool operator !=(NavigationTriangle a, NavigationTriangle b)
        {
            if (((object)a) == null)
            {
                return ((object)b) == null;
            }
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode() ^ W.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            NavigationTriangle other = obj as NavigationTriangle;

            if (other == null)
                return false;

            return !(this.U != other.U || V != other.V || W != other.W);
        }
    }
}