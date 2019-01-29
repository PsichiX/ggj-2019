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
        public Dictionary<Item, GameObject> itemGOs = new Dictionary<Item, GameObject>();


        public void AddItem(Item item, GameObject itemGO)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                itemGOs.Add(item, itemGO);
            }
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
            itemGOs.Remove(item);
        }
    }
}