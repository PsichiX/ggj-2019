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

            var itemsFound = Resources.LoadAll<ItemScheme>("ItemSchemes");
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

        [SerializeField] private string prefabsPath = "Resources/ItemSchemes/";
        public void GenerateItemSchemes()
        {
            var itemPrefabsFound = Resources.LoadAll("Items_NEW");
            foreach (var item in itemPrefabsFound)
            {
                var scheme = ScriptableObject.CreateInstance<ItemScheme>();
                scheme.itemPrefab = item as GameObject;
                scheme.name = item.name;
                var path = $"{prefabsPath}{scheme.name}.asset";
                UnityEditor.AssetDatabase.CreateFolder("Resources", "ItemSchemes");
                UnityEditor.AssetDatabase.CreateAsset(scheme, path);
            }
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
    }
}