using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class TruckLoader : MonoBehaviour
    {
		[SerializeField] private MeshRenderer meshToColor;
        public List<ItemScheme> itemsInTruck;

        void Start()
        {
			meshToColor.material.color = Random.ColorHSV(0, 1, 0.7f, 1f);
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
            if (obj != null)
            {
                if (obj.transform.parent != null)
                {
                    var itemScheme = obj.transform.parent.GetComponent<ItemScheme>();
					if (itemsInTruck.Contains(itemScheme) == false)
					{
						if (itemScheme != null)
						{
							itemsInTruck.Add(itemScheme);
							itemScheme.InTruck();
						}
					}
                }
            }
        }
    }
}