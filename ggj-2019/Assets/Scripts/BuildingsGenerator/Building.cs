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

            for(int i = 0; i < floors.Count; i++)
            {

            }
        }
    }

}