using UnityEngine;

public class BuildingsGenerator
{
    private BuildingSegmentsDatabase buildingsDatabase;


    public BuildingsGenerator()
    {
        buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
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
        Floor floor = null;
        if (!building.floors.TryGetValue(floorIndex, out floor))
        {
            floor = GameObject.Instantiate<Floor>(buildingsDatabase.floorPrefab, floorParent.position, floorParent.rotation);
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
        floor.segments.Add(GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject);

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
        floor.segments.Add(GameObject.Instantiate(prefab, position, rotation, floor.transform) as GameObject);
    }
}
