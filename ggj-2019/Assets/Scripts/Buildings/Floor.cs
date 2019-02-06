using GaryMoveOut.Items;
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


        public bool AddItem(Item item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                item.transform.SetParent(this.transform);
                GameplayManager.GetGameplayManager().SetItemInBuilding(item);
                return true;
            }
            return false;
        }

        public bool RemoveItem(Item item)
        {
            bool success = items.Remove(item);
            if (success)
            {
                item.transform.SetParent(null);
            }
            return success;
        }
    }
}