using GaryMoveOut.Items;
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

    [System.Serializable]
    public struct FloorSize
    {
        public int segmentsCount;
        public int stairsSegmentIndex;
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





        [System.Obsolete] public List<Item_OLD> items_OLD = new List<Item_OLD>();
        [System.Obsolete] public Dictionary<Item_OLD, GameObject> itemGOs_GO = new Dictionary<Item_OLD, GameObject>();

        [System.Obsolete]
        public void AddItem_OLD(Item_OLD item, GameObject itemGO)
        {
            if (!items_OLD.Contains(item))
            {
                items_OLD.Add(item);
                itemGOs_GO.Add(item, itemGO);
            }
        }

        [System.Obsolete]
        public void RemoveItem_OLD(Item_OLD item)
        {
            items_OLD.Remove(item);
            itemGOs_GO.Remove(item);
        }
    }
}