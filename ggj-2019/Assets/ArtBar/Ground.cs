using GaryMoveOut.Items;
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
                if (obj != null && obj.transform.parent != null)
                {
                    var item = obj.transform.parent.GetComponent<Item>();
                    if (item != null)
                    {
                        if (item.IsAlive)
                        {
                            var rb = obj.transform.parent.GetComponent<Rigidbody2D>();
                            if (rb != null)
                            {
                                if (rb.velocity.magnitude > 1f)
                                {
                                    Debug.Log("item hit the ground");
                                    item.DestroyOnGround();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}