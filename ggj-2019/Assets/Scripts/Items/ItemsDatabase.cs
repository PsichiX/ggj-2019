using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut.Items
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items/Items Database")]
    public class ItemsDatabase : ScriptableObject
    {
        public List<ItemScheme> items = new List<ItemScheme>();
        public Dictionary<ItemMaterialType, List<ItemScheme>> itemsByMaterialType = new Dictionary<ItemMaterialType, List<ItemScheme>>();

        public void RefreshDatabase()
        {
            items.Clear();
            itemsByMaterialType.Clear();

            var itemsFound = Resources.LoadAll<ItemScheme>("ItemSchemas");
            foreach(var item in itemsFound)
            {
                items.Add(item);
                if (!itemsByMaterialType.ContainsKey(item.materialType))
                {
                    itemsByMaterialType.Add(item.materialType, new List<ItemScheme>());
                }
                if (!itemsByMaterialType[item.materialType].Contains(item))
                {
                    itemsByMaterialType[item.materialType].Add(item);
                }
            }
        }

        [SerializeField] private string prefabsPath;
        public void GenerateItemSchemas()
        {
            // to do
            // ...
        }

        public ItemScheme GetRandomItem()
        {
            if (items.Count == 0)
            {
                return null;
            }
            else
            {
                return items[UnityEngine.Random.Range(0, items.Count)];
            }
        }

        public List<ItemScheme> GetRandomItems(int itemsCount)
        {
            if (items.Count == 0)
            {
                return null;
            }
            else
            {
                List<ItemScheme> list = new List<ItemScheme>();
                for(int i = 0; i < itemsCount; i++)
                {
                    list.Add(GetRandomItem());
                }
                return list;
            }
        }

        public ItemScheme GetRandomItem(ItemMaterialType materialType)
        {
            if (itemsByMaterialType.TryGetValue(materialType, out List<ItemScheme> itemList))
            {
                if (itemList.Count == 0)
                {
                    return null;
                }
                else
                {
                    return itemList[UnityEngine.Random.Range(0, itemList.Count)];
                }
            }
            else
            {
                return null;
            }
        }





        #region OLD
        public ItemScheme_OLD debugItem_OLD;

        public List<ItemScheme_OLD> database_OLD = new List<ItemScheme_OLD>();
        public Dictionary<ItemType, List<ItemScheme_OLD>> itemsByType_OLD = new Dictionary<ItemType, List<ItemScheme_OLD>>();


        [System.Obsolete]
        public void LoadItemsFromAssets_OLD()
        {
            database_OLD.Clear();
            itemsByType_OLD.Clear();

            var items = Resources.LoadAll<ItemScheme_OLD>("Items");
            foreach (var item in items)
            {
                database_OLD.Add(item);
                if (!itemsByType_OLD.ContainsKey(item.type))
                {
                    itemsByType_OLD.Add(item.type, new List<ItemScheme_OLD>());
                }
                itemsByType_OLD[item.type].Add(item);
            }
        }

        [System.Obsolete]
        public List<ItemScheme_OLD> GetRandomItems_OLD(int count)
        {
            List<ItemScheme_OLD> items = new List<ItemScheme_OLD>();
            for (int i = 0; i < count; i++)
            {
                items.Add(GetRandomItem_OLD());
            }
            return items;
        }


        [System.Obsolete]
        public ItemScheme_OLD GetRandomItem_OLD()
        {
            if (database_OLD.Count > 0)
            {
                return database_OLD[Random.Range(0, database_OLD.Count)];
            }
            else
            {
                return debugItem_OLD;
            }
        }

        [System.Obsolete]
        public ItemScheme_OLD GetRandomItemByType_OLD(ItemType type)
        {
            if (itemsByType_OLD.ContainsKey(type) && itemsByType_OLD[type].Count > 0)
            {
                return itemsByType_OLD[type][Random.Range(0, itemsByType_OLD[type].Count)];
            }
            else
            {
                return debugItem_OLD;
            }
        }
        #endregion
    }
}