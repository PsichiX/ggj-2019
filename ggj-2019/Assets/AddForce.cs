using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
	[SerializeField] private Rigidbody[] shards;
	public bool right;

	public void Peknij()
	{
		if (right)
		{
			foreach (var s in shards)
			{
				s.AddForce(Random.Range(80,250), Random.Range(8, 10), 0);
			}
		}
		else
		{
			foreach (var s in shards)
			{
				s.AddForce(Random.Range(-80, -250), Random.Range(8, 10), 0);
			}
		}
		Destroy(gameObject, 5f);
	}
}
