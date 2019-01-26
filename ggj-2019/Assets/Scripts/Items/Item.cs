using UnityEngine;

namespace GaryMoveOut
{
    public class Item
    {
        public float value;
        public float weight;
        public ItemType type;
        public GameObject prefab;

        public Item(ItemScheme scheme)
        {
            value = scheme.value;
            weight = scheme.weight;
            type = scheme.type;
            prefab = scheme.gameObject;
        }
    }
}