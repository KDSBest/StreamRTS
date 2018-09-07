using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace Navigation
{
    public class NavigationPolygon : List<IntPoint>
    {
        public NavigationPolygon()
        {

        }

        public NavigationPolygon(int cnt) : base(cnt)
        {

        }

        public NavigationEdge GetBounding()
        {
            var min = new IntPoint(int.MaxValue, int.MaxValue);
            var max = new IntPoint(int.MinValue, int.MinValue);

            foreach (var p in this)
            {
                min.X = Math.Min(p.X, min.X);
                min.Y = Math.Min(p.Y, min.Y);
                max.X = Math.Max(p.X, max.X);
                max.Y = Math.Max(p.Y, max.Y);
            }

            return new NavigationEdge(min, max);
        }

        public NavigationPolygon DeepCopy()
        {
            NavigationPolygon copy = new NavigationPolygon(this.Count);

            for (var i = 0; i < this.Count; i++)
            {
                copy.Add(this[i].DeepCopy());
            }

            return copy;
        }
    }
}
