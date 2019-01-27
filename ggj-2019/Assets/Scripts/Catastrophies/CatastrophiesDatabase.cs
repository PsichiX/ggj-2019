using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies Database")]
    public class CatastrophiesDatabase : ScriptableObject
    {
        public BaseCatastrophy defaultCatastrophy;

        public List<BaseCatastrophy> database = new List<BaseCatastrophy>();

        public void LoadDataFromResources()
        {
            database.Clear();

            var catastrophies = Resources.LoadAll<BaseCatastrophy>("Catastrophies");
            foreach (var catastrophy in catastrophies)
            {
                catastrophy.Initialize();
                database.Add(catastrophy);
            }
        }


        public BaseCatastrophy GetRandomCatastrophy()
        {
            //return defaultCatastrophy;

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