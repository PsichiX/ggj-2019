using GaryMoveOut.Items;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [System.Serializable]
    public struct SegmentSize
    {
        public float Width;
        public float Height;
        public float Depth;
    }

    public class Building
    {
		public Color wallsColor;
        public bool isBuildingIn;
        public Transform root;
        public Dictionary<int, Floor> Floors { get; private set; }
        public Dictionary<int, DoorPortal> Stairs { get; private set; }
		public FloorSize FloorSize;
        public SegmentSize SegmentSize { get; private set; }

        private ItemsSpawner itemsSpawner;


        public Building(ItemsSpawner itemsSpawner, SegmentSize segmentSize, FloorSize floorSize)
        {
            Floors = new Dictionary<int, Floor>();
            Stairs = new Dictionary<int, DoorPortal>();
			FloorSize = floorSize;

            this.itemsSpawner = itemsSpawner;
            SegmentSize = segmentSize;
        }


        public void DestroyFloor(int floorIndex)
        {
            if (Floors.TryGetValue(floorIndex, out Floor floor))
            {
                foreach (var segment in floor.segments)
                {
                    GameObject.Destroy(segment);
                }
                floor.items.Clear();
            }
        }

        private List<int> segmentIndicesTemp = new List<int>();

        public Vector3? GetSpawnPosition()
        {
            if (Stairs.TryGetValue(1, out DoorPortal door))
            {
                return door.transform.position;
            }
            return null;
        }

        public Dictionary<int, List<ItemScheme>> GetItems()
        {
            Dictionary<int, List<ItemScheme>> itemsByFloorsId = new Dictionary<int, List<ItemScheme>>();
            foreach (var floor in Floors)
            {
                itemsByFloorsId.Add(floor.Key, new List<ItemScheme>());
                foreach(var item in floor.Value.items)
                {
                    itemsByFloorsId[floor.Key].Add(item.Scheme);
                }
            }
            return itemsByFloorsId;
        }

		public void SpawnItemsInside(Dictionary<int, List<ItemScheme>> oldItems)
		{
			if (this.Floors.Count == 0 || oldItems == null)
			{
				return;
			}
			for (int i = 0; i < Floors.Count; i++)
			{
				if (oldItems.ContainsKey(i))
				{
					if (oldItems[i] != null && oldItems[i].Count > 0)
					{
						for (int itemId = 0; itemId < oldItems[i].Count; itemId++)
						{
							for (int s = 0; s < Floors[i].segments.Count; s++)
							{
								var itemSlot = Floors[i].segments[s].GetComponentInChildren<ItemSlot>();
								if (itemSlot != null && itemSlot.isOccupied == false)
								{
									if (oldItems[i][itemId] != null)
									{
										SpawnItem(oldItems[i][itemId], itemSlot, Floors[i]);
										break;
									}
								}
							}
						}
						var dif = oldItems[i].Count - Floors[i].segments.Count;
						// FixMe: If more segments, should move to another floor;
						if (dif > 0)
						{

						}
					}
				}

			}

			//while (itemsPlaced < items.Count && i < this.Floors.Count)
			//{
			//	if (this.Floors[i].Type == FloorType.GroundFloor)
			//	{
			//		continue;
			//	}
			//	var floor = this.Floors[i];
			//	foreach (var segment in floor.segments)
			//	{
			//		var itemSlot = segment.GetComponentInChildren<ItemSlot>();
			//		if (itemSlot != null && !itemSlot.isOccupied)
			//		{
			//			var scheme = items[itemsPlaced++];
			//			SpawnItem(scheme, itemSlot, floor);
			//			itemsPlaced++;
			//			break;
			//		}
			//	}
			//}
		}

		public void SpawnItemsInside(List<ItemScheme> items)
        {
            if (this.Floors.Count == 0)
            {
                return;
            }

            var itemsPerFloor = items.Count / (this.Floors.Count - 1);

            int i = 0;
            int itemsPlaced = 0;
            foreach (var floor in this.Floors.Values)
            {
                if (floor.Type == FloorType.GroundFloor)
                {
                    i++;
                    continue;
                }

                segmentIndicesTemp.Clear();
                for (i = 0; i < floor.segments.Count; i++)
                {
                    segmentIndicesTemp.Add(i);
                }

				ItemSlot bigItemSlot = null;
				for (i = 0; i < itemsPerFloor && segmentIndicesTemp.Count > 0;)
                {
                    var index = UnityEngine.Random.Range(0, segmentIndicesTemp.Count);
                    var si = segmentIndicesTemp[index];
                    segmentIndicesTemp.RemoveAt(index);

                    var itemSlot = floor.segments[si].GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null)
                    {
                        var scheme = items[itemsPlaced++];
						if (scheme.isSmall == false && scheme.canSpawnOnTop)
						{
							SpawnItem(scheme, itemSlot, floor);
							bigItemSlot = itemSlot;
						}
						else if (scheme.isSmall == true && bigItemSlot != null)
						{
							SpawnItem(scheme, bigItemSlot, floor, true);
							// comment this if you want many items ontop of one
							bigItemSlot = null;
						}
						else
						{
							SpawnItem(scheme, itemSlot, floor);
						}

                        i++;
                    }
                }
            }

            i = 0;
            while (itemsPlaced < items.Count && i < this.Floors.Count)
            {
                if (this.Floors[i].Type == FloorType.GroundFloor)
                {
                    i++;
                    continue;
                }
                var floor = this.Floors[i];
                foreach (var segment in floor.segments)
                {
                    var itemSlot = segment.GetComponentInChildren<ItemSlot>();
                    if (itemSlot != null && !itemSlot.isOccupied)
                    {
                        var scheme = items[itemsPlaced++];
                        SpawnItem(scheme, itemSlot, floor);
                        itemsPlaced++;
                        break;
                    }
                }
                i++;
            }
        }

        private void SpawnItem(ItemScheme scheme, ItemSlot itemSlot, Floor floor, bool addYTransform = false)
        {
            if (scheme != null && itemSlot != null)
            {
                var itemGO = itemsSpawner.SpawnItem(scheme, itemSlot.transform);
				if (addYTransform)
				{
					itemGO.transform.position += new Vector3(0, 1);
				}
                var item = itemGO.GetComponent<Item>();
                floor.AddItem(item);
                itemSlot.isOccupied = true;
            }
        }
    }
}