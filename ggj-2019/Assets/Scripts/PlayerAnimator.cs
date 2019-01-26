using DG.Tweening;
using GaryMoveOut;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	[SerializeField]
	private PlayerController m_player;

	private Animator thisAnimator;

	private const string forwardString = "Forward";
	private const string turnString = "Turn";

	private float speedModifier = 0.5f;
	private float m_lastVelocity = 0;

	private void Start()
	{
		thisAnimator = GetComponentInChildren<Animator>();
		m_player = GetComponent<PlayerController>();
	}

	private void Update()
	{
		var vel = m_player == null ? 0 : m_player.Velocity;
		var vs = vel > 0 ? 1 : (vel < 0 ? -1 : 0);
		var lvs = m_lastVelocity > 0 ? 1 : (m_lastVelocity < 0 ? -1 : 0);
		thisAnimator.SetFloat(forwardString, Mathf.Abs(vel) * speedModifier);
		if (vs != lvs)
		{
			if (vel < 0)
			{
				transform.GetChild(0).DORotate(new Vector3(0, -90, 0), 0.25f);
			}
			else if (vel > 0)
			{
				transform.GetChild(0).DORotate(new Vector3(0, 90, 0), 0.25f);
			}
		}
		m_lastVelocity = vel;
	}
}
