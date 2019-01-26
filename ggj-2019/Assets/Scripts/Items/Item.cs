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
            value = scheme.value;
            weight = scheme.weight;
            vertical = scheme.vertical;
            type = scheme.type;
            prefab = scheme.gameObject;
        }
    }
}