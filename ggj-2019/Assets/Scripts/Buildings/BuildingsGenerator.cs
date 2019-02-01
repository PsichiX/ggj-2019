using GaryMoveOut.Items;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
	public class BuildingsGenerator
	{
		public BuildingSegmentsDatabase BuildingsDatabase { get; private set; }
		public ItemsSpawner ItemsSpawner { get; private set; }


		public BuildingsGenerator()
		{
			BuildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");

			ItemsSpawner = new ItemsSpawner();
		}


		public void DestroyBuildingOut(ref Building buildingOut)
		{
			foreach (var floor in buildingOut.Floors)
			{
				buildingOut.DestroyFloor(floor.Key);
				//foreach(var segment in floor.segments)
				//{
				//    GameObject.Destroy(segment);
				//}
				//floor.segments.Clear();

				//floor.items.Clear();

				GameObject.Destroy(floor.Value.gameObject);
			}
			buildingOut.Floors.Clear();
		}


		#region Building generator
		public Building GenerateBuilding(Transform root, int buildingFloorsCount, FloorSize floorSize, List<ItemScheme> items = null)
		{
			var building = ConstructBuilding(root, buildingFloorsCount, floorSize);
			if (items != null)
			{
				building.SpawnItemsInside(items);
			}
			Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
			return building;
		}

		public Building GenerateBuilding(Transform root, int oldHeight, FloorSize oldFloorSize, Dictionary<int, List<ItemScheme>> oldItems)
		{
			var building = ConstructBuilding(root, oldHeight, oldFloorSize);
			if (oldItems != null && oldItems.Count > 0)
			{
				building.SpawnItemsInside(oldItems);
			}
			Debug.Log($"Generated building COPY");
			return building;
		}

		private Building ConstructBuilding(Transform root, int buildingFloorsCount, FloorSize floorSize)
		{
			var floorScheme = BuildingsDatabase.GetRandomFloorScheme();
			var segmentSize = new SegmentSize()
			{
				Width = floorScheme.segmentWidth,
				Height = floorScheme.segmentHeight,
				Depth = floorScheme.segmentDepth
			};
			var building = new Building(ItemsSpawner, segmentSize, floorSize)
			{
				root = root
			};

			Vector3 position = root.position;
			Quaternion rotation = root.rotation;

			GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSize, 0, FloorType.GroundFloor);

			int index = 1;
			for (; index <= buildingFloorsCount; index++)
			{
				position += new Vector3(0f, floorScheme.segmentHeight, 0f);
				GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSize, index, FloorType.MiddleFloor);
			}

			position += new Vector3(0f, floorScheme.segmentHeight, 0f);
			var roofScheme = BuildingsDatabase.GetRandomRoofScheme();
			GenerateFloor(ref building, roofScheme, root, position, root.rotation, floorSize, index, FloorType.Roof);

			return building;
		}

		private void GenerateFloor(ref Building building, FloorScheme scheme, Transform floorParent, Vector3 position, Quaternion rotation, FloorSize floorSize, int floorIndex, FloorType type)
		{
			if (!building.Floors.TryGetValue(floorIndex, out Floor floor))
			{
				floor = Object.Instantiate(BuildingsDatabase.floorPrefab, floorParent.position, floorParent.rotation);
				building.Floors.Add(floorIndex, floor);
			}
			floor.Type = type;
			floor.transform.SetParent(floorParent);
			GameObject prefab = null;

			// add the right side:
			switch (floor.Type)
			{
				case FloorType.GroundFloor:
					prefab = scheme.SideDoor;
					break;
				case FloorType.Roof:
					prefab = scheme.SideWallR;
					break;
				default:
					prefab = scheme.SideWindowR;
					break;
			}
			var segment = GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject;
			var itemCatcher = segment.GetComponentInChildren<ItemCatcher>();
			if (itemCatcher != null)
			{
				itemCatcher.Setup(floor);
			}
			floor.segments.Add(segment);

			// add floor middle segments:
			for (int i = 0; i < floorSize.segmentsCount; i++)
			{
				if (type != FloorType.Roof && i == floorSize.stairsSegmentIndex)
				{
					prefab = scheme.Stairs;
				}
				else
				{
					prefab = scheme.EmptyWall;
				}

				segment = GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject;
				var doorPortal = segment.GetComponentInChildren<DoorPortal>();
				if (doorPortal != null)
				{
					doorPortal.floorIndex = floorIndex;
					// setup index below:
					var indexBelow = floorIndex - 1;
					indexBelow = (indexBelow < 0) ? DoorPortal.MinIndex : indexBelow;
					doorPortal.floorIndexBelow = indexBelow;
					// setup index above:
					var indexAbove = floorIndex + 1;
					indexAbove = (indexAbove < building.Floors.Count) ? DoorPortal.MaxIndex : indexAbove;
					doorPortal.floorIndexAbove = indexAbove;
					doorPortal.building = building;

					building.Stairs.Add(floorIndex, doorPortal);
				}
				floor.segments.Add(segment);
				position += new Vector3(scheme.segmentWidth, 0f, 0f);
			}

			// add empty wall OR window on the left side:
			switch (floor.Type)
			{
				case FloorType.Roof:
					prefab = scheme.SideWallL;
					break;
				case FloorType.MiddleFloor:
					prefab = scheme.SideWindowL;
					break;
				default:
					prefab = scheme.GetRandomSideWall(rightSide: false);
					break;
			}
			segment = GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject;
			itemCatcher = segment.GetComponent<ItemCatcher>();
			if (itemCatcher != null)
			{
				itemCatcher.Setup(floor);
			}
			floor.segments.Add(segment);
		}
		#endregion



		//// TBR
		//[System.Obsolete]
		//public Building GenerateBuildingWithItems(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex, List<ItemScheme_OLD> items)
		//{
		//    var floorSize = new FloorSize()
		//    {
		//        segmentsCount = floorSegmentsCount,
		//        stairsSegmentIndex = stairsSegmentIndex
		//    };
		//    var building = ConstructBuilding(root, buildingFloorsCount, floorSize);
		//    building.SpawnItemsInside(items);
		//    Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
		//    return building;
		//}
		//[System.Obsolete]
		//public Building GenerateBuildingWithItems(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex, List<Item_OLD> items)
		//{
		//    var floorSize = new FloorSize()
		//    {
		//        segmentsCount = floorSegmentsCount,
		//        stairsSegmentIndex = stairsSegmentIndex
		//    };
		//    var building = ConstructBuilding(root, buildingFloorsCount, floorSize);
		//    building.SpawnItemsInside(items);
		//    Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
		//    return building;
		//}
		//[System.Obsolete]
		//public Building GenerateBuildingWithItems(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex, Dictionary<int, List<Item_OLD>> items)
		//{
		//    var floorSize = new FloorSize()
		//    {
		//        segmentsCount = floorSegmentsCount,
		//        stairsSegmentIndex = stairsSegmentIndex
		//    };
		//    var building = ConstructBuilding(root, buildingFloorsCount, floorSize);
		//    building.SpawnItemsInside(items);
		//    Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
		//    return building;
		//}
	}
}