using UnityEngine;

namespace GaryMoveOut
{
    public class Item : MonoBehaviour
    {
        public float value;

        private Collider coll;
        private Bounds bounds;


        private void Start()
        {
            coll = GetComponent<Collider>();
            if (coll != null)
            {
                bounds = coll.bounds;
            }
        }
    }
}