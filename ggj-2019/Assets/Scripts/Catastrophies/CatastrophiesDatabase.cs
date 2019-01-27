using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies Database")]
    public class CatastrophiesDatabase : ScriptableObject
    {
        public List<BaseCatastrophy> database = new List<BaseCatastrophy>();

        public void LoadDataFromResources()
        {
            database.Clear();

            var items = Resources.LoadAll<BaseCatastrophy>("Catastrophies");
            foreach (var item in items)
            {
                database.Add(item);
            }
        }


        public BaseCatastrophy GetRandomCatastrophy()
        {
            if (database.Count > 0)
            {
                return database[Random.Range(0, database.Count)];
            }
            else
            {
                return null;
            }
        }
    }
}