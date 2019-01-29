using UnityEngine;

namespace GaryMoveOut
{
    public class ItemCatcher : MonoBehaviour
    {
        [SerializeField] private bool isInside = true;
        [SerializeField] private AddForce addForce;
        [SerializeField] private GameObject glassSheet;
		private bool isBroken = false;
        private Collider coll;

        private Floor floor;

        public void Setup(Floor floor)
        {
            this.floor = floor;
        }

        // FIX ME:
        private void OnTriggerEnter2D(Collider2D other)
        {
    //        var itemScheme = other.GetComponent<ItemScheme_OLD>();
    //        if (itemScheme != null && floor != null)
    //        {
    //            var item = itemScheme.assignedItem;
    //            if (isInside)
    //            {
				//	Debug.Log("Added " + item.prefab.name + "to floor " + floor.Type);
    //                floor.AddItem_OLD(item, other.gameObject);
    //            }
    //            else
    //            {
				//	Debug.Log("Removed " + item.prefab.name + "from floor " + floor.Type);
				//	floor.RemoveItem_OLD(item);
    //            }
    //        }
    //        if (addForce != null)
    //        {
				//isBroken = true;
				//var rigid = other.GetComponent<Rigidbody2D>();
    //            if (rigid)
    //            {
    //                if (rigid.velocity.x > 0)
    //                {
    //                    YouAreTearingMeApartItem(true);
    //                }
    //                else
    //                {
    //                    YouAreTearingMeApartItem(false);
    //                }
    //            }
    //        }
        }

        private void YouAreTearingMeApartItem(bool toTheRight)
        {
            glassSheet.SetActive(false);
            addForce.gameObject.SetActive(true);
            addForce.right = toTheRight;
            addForce.Peknij();
        }
    }
}
