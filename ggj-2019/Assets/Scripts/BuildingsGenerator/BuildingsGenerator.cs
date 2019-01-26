using UnityEngine;

namespace GaryMoveOut
{
    public class BuildingsGenerator
    {
        private BuildingSegmentsDatabase buildingsDatabase;
        private ItemsDatabase itemsDatabase;


        public BuildingsGenerator()
        {
            buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
            itemsDatabase = Resources.Load<ItemsDatabase>("Databases/ItemsDatabase");
        }


        public void Destroy(ref Building building)
        {
            // to do
        }

        public Building GenerateBuilding(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex)
        {
            var building = new Building();
            var floorScheme = buildingsDatabase.GetRandomFloorScheme();

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
            var roofScheme = buildingsDatabase.GetRandomRoofScheme();
            GenerateFloor(ref building, roofScheme, root, position, root.rotation, floorSegmentsCount, stairsSegmentIndex, index, FloorType.Roof);



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
                floor = Object.Instantiate(buildingsDatabase.floorPrefab, floorParent.position, floorParent.rotation);
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

                floor.segments.Add(GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject);
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


        public void DebugGenerateItems(ref Building building)
        {
            if (building != null)
            {
                for(int i = 0; i < building.floors.Count; i++)
                {
                    int rnd = Random.Range(1, 3);
                    for(int j = 0; j < rnd; j++)
                    {
                        var item = GameObject.Instantiate<Item>(itemsDatabase.debugItem);
                        building.floors[i].AddItem(item);
                    }
                }
            }
        }
    }

}