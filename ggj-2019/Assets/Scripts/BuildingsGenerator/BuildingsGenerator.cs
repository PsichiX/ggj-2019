using UnityEngine;
using System.Collections;


public class BuildingsGenerator
{
    private BuildingSegmentsDatabase buildingsDatabase;


    public BuildingsGenerator()
    {
        buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
    }



    public void GenerateBuilding(Transform root, int floorSegmentsCount, int buildingFloorsCount, int stairsSegmentIndex)
    {
        Debug.Log($"Generated building with {floorSegmentsCount} segments width, {buildingFloorsCount} floors and stairs at each {stairsSegmentIndex} floor segment");
    }
}
