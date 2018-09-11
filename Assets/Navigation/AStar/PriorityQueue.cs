using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace Assets.Navigation.AStar
{
    public class PriorityQueue<T> where T : class, IPriorityQueueEntry
    {
        private Dictionary<int, List<T>> entries = new Dictionary<int, List<T>>();

        public int Count
        {
            get { return entries.Count; }
        }

        public void Add(T entry)
        {
            int cost = entry.GetCost().ToInt();

            if (!entries.ContainsKey(cost))
            {
                entries.Add(cost, new List<T>());
            }

            entries[cost].Add(entry);
        }

        public bool Remove(T entry)
        {
            int cost = entry.GetCost().ToInt();

            if (!entries.ContainsKey(cost))
            {
                return false;
            }

            bool result = entries[cost].Remove(entry);
            if(entries[cost].Count == 0)
            {
                entries.Remove(cost);
            }

            return result;
        }

        public T GetNext()
        {
            if (entries.Count == 0)
                return null;

            int lowKey = entries.Keys.Min();

            T ret = entries[lowKey][0];
            entries[lowKey].RemoveAt(0);

            if (entries[lowKey].Count == 0)
            {
                entries.Remove(lowKey);
            }

            return ret;
        }

        public void Clear()
        {
            entries.Clear();
        }
    }
}
