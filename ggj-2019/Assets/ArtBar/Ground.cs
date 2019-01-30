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
			var obj = other.gameObject;
			var item = obj.transform.parent.GetComponent<Item>();
			if (item != null)
			{
				if (item.IsAlive)
				{
					if (item.GetComponent<Rigidbody2D>().velocity.magnitude > 1f)
					{
						Debug.Log("item hit the ground");
						item.DestroyOnGround();
					}
				}
			}
		}
	}
}