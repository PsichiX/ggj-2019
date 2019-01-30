using UnityEngine;
using System.Collections;
using GaryMoveOut.Items;

namespace GaryMoveOut
{
    public class OhHiWindow : MonoBehaviour
    {
        [SerializeField] private AddForce addForce;
        [SerializeField] private GameObject glassSheet;
        private bool isBroken = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var item = other.gameObject.GetComponent<Item>();
            if (item == null)
            {
                return;
            }

            if (addForce != null)
            {
                if (isBroken == false)
                {
                    var rigid = other.gameObject.GetComponent<Rigidbody2D>();
                    if (rigid)
                    {
                        if (rigid.velocity.x > 0)
                        {
                            YouAreTearingMeApartItem(true);
                        }
                        else
                        {
                            YouAreTearingMeApartItem(false);
                        }
                    }
                    isBroken = true;
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