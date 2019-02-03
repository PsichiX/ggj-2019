using UnityEngine;

namespace GaryMoveOut
{
    public class WindowCrusher : MonoBehaviour
    {
        [SerializeField] private AddForce addForce;
        [SerializeField] private GameObject glassSheet;
        public bool isBroken = false;


        private void OnTriggerStay2D(Collider2D other)
        {
            if (addForce != null && !isBroken)
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
                }
            }
        }

        private void YouAreTearingMeApartItem(bool toTheRight)
        {
            isBroken = true;
            glassSheet.SetActive(false);
            addForce.gameObject.SetActive(true);
            addForce.right = toTheRight;
            addForce.TearMeApart();
        }
    }
}