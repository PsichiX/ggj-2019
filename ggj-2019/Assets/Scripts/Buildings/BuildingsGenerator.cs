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


        public void DestroyBuilding(ref Building buildingOut)
        {
            foreach(var floor in buildingOut.Floors)
            {
                buildingOut.DestroyFloor(floor.Key);
                GameObject.Destroy(floor.Value.gameObject);
            }
            buildingOut.Floors.Clear();
        }


        #region Building generator
        public Building GenerateBuilding(Transform root, int buildingFloorsCount, FloorSize floorSize, bool isBuildingIn,  List<ItemScheme> items = null)
        {
            var building = ConstructBuilding(root, buildingFloorsCount, floorSize, isBuildingIn);
            if (items != null)
            {
                building.SpawnItemsInside(items);
            }
            Debug.Log($"Generated building with {floorSize.segmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {floorSize.stairsSegmentIndex} floor segment");
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
            var building = new Building(ItemsSpawner, segmentSize)
            {
                root = root
            };

            Vector3 position = root.position;
            Quaternion rotation = root.rotation;

            GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSize, 0, FloorType.GroundFloor, isBuildingIn);

            int index = 1;
            for (; index <= buildingFloorsCount; index++)
            {
                position += new Vector3(0f, floorScheme.segmentHeight, 0f);
                GenerateFloor(ref building, floorScheme, root, position, root.rotation, floorSize, index, FloorType.MiddleFloor, isBuildingIn);
            }

            position += new Vector3(0f, floorScheme.segmentHeight, 0f);
            var roofScheme = BuildingsDatabase.GetRandomRoofScheme();
            GenerateFloor(ref building, roofScheme, root, position, root.rotation, floorSize, index, FloorType.Roof, isBuildingIn);

            return building;
        }

        private void GenerateFloor(ref Building building,
                                   FloorScheme scheme,
                                   Transform parent,
                                   Vector3 position,
                                   Quaternion rotation,
                                   FloorSize floorSize,
                                   int floorIndex,
                                   FloorType type,
                                   bool isBuildingIn)
        {
            if (!building.Floors.TryGetValue(floorIndex, out Floor floor))
            {
                floor = Object.Instantiate(BuildingsDatabase.floorPrefab, parent.position, parent.rotation);
                building.Floors.Add(floorIndex, floor);
            }
            floor.Type = type;
            floor.transform.SetParent(parent);
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
            floor.segments.Add(segment);

            // generate ItemCatcher game object:
            if (type != FloorType.Roof)
            {
                var floorWidth = Vector3.Distance(floor.segments[0].transform.position, floor.segments[floor.segments.Count - 2].transform.position);
                var widthRation = isBuildingIn ? 0.9f : 1.2f;
                var realFloorSize = new Vector3((floorWidth + scheme.segmentWidth) * widthRation, scheme.segmentHeight * 0.8f);
                var center = new Vector3(floor.transform.position.x + floorWidth * 0.5f, position.y + scheme.segmentHeight * 0.5f);
                var itemCatcher = new GameObject("ItemCatcher").AddComponent<ItemCatcher>();
                itemCatcher.gameObject.transform.SetParent(floor.gameObject.transform);
                itemCatcher.Setup(center, realFloorSize, floor);
            }
        }
        #endregion
    }
}