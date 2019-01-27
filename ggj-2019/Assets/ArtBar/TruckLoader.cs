using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class TruckLoader : MonoBehaviour
    {
        public List<ItemScheme> itemsInTruck;

        void Start()
        {
            itemsInTruck = new List<ItemScheme>();
        }

        public void ResetTruckItemList()
        {
            itemsInTruck.Clear();
        }

        public List<ItemScheme> GetItemList()
        {
            return itemsInTruck;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var obj = other.gameObject;
            var itemScheme = obj.transform.parent.GetComponent<ItemScheme>();
            if(itemScheme != null)
            {
                itemsInTruck.Add(itemScheme);
                itemScheme.InTruck();
            }
        }
    }
}