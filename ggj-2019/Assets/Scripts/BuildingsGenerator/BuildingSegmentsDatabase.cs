using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Building Segments Database")]
public class BuildingSegmentsDatabase : ScriptableObject
{
    public Floor floorPrefab;

    public FloorScheme defaultFloor;
    public FloorScheme defaultRoof;

    public List<FloorScheme> floorsSegments = new List<FloorScheme>();
    public List<FloorScheme> roofsSegments = new List<FloorScheme>();


    public FloorScheme GetRandomFloorScheme()
    {
        if (floorsSegments == null || floorsSegments.Count == 0)
        {
            return defaultFloor;
        }
        else
        {
            int index = Random.Range(0, floorsSegments.Count);
            return floorsSegments[index];
        }
    }
    public FloorScheme GetRandomRoofScheme()
    {
        if (roofsSegments == null || roofsSegments.Count == 0)
        {
            return defaultRoof;
        }
        else
        {
            int index = Random.Range(0, roofsSegments.Count);
            return roofsSegments[index];
        }
    }
}
