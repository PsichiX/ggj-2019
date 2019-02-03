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


		public void DestroyBuilding(ref Building building)
		{
			foreach (var floor in building.Floors)
			{
				building.DestroyFloor(floor.Key);
				GameObject.Destroy(floor.Value.gameObject);
			}
			building.Floors.Clear();
		}


		#region Building generator
		public Building GenerateBuilding(Transform root, int buildingFloorsCount, FloorSize floorSize, bool isBuildingIn, List<ItemScheme> items = null)
		{
			useOldColor = true;
			var building = ConstructBuilding(root, buildingFloorsCount, floorSize, isBuildingIn);
			if (items != null)
			{
				building.SpawnItemsInside(items);
			}
			Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
			return building;
		}

		private bool useOldColor;
		public Building GenerateBuildingBasedOnOldOne(Transform root, int oldHeight, FloorSize oldFloorSize, Dictionary<int, List<ItemScheme>> oldItems, Color oldColor)
		{
			useOldColor = false;
			var building = ConstructBuilding(root, oldHeight, oldFloorSize, isBuildingIn: false);
			if (oldItems != null && oldItems.Count > 0)
			{
				building.SpawnItemsInside(oldItems);
			}
			Debug.Log($"Generated building COPY");
			return building;
		}

		private Building ConstructBuilding(Transform root, int buildingFloorsCount, FloorSize floorSize, bool isBuildingIn)
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
				root = root,
                isBuildingIn = isBuildingIn
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
				floor = Object.Instantiate(BuildingsDatabase.floorPrefab, position, floorParent.rotation);
                floor.gameObject.name = $"Floor_{floorIndex}";
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
			//var itemCatcher = segment.GetComponentInChildren<ItemCatcher_OLD>();
			//if (itemCatcher != null)
			//{
			//	itemCatcher.Setup(floor);
			//}
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
					if (useOldColor)
					{
						building.wallsColor = Random.ColorHSV(0,1,0,1,0.65f,0.95f);
						var mr = prefab.GetComponentInChildren<MeshRenderer>();
						mr.sharedMaterial.color = building.wallsColor;
					}
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
			//itemCatcher = segment.GetComponent<ItemCatcher_OLD>();
			//if (itemCatcher != null)
			//{
			//	itemCatcher.Setup(floor);
			//}
			floor.segments.Add(segment);

            // generate ItemCatcher game object:
            if (type != FloorType.Roof)
            {
                var floorWidth = Vector3.Distance(floor.segments[0].transform.position, floor.segments[floor.segments.Count - 2].transform.position);
                var widthRatio = building.isBuildingIn ? 0.9f : 1.2f;
                var realFloorSize = new Vector3((floorWidth + scheme.segmentWidth) * widthRatio, scheme.segmentHeight * 0.8f);
                var center = new Vector3(floor.transform.position.x + floorWidth * 0.5f, position.y + scheme.segmentHeight * 0.5f);
                var itemCatcher = new GameObject("ItemCatcher").AddComponent<ItemCatcher>();
                itemCatcher.gameObject.transform.SetParent(floor.gameObject.transform);
                itemCatcher.Setup(center, realFloorSize, floor);
            }
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