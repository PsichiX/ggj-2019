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
    private PlayerController.Side m_lastSide;

    private void Start()
    {
        thisAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        var vel = m_player == null ? 0 : m_player.Velocity;
        thisAnimator.SetFloat(forwardString, Mathf.Abs(vel) * speedModifier);
        if (m_player.TurnToSide != m_lastSide)
        {
            if (m_player.TurnToSide == PlayerController.Side.Left)
            {
                transform.GetChild(0).DORotate(new Vector3(0, -90, 0), 0.25f);
            }
            else
            {
                transform.GetChild(0).DORotate(new Vector3(0, 90, 0), 0.25f);
            }
        }
        m_lastSide = m_player.TurnToSide;
    }
}
