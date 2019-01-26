using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Items Database")]
    public class ItemsDatabase : ScriptableObject
    {
        public Item debugItem;

        public List<Item> database;
    }
}