using UnityEngine;
using System.Collections;

namespace GaryMoveOut
{
    public enum ItemType
    {
        None = 0,
        Fragile = 1,
        Heavy = 2,
        Fluffy = 3,
    }

    public class ItemScheme : MonoBehaviour
    {
        public float value;
        public float weight;
        public ItemType type;
    }
}