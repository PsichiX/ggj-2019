using UnityEngine;

namespace GaryMoveOut.Items
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items/Item Scheme")]
    public class ItemScheme : ScriptableObject
    {
        public bool isSmall;
        public float value;
        public float weight;
        public bool vertical;
        public ItemMaterialType materialType;
        public GameObject itemPrefab;
        public GameObject explosion;
    }
}
