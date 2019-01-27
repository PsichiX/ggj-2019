using UnityEngine;

namespace GaryMoveOut
{
    public class Item
    {
        public float value;
        public float weight;
        public bool vertical;
        public ItemType type;
        public GameObject prefab;

        public Item(ItemScheme scheme)
        {
            if (scheme == null)
            {
                Debug.LogError("Could not create Item out of empty ITemScheme!");
                return;
            }
            value = scheme.value;
            weight = scheme.weight;
            vertical = scheme.vertical;
            type = scheme.type;
            prefab = scheme.gameObject;

            scheme.assignedItem = this;
        }
    }
}