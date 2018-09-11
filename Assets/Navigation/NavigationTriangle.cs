using Assets.Navigation;
using ClipperLib;
using JetBrains.Annotations;
using Navigation.DeterministicMath;
using UnityEngine;

namespace Navigation
{
    public class NavigationTriangle
    {
        public DeterministicVector2 U;
        public DeterministicVector2 V;
        public DeterministicVector2 W;

        public NavigationEdge S0
        {
            get
            {
                return new NavigationEdge(U, V);
            }
        }

        public NavigationEdge S1
        {
            get
            {
                return new NavigationEdge(V, W);
            }
        }

        public NavigationEdge S2
        {
            get
            {
                return new NavigationEdge(W, U);
            }
        }
        private static DeterministicInt Sign(DeterministicVector2 p1, DeterministicVector2 p2, DeterministicVector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }


        public bool IsPointInTriangle(DeterministicVector2 p)
        {
            bool b1 = Sign(p, this.U, this.V) < 0;
            bool b2 = Sign(p, this.V, this.W) < 0;

            if (b1 != b2)
                return false;

            bool b3 = Sign(p, this.W, this.U) < 0;

            return b2 == b3;
        }

        public DeterministicVector2 GetMiddlePoint()
        {
            return (U + V + W) / 3;
        }

        public NavigationTriangle(DeterministicVector2 u, DeterministicVector2 v, DeterministicVector2 w)
        {
            U = u;
            V = v;
            W = w;
        }

        public static bool operator ==(NavigationTriangle a, NavigationTriangle b)
        {
            if (((object)a) == null)
            {
                return ((object)b) == null;
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