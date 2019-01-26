using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        var scheme = buildingsDatabase.GetRandomFloorScheme();

        GenerateFloorZero(ref building, scheme, root, floorSegmentsCount, stairsSegmentIndex);

        //for (int i = 1; i < buildingFloorsCount; i++)
        {
            // to do. generate middle floors
        }

        // to do: generate roof



        Debug.Log($"Generated building with {floorSegmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {stairsSegmentIndex} floor segment");
        return building;
    }


    private void GenerateFloorZero(ref Building building, FloorScheme scheme, Transform floorRoot, int floorSegmentsCount, int stairsSegmentIndex)
    {
        if (building.floors.Count != 0)
        {
            building.floors.Clear();
        }
        int floorIndex = 0;



        Floor floor = null;
        if (!building.floors.TryGetValue(floorIndex, out floor))
        {
            floor = GameObject.Instantiate<Floor>(buildingsDatabase.floorPrefab, floorRoot.position, floorRoot.rotation);
            building.floors.Add(floorIndex, floor);
        }

        // add door on the right side:
        floor.segments.Add(GameObject.Instantiate(scheme.SideDoor, floorRoot.position, floorRoot.rotation, floor.transform) as GameObject);

        // add floor middle segments:
        for (int i = 0; i < floorSegmentsCount; i++)
        {
            if (i == stairsSegmentIndex)
            {
                floor.segments.Add(GameObject.Instantiate(scheme.Stairs, floorRoot.position, floorRoot.rotation, floor.transform) as GameObject);
            }
            else
            {
                floor.segments.Add(GameObject.Instantiate(scheme.EmptyWall, floorRoot.position, floorRoot.rotation, floor.transform) as GameObject);
            }

            floorRoot.position += new Vector3(scheme.segmentWidth, 0f, 0f);
        }

        // add empty wall OR window on the left side:
        int rnd = Random.Range(0, 2);
        switch (rnd)
        {
            // empty side wall
            case 0:
                floor.segments.Add(GameObject.Instantiate(scheme.SideWall, floorRoot.position, floorRoot.rotation, floor.transform) as GameObject);
                break;
            // side window:
            default:
                floor.segments.Add(GameObject.Instantiate(scheme.SideWindow, floorRoot.position, floorRoot.rotation, floor.transform) as GameObject);
                break;
        }
    }




    private void GenerateFloor(FloorScheme scheme, Transform floorRoot, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex)
    {
        Vector3 currPosition = floorRoot.position;
    }
}
