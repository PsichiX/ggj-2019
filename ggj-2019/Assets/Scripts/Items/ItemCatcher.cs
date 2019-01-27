using UnityEngine;
using System.Collections;

namespace GaryMoveOut
{
    public class ItemCatcher : MonoBehaviour
    {
        [SerializeField] private bool isInside = true;
		[SerializeField] private AddForce addForce;
		[SerializeField] private GameObject glassSheet;
		private Collider coll;

        private Floor floor;

        public void Setup(Floor floor)
        {
            this.floor = floor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var item = other.GetComponent<ItemScheme>();
			var item2 = new Item(item);
            if (item != null && floor != null)
            {
                if (isInside)
                {
                    floor.AddItem(item2);
                }
                else
                {
                    floor.RemoveItem(item2);
                }
            }
			if (addForce != null)
			{
				var rigid = other.GetComponent<Rigidbody2D>();
				if (rigid)
				{
					if (rigid.velocity.x > 0)
					{
						Pekaj(true);
					}
					else
					{
						Pekaj(false);
					}
				}
			}
        }

		private void Pekaj(bool wPrawo)
		{
			glassSheet.SetActive(false);
			addForce.gameObject.SetActive(true);
			addForce.right = wPrawo;
			addForce.Peknij();
		}
    }
}