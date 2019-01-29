using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
	[SerializeField] private float timeToDestroy = 0.5f;
	private void OnEnable()
	{
		Destroy(gameObject, timeToDestroy);
	}
}
