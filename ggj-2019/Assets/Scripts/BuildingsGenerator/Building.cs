using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GaryMoveOut
{
    public class Building
    {
        public Dictionary<int, Floor> floors;
        public List<DoorPortal> stairs;


        public Building()
        {
            floors = new Dictionary<int, Floor>();
            stairs = new List<DoorPortal>();
        }


        public void RelocateItems()
        {
            var floors = new List<Floor>(this.floors.Values);
            floors.Sort(new FloorMaxToMinItemsComparer());

            for(int fi = 0; fi < floors.Count; fi++)
            {
                int si = 0;
                int i = 0;
                for(; i < floors[fi].items.Count && si < floors[fi].segments.Count; si++)
                {
                    var item = floors[fi].items[i];
                    var itemSlot = floors[fi].segments[si].GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null)
                    {
                        item.transform.position = itemSlot.transform.position;
                        item.transform.rotation = Quaternion.identity;
                        i++;
                    }
                }

                // move the rest items to another floor:
                int nfi = fi + 1;
                if (nfi < floors.Count)
                {
                    for (; i < floors[fi].items.Count; i++)
                    {
                        floors[nfi].items.Add(floors[fi].items[i]);
                        floors[fi].items.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        List<int> segmentIndices = new List<int>();
        public void RelocateItem2()
        {
            //var floors = new List<Floor>(this.floors.Values);
            //floors.Sort(new FloorMaxToMinItemsComparer());

            //int i;
            //for (int fi = 0; fi < floors.Count; fi++)
            //{
            //    segmentIndices.Clear();
            //    for(i = 0; i < floors[fi].segments.Count; i++)
            //    {
            //        segmentIndices.Add(i);
            //    }

            //    for(int i = 0; i < floors[fi].items.Count && segmentIndices.Count > 0; )
            //    {
            //        var item = floors[fi].items[i];
            //        while (segmentIndices.Count > 0)
            //        {
            //            var index = Random.Range(0, segmentIndices.Count);
            //            var si = segmentIndices[index];
            //            segmentIndices.RemoveAt(index);

            //            var itemSlot = floors[fi].segments[si].GetComponentInChildren<ItemSlot>();
            //            if (itemSlot != null)
            //            {
            //                item.transform.position = itemSlot.transform.position;
            //                item.transform.rotation = Quaternion.identity;
            //                i++;
            //                break;
            //            }
            //        }
            //    }



                //for (; i < floors[fi].items.Count && si < floors[fi].segments.Count; si++)
                //{
                //    var item = floors[fi].items[i];
                //    var itemSlot = floors[fi].segments[si].GetComponentInChildren<ItemSlot>();
                //    if (itemSlot != null)
                //    {
                //        item.transform.position = itemSlot.transform.position;
                //        item.transform.rotation = Quaternion.identity;
                //        i++;
                //    }
                //}





                //// move the rest items to another floor:
                //int nfi = fi + 1;
                //if (nfi < floors.Count)
                //{
                //    for (; i < floors[fi].items.Count; i++)
                //    {
                //        floors[nfi].items.Add(floors[fi].items[i]);
                //        floors[fi].items.RemoveAt(i);
                //        i--;
                //    }
                //}
            //}
        }
    }

}