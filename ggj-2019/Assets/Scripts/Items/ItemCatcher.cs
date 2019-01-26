using UnityEngine;
using System.Collections;

namespace GaryMoveOut
{
    public class ItemCatcher : MonoBehaviour
    {
        [SerializeField] private bool isInside = true;
        private Collider coll;

        private Floor floor;

        public void Setup(Floor floor)
        {
            this.floor = floor;
        }

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<Item>();
            if (item != null && floor != null)
            {
                if (isInside)
                {
                    floor.AddItem(item);
                }
                else
                {
                    floor.RemoveItem(item);
                }
            }
        }
    }
}