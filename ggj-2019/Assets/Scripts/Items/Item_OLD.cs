using UnityEngine;

namespace GaryMoveOut
{
    [System.Obsolete]
    public class Item_OLD
    {
        public float value;
        public float weight;
        public bool vertical;
        public ItemType type;
        public GameObject prefab;

        public Item_OLD(ItemScheme_OLD scheme)
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

        public Item_OLD(Item_OLD scheme)
        {
            if (scheme == null)
            {
                Debug.LogError("Could not create Item out of empty Item!");
                return;
            }
            value = scheme.value;
            weight = scheme.weight;
            vertical = scheme.vertical;
            type = scheme.type;
            prefab = scheme.prefab;
        }
    }
}