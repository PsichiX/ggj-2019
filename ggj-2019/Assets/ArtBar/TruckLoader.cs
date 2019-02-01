using GaryMoveOut.Items;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class TruckLoader : MonoBehaviour
    {
		[SerializeField] private MeshRenderer meshToColor;
        public List<Item> itemsInTruck;

        void Start()
        {
			meshToColor.material.color = Random.ColorHSV(0, 1, 0.7f, 1f);
			itemsInTruck = new List<Item>();
        }

        public void ResetTruckItemList()
        {
			foreach (var item in itemsInTruck)
			{
				Destroy(item.gameObject);
			}
			itemsInTruck.Clear();
		}

        public List<Item> GetItemList()
        {
            return itemsInTruck;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var obj = other.gameObject;
            if (obj != null)
            {
                if (obj.transform.parent != null)
                {
                    var item = obj.transform.parent.GetComponent<Item>();
					if (item != null && !itemsInTruck.Contains(item))
                    {
                        itemsInTruck.Add(item);
                        item.InTruck();
                    }
                }
            }
        }
    }
}