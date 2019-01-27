using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class Building
    {
        public Transform root;
        public Dictionary<int, Floor> floors;
        public Dictionary<int, DoorPortal> stairs;

        public float SegmentWidth { get; private set; }
        public float SegmentHeight { get; private set; }
        public float SegmentDepth { get; private set; }


        public Building(float segmentWidth, float segmentHeight, float segmentDepth)
        {
            floors = new Dictionary<int, Floor>();
            stairs = new Dictionary<int, DoorPortal>();

            SegmentWidth = segmentWidth;
            SegmentHeight = segmentHeight;
            SegmentDepth = segmentDepth;
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

        public void DestroyFloor(int floorIndex)
        {
            if (floors.TryGetValue(floorIndex, out Floor floor))
            {
                foreach(var segment in floor.segments)
                {
                    GameObject.Destroy(segment);
                }
                floor.items.Clear();
            }
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
                    var index = UnityEngine.Random.Range(0, segmentIndices.Count);
                    var si = segmentIndices[index];
                    segmentIndices.RemoveAt(index);

                    var itemSlot = floor.segments[si].GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null)
                    {
                        var item = new Item(items[itemsPlaced++]);
                        SpawnItem(item, itemSlot, floor);
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
                        SpawnItem(item, itemSlot, floor);
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
            
            int index = 0;
            List<Item> unstackedItems = new List<Item>();
            foreach(var floor in floors)
            {
                List<int> indices = new List<int>();
                for(int i = 0; i < floor.Value.segments.Count; i++)
                {
                    indices.Add(i);
                }

                while (itemsByFloorIndex[floor.Key].Count > 0 && indices.Count > 0)
                {
                    var rnd = UnityEngine.Random.Range(0, indices.Count);
                    index = indices[rnd];
                    indices.RemoveAt(rnd);

                    var itemSlot = floor.Value.segments[index].GetComponent<ItemSlot>();
                    if (itemSlot != null && !itemSlot.isOccupied)
                    {
                        var item = itemsByFloorIndex[floor.Key][0];
                        itemsByFloorIndex[floor.Key].RemoveAt(0);
                        SpawnItem(item, itemSlot, floor.Value);
                    }
                }
                if (itemsByFloorIndex[floor.Key].Count > 0)
                {
                    unstackedItems.AddRange(itemsByFloorIndex[floor.Key]);
                }
            }

            // place unstacked items:
            var floorsList = new List<Floor>(floors.Values);
            index = 0;
            while (unstackedItems.Count > 0)
            {
                var floor = floorsList[index];
                foreach (var segment in floor.segments)
                {
                    var itemSlot = segment.GetComponent<ItemSlot>();
                    if (itemSlot != null && !itemSlot.isOccupied)
                    {
                        var item = unstackedItems[0];
                        unstackedItems.RemoveAt(0);
                        SpawnItem(item, itemSlot, floor);
                        break;
                    }
                }
                index = (++index == floorsList.Count) ? 0 : index;
            }

        }

        private void SpawnItem(Item item, ItemSlot itemSlot, Floor floor)
        {
            var itemGO = GameObject.Instantiate(item.prefab, itemSlot.gameObject.transform);
            itemGO.transform.SetParent(null);
            //floor.items.Add(item);
            floor.AddItem(item, itemGO);
            itemSlot.isOccupied = true;
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