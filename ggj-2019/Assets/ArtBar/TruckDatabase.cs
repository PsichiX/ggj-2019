using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Truck Database")]
    public class TruckDatabase : ScriptableObject
    {
        public List<GameObject> truckPrefabs;
    }
}
