using System.Collections.Generic;
using UnityEngine;

public enum FloorType
{
    GroundFloor = 0,
    MiddleFloor = 1,
    Roof = 999
}

public class Floor : MonoBehaviour
{
    public FloorType Type;
    public List<GameObject> segments = new List<GameObject>();
}
