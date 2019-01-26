using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Building
{
    public Dictionary<int, Floor> floors;
    public List<DoorPortal> stairs;


    public Building()
    {
        floors = new Dictionary<int, Floor>();
        stairs = new List<DoorPortal>();
    }
}
