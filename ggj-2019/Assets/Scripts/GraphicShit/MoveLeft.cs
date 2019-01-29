using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveLeft : MonoBehaviour
{
	[SerializeField] private float xValue = -100f;
	[SerializeField] private float duration = 10f;
	private Vector3 startingPosition;

	private void Start()
	{
		startingPosition = transform.position;
		StartTween();
	}

	private void GetBack()
	{
		transform.position = startingPosition;
		StartTween();
	}

	private void StartTween()
	{
		transform.DOMoveX(transform.position.x + xValue, duration)
			.SetEase(Ease.Linear)
			.OnComplete(() => GetBack());
	}
}
