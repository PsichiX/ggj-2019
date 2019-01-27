using UnityEngine;

namespace GaryMoveOut
{
    public enum ItemType
    {
        None = 0,
        Fragile = 1,
        Heavy = 2,
        Fluffy = 3,
    }

    public class ItemScheme : MonoBehaviour
    {
        public float value;
        public float weight;
        public bool vertical;
        public ItemType type;
        public Item assignedItem;

        private bool isAlive = true;
        public void DestroyOnGround()
        {
            isAlive = false;
            // TODO: destroy viz
        }

        public void InTruck()
        {
            // TODO: hide object
        }

        public bool IsItemAlive()
        {
            return isAlive;
        }
    }
}