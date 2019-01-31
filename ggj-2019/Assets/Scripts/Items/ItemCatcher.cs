using GaryMoveOut.Items;
using UnityEngine;

namespace GaryMoveOut
{
    public class ItemCatcher : MonoBehaviour
    {
        [SerializeField] private bool isInside = true;
        [SerializeField] private AddForce addForce;
        [SerializeField] private GameObject glassSheet;
		public bool isBroken = false;
        private Floor floor;

        public void Setup(Floor floor)
        {
            this.floor = floor;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var item = other.GetComponent<Item>();
            if (item != null && floor != null)
            {
                if (isInside)
                {
                    Debug.Log("Added " + item.name + "to floor " + floor.Type);
                    floor.AddItem(item);
                }
                else
                {
                    Debug.Log("Removed " + item.name + "from floor " + floor.Type);
                    floor.RemoveItem(item);
                }
            }
            if (addForce != null)
            {
				if (isBroken == false)
				{
					var rigid = other.GetComponent<Rigidbody2D>();
					if (rigid)
					{
						if (rigid.velocity.x > 0)
						{
							YouAreTearingMeApartItem(true);
						}
						else if (rigid.velocity.magnitude != 0)
						{
							YouAreTearingMeApartItem(false);
						}
						isBroken = true;
					}
				}
            }
        }

        private void YouAreTearingMeApartItem(bool toTheRight)
        {
            glassSheet.SetActive(false);
            addForce.gameObject.SetActive(true);
            addForce.right = toTheRight;
            addForce.TearMeApart();
        }
    }
}
