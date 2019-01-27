using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class TruckLoader : MonoBehaviour
    {
        public List<ItemScheme> itemsInTruck;
        public List<Item> itemsInTruck2;

        void Start()
        {
            itemsInTruck = new List<ItemScheme>();
            itemsInTruck2 = new List<Item>();
        }

        public void ResetTruckItemList()
        {
            itemsInTruck.Clear();
            itemsInTruck2.Clear();
        }

        public List<ItemScheme> GetItemList()
        {
            return itemsInTruck;
        }
        public List<Item> GetItemList2()
        {
            return itemsInTruck2;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var obj = other.gameObject;
            if (obj != null)
            {
                if (obj.transform.parent != null)
                {
                    var itemScheme = obj.transform.parent.GetComponent<ItemScheme>();
                    if (itemScheme != null)
                    {
                        itemsInTruck.Add(itemScheme);
                        //itemsInTruck2.Add(new Item(itemScheme.assignedItem));
                        itemScheme.InTruck();

                    }
                }
            }
        }
    }
}