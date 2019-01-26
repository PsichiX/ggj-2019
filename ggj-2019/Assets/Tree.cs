using GaryMoveOut;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
	[SerializeField] private Animator thisAnimator;
	[SerializeField] private GameplayEvents gameplayEvents;

	private void Start()
	{
		gameplayEvents = GameplayEvents.GetGameplayEvents();
		gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.TruckStart, StartStrongSway); 
	}

	private void StartStrongSway(object param)
	{
		DG.Tweening.DOVirtual.DelayedCall(0.15f, () => Sway());
	}

	private void Sway()
	{
		thisAnimator.SetTrigger("Sway");
		gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.TruckStart, StartStrongSway);
	}
}
