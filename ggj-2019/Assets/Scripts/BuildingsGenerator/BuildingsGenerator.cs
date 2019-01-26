using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class BuildingsGenerator
    {
        public BuildingSegmentsDatabase BuildingsDatabase { get; private set; }
        public ItemsDatabase ItemsDatabase { get; private set; }


        public BuildingsGenerator()
        {
            BuildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
            ItemsDatabase = Resources.Load<ItemsDatabase>("Databases/ItemsDatabase");
            ItemsDatabase.LoadItemsFromAssets();
        }


        public void DestroyBuildingOut(ref Building buildingOut)
        {
            foreach(var floor in buildingOut.floors.Values)
            {
                foreach(var segment in floor.segments)
                {
                    GameObject.Destroy(segment);
                }
                floor.segments.Clear();

                floor.items.Clear();
                GameObject.Destroy(floor.gameObject);
            }
            buildingOut.floors.Clear();
        }

        public Building GenerateBuilding(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex)
        {
            var building = ConstructBuilding(root, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex);
            Debug.Log($"Generated building with {floorSegmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {stairsSegmentIndex} floor segment");
            return building;
        }


        public Building GenerateBuildingWithItems(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex, List<ItemScheme> items)
        {
            var building = ConstructBuilding(root, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex);
            building.SpawnItemsInside(items);
            Debug.Log($"Generated building with {floorSegmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {stairsSegmentIndex} floor segment");
            return building;
        }

        private Building ConstructBuilding(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex)
        {
            var building = new Building();
            var floorScheme = BuildingsDatabase.GetRandomFloorScheme();

            Vector3 position = root.position;
            Quaternion rotation = root.rotation;

            GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSegmentsCount, stairsSegmentIndex, 0, FloorType.GroundFloor);

            int index = 1;
            for (; index <= buildingFloorsCount; index++)
            {
                position += new Vector3(0f, floorScheme.segmentHeight, 0f);
                GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSegmentsCount, stairsSegmentIndex, index, FloorType.MiddleFloor);
            }

            position += new Vector3(0f, floorScheme.segmentHeight, 0f);
            var roofScheme = BuildingsDatabase.GetRandomRoofScheme();
            GenerateFloor(ref building, roofScheme, root, position, root.rotation, floorSegmentsCount, stairsSegmentIndex, index, FloorType.Roof);

            return building;
        }


        public Building GenerateBuildingWithItems(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex, Dictionary<int, List<Item>> items)
        {
            var building = ConstructBuilding(root, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex);
            building.SpawnItemsInside(items);
            Debug.Log($"Generated building with {floorSegmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {stairsSegmentIndex} floor segment");
            return building;
        }

        private void GenerateFloor(ref Building building,
                                   FloorScheme scheme,
                                   Transform floorParent,
                                   Vector3 position,
                                   Quaternion rotation,
                                   int floorSegmentsCount,
                                   int stairsSegmentIndex,
                                   int floorIndex,
                                   FloorType type)
        {
            if (!building.floors.TryGetValue(floorIndex, out Floor floor))
            {
                floor = Object.Instantiate(BuildingsDatabase.floorPrefab, floorParent.position, floorParent.rotation);
                building.floors.Add(floorIndex, floor);
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
                    prefab = scheme.SideWindow;
                    break;
            }
            var segment = GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject;
            var itemCatcher = segment.GetComponent<ItemCatcher>();
            if (itemCatcher != null)
            {
                itemCatcher.Setup(floor);
            }
            floor.segments.Add(segment);

            // add floor middle segments:
            for (int i = 0; i < floorSegmentsCount; i++)
            {
                if (i == stairsSegmentIndex)
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
                    indexAbove = (indexAbove >= building.floors.Count) ? DoorPortal.MaxIndex : indexAbove;
                    doorPortal.floorIndexAbove = indexAbove;
                    building.stairs.Add(floorIndex, doorPortal);
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
                    prefab = scheme.SideWindow;
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


    }
}