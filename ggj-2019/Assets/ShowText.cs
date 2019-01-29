using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GaryMoveOut;
using DG.Tweening;

public class ShowText : MonoBehaviour
{
	[SerializeField] private TextMeshPro textMesh;

	private void Start()
	{
		GameplayEvents.GetGameplayEvents().AttachToEvent(GamePhases.GameplayPhase.TruckStart, ShowIt);
	}

	private void ShowIt(object obj)
	{
		textMesh.DOFade(0.8f, 7.5f).SetEase(Ease.InExpo);
		DOVirtual.DelayedCall(8f, () => textMesh.DOFade(0, 3f));
	}
}
