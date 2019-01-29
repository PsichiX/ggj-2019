using UnityEngine;

namespace GaryMoveOut.Items
{
    public class ItemsGenerator
    {
        private ItemsDatabase itemsDatabase;

        public ItemsGenerator()
        {
            itemsDatabase = Resources.Load<ItemsDatabase>("Databases/ItemsDatabase");
        }


        public GameObject SpawnRandomItem()
        {
            var scheme = itemsDatabase.GetRandomItem();
            return SpawnItem(scheme);
        }

        public GameObject SpawnRandomItem(ItemMaterialType materialType)
        {
            var scheme = itemsDatabase.GetRandomItem(materialType);
            return SpawnItem(scheme);
        }

        private GameObject SpawnItem(ItemScheme scheme)
        {
            GameObject itemGO = null;
            if (scheme != null)
            {
                itemGO = GameObject.Instantiate(scheme.itemPrefab);
            }
            return itemGO;
        }
    }
}