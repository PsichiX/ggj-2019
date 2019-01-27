using UnityEngine;

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
        public bool vertical;
        public ItemType type;

        private bool isAlive = true;
        public void DestroyOnGround()
        {
            isAlive = false;
        }

        public bool IsItemAlive()
        {
            return isAlive;
        }
    }
}