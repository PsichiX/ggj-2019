using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut.Catastrophies
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies Database")]
    public class CatastrophiesDatabase : ScriptableObject
    {
        public BaseCatastrophy defaultCatastrophy;
        public bool alwaysUseDefault = false;

        public List<BaseCatastrophy> database = new List<BaseCatastrophy>();

        public void LoadDataFromResources()
        {
            database.Clear();

            var catastrophies = Resources.LoadAll<BaseCatastrophy>("Catastrophies");
            foreach (var catastrophy in catastrophies)
            {
                database.Add(catastrophy);
            }
        }


        public BaseCatastrophy GetRandomCatastrophy()
        {
            if (alwaysUseDefault)
            {
                return defaultCatastrophy;
            }
            else
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
}