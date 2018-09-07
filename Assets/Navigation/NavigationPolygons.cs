using System.Collections.Generic;

namespace Navigation
{
    public class NavigationPolygons : List<NavigationPolygon>
    {
        public NavigationPolygons()
        {

        }

        public NavigationPolygons(int cnt) : base(cnt)
        {

        }

        public NavigationPolygons(IEnumerable<NavigationPolygon> polygons)
        {
            this.AddRange(polygons);
        }

        public NavigationPolygons DeepCopy()
        {
            NavigationPolygons copy = new NavigationPolygons(this.Count);

            for (var i = 0; i < this.Count; i++)
            {
                copy.Add(this[i].DeepCopy());
            }

            return copy;
        }
    }
}