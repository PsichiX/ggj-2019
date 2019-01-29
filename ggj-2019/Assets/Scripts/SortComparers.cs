using System.Collections.Generic;

namespace GaryMoveOut
{
    public class FloorMaxToMinItemsComparer : IComparer<Floor>
    {
        public int Compare(Floor x, Floor y)
        {
            if (x.items_OLD.Count > y.items_OLD.Count)
                return -1;
            else if (x.items_OLD.Count == y.items_OLD.Count)
                return 0;
            else
                return 1;
        }
    }
}