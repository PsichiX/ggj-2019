using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class Ground : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (other.tag == "Interactible")
            {

                var obj = other.gameObject;
                var itemScheme = obj.transform.parent.GetComponent<ItemScheme>();
                if (itemScheme != null)
                {
                    //itemsInTruck.Add(itemScheme);
                    Debug.Log("item hit the ground");
                }

                var playerController = obj.transform.parent.GetComponent<PlayerController>();
                if(playerController != null)
                {
                    Debug.Log("player hit the ground");
                }
            }
        }
    }
}