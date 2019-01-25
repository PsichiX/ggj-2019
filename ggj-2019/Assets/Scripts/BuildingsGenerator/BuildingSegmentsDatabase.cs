using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FloorScheme
{
    public GameObject EmptyWall;
    public GameObject Stairs;
    public GameObject SideWall;
    [SerializeField] private GameObject sideWallR;
    public GameObject SideWallR { get { return sideWallR ?? SideWall; } }
    [SerializeField] private GameObject sideWallL;
    public GameObject SideWallL { get { return sideWallL ?? SideWall; } }
    public GameObject SideDoor;
    public GameObject SideWindow;
}


[CreateAssetMenu(menuName = "ScriptableObjects/Building Segments Database")]
public class BuildingSegmentsDatabase : ScriptableObject
{
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
