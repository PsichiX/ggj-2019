using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
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
        public List<Item> items = new List<Item>();


        public void AddItem(Item item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }
    }
}