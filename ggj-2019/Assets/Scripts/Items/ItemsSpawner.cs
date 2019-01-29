using UnityEngine;

namespace GaryMoveOut.Items
{
    public class ItemsSpawner
    {
        public ItemsDatabase ItemsDatabase { get; private set; }

        public ItemsSpawner()
        {
            ItemsDatabase = Resources.Load<ItemsDatabase>("Databases/ItemsDatabase");
        }


        public GameObject SpawnRandomItem()
        {
            var scheme = ItemsDatabase.GetRandomItem();
            return SpawnItem(scheme, null);
        }

        public GameObject SpawnRandomItem(ItemMaterialType materialType)
        {
            var scheme = ItemsDatabase.GetRandomItem(materialType);
            return SpawnItem(scheme, null);
        }

        public GameObject SpawnItem(ItemScheme scheme, Transform parent)
        {
            GameObject itemGO = null;
            if (scheme != null)
            {
                itemGO = GameObject.Instantiate(scheme.itemPrefab, parent);
                if (itemGO != null)
                {
                    var item = itemGO.GetComponent<Item>();
                    if (item != null)
                    {
                        item.Setup(scheme);
                    }
                }
            }
            return itemGO;
        }
    }
}