using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class Building
    {
        public Dictionary<int, Floor> floors;
        public Dictionary<int, DoorPortal> stairs;


        public Building()
        {
            floors = new Dictionary<int, Floor>();
            stairs = new Dictionary<int, DoorPortal>();
        }


        private List<int> segmentIndices = new List<int>();

        public Vector3? GetSpawnPosition()
        {
            if (stairs.TryGetValue(1, out DoorPortal door))
            {
                return door.transform.position;
            }
            return null;
        }

        public void SpawnItemsInside(List<ItemScheme> items)
        {
            if (this.floors.Count == 0)
            {
                return;
            }

            var itemsPerFloor = items.Count / (this.floors.Count - 1);

            int i = 0;
            int itemsPlaced = 0;
            foreach (var floor in this.floors.Values)
            {
                if (floor.Type == FloorType.GroundFloor)
                {
                    i++;
                    continue;
                }

                segmentIndices.Clear();
                for (i = 0; i < floor.segments.Count; i++)
                {
                    segmentIndices.Add(i);
                }

                for (i = 0; i < itemsPerFloor && segmentIndices.Count > 0;)
                {
                    var index = Random.Range(0, segmentIndices.Count);
                    var si = segmentIndices[index];
                    segmentIndices.RemoveAt(index);

                    var itemSlot = floor.segments[si].GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null)
                    {
                        var item = new Item(items[itemsPlaced++]);
                        var itemGO = GameObject.Instantiate(item.prefab, itemSlot.gameObject.transform);
                        floor.items.Add(item);
                        itemSlot.isOccupied = true;
                        i++;
                    }
                }
            }

            i = 0;
            while (itemsPlaced < items.Count && i < this.floors.Count)
            {
                if (this.floors[i].Type == FloorType.GroundFloor)
                {
                    i++;
                    continue;
                }
                var floor = this.floors[i];
                foreach (var segment in floor.segments)
                {
                    var itemSlot = segment.GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null && !itemSlot.isOccupied)
                    {
                        var item = new Item(items[itemsPlaced++]);
                        var itemGO = GameObject.Instantiate(item.prefab, itemSlot.gameObject.transform);
                        floor.items.Add(item);
                        itemSlot.isOccupied = true;
                        itemsPlaced++;
                        break;
                    }
                }
                i++;
            }
        }

        public void SpawnItemsInside(Dictionary<int, List<Item>> itemsByFloorIndex)
        {
            if (this.floors.Count == 0)
            {
                return;
            }






        }

        public Dictionary<int, List<Item>> GetItems()
        {
            Dictionary<int, List<Item>> itemsByFloorsId = new Dictionary<int, List<Item>>();
            foreach (var floor in floors)
            {
                itemsByFloorsId.Add(floor.Key, new List<Item>(floor.Value.items));
            }
            return itemsByFloorsId;
        }
    }
}