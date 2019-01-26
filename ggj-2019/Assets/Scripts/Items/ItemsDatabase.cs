using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items Database")]
    public class ItemsDatabase : ScriptableObject
    {
        public ItemScheme debugItem;

        public List<ItemScheme> database = new List<ItemScheme>();
        public Dictionary<ItemType, List<ItemScheme>> itemsByType = new Dictionary<ItemType, List<ItemScheme>>();


        public void LoadItemsFromAssets()
        {
            database.Clear();
            itemsByType.Clear();

            var items = Resources.LoadAll<ItemScheme>("Items");
            foreach(var item in items)
            {
                database.Add(item);
                if (!itemsByType.ContainsKey(item.type))
                {
                    itemsByType.Add(item.type, new List<ItemScheme>());
                }
                itemsByType[item.type].Add(item);
            }
        }

        public List<ItemScheme> GetRandomItems(int count)
        {
            List<ItemScheme> items = new List<ItemScheme>();
            for(int i = 0; i < count; i++)
            {
                items.Add(GetRandomItem());
            }
            return items;
        }


        public ItemScheme GetRandomItem()
        {
            if (database.Count > 0)
            {
                return database[Random.Range(0, database.Count)];
            }
            else
            {
                return debugItem;
            }
        }

        public ItemScheme GetRandomItemByType(ItemType type)
        {
            if (itemsByType.ContainsKey(type) && itemsByType[type].Count > 0)
            {
                return itemsByType[type][Random.Range(0, itemsByType[type].Count)];
            }
            else
            {
                return debugItem;
            }
        }
    }
}