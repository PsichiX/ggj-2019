using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public enum Side
        {
            Left = -1,
            Right = 1
        }

        public float Velocity { get; private set; }
        public Side TurnToSide { get; private set; }
        public InputHandler.Layout InputLayout
        {
            get { return m_inputHandler == null ? InputHandler.Layout.None : m_inputHandler.InputLayout; }
            set { if (m_inputHandler != null) m_inputHandler.InputLayout = value; }
        }
        public event Action CollidesWithPickable;
        public event Action CollidesWithPickableEnd;
        public event Action CarryItemStart;
        public event Action CarryItemEnd;

        [SerializeField]
        private InputHandler m_inputHandler;
        [SerializeField]
        private float m_speed;
        [SerializeField]
        private Transform m_pickableOrigin;
        [SerializeField]
        private Vector2 m_aimStrengthRange = new Vector2(1, 10);
        [SerializeField]
        private float m_aimingAngleSpeed = 1;
        [SerializeField]
        private float m_aimingStrengthSpeed = 1;

        private Rigidbody2D m_rigidBody;
        private HashSet<GameObject> m_interactibles = new HashSet<GameObject>();
        private bool m_lastAction = false;
        private bool m_lastUp = false;
        private bool m_lastDown = false;
        private Pickable m_pickedUp;
        private bool m_isAiming = false;
        private float m_aimAngle;
        private float m_aimStrength;
        private UiController m_ui;
        private GameplayEvents m_gameplayEvents;
        private bool m_inputBlocked = false;
        private Animator m_animator;
        private bool isCarryingItem;

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            m_ui = FindObjectOfType<UiController>();
            m_gameplayEvents = GameplayEvents.GetGameplayEvents();
            m_animator = GetComponentInChildren<Animator>();

            if (m_ui != null)
            {
                m_ui.RegisterPlayer(this);
            }
            if (m_gameplayEvents != null)
            {
                m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.Evacuation, OnEvacuation);
                m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.PlayerJump, OnPlayerJump);
                m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.DeEvacuation, OnDeEvacuation);
                m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.LastItemShot, OnLastItemShot);
                m_inputBlocked = true;
            }
        }

        private void OnDestroy()
        {
            if (m_ui != null)
            {
                m_ui.UnregisterPlayer(this);
            }
            m_ui = null;
            if (m_gameplayEvents != null)
            {
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.Evacuation, OnEvacuation);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.PlayerJump, OnPlayerJump);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.DeEvacuation, OnDeEvacuation);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.LastItemShot, OnLastItemShot);
            }
            m_gameplayEvents = null;
        }

        private void FixedUpdate()
        {
            Velocity = 0;
            if (m_inputHandler != null && !m_inputBlocked)
            {
                var dt = Time.fixedDeltaTime;
                var action = m_inputHandler.Action;
                if (action != m_lastAction)
                {
                    m_lastAction = action;
                    if (m_pickedUp != null && m_pickedUp.IsPickedUp)
                    {
                        if (action)
                        {
                            Aim();
                        }
                        else
                        {
                            Throw();
                        }
                    }
                }
                if (m_isAiming)
                {
                    if (m_inputHandler.Up)
                    {
                        m_aimAngle += m_aimingAngleSpeed * dt;
                    }
                    else if (m_inputHandler.Down)
                    {
                        m_aimAngle -= m_aimingAngleSpeed * dt;
                    }
                    if (TurnToSide == Side.Left)
                    {
                        if (m_inputHandler.Left)
                        {
                            m_aimStrength = Mathf.Clamp(
                                m_aimStrength + dt * m_aimingStrengthSpeed,
                                m_aimStrengthRange.x,
                                m_aimStrengthRange.y
                            );
                        }
                        else if (m_inputHandler.Right)
                        {
                            m_aimStrength = Mathf.Clamp(
                                m_aimStrength - dt * m_aimingStrengthSpeed,
                                m_aimStrengthRange.x,
                                m_aimStrengthRange.y
                            );
                        }
                    }
                    else
                    {
                        if (m_inputHandler.Right)
                        {
                            m_aimStrength = Mathf.Clamp(
                                m_aimStrength + dt * m_aimingStrengthSpeed,
                                m_aimStrengthRange.x,
                                m_aimStrengthRange.y
                            );
                        }
                        else if (m_inputHandler.Left)
                        {
                            m_aimStrength = Mathf.Clamp(
                                m_aimStrength - dt * m_aimingStrengthSpeed,
                                m_aimStrengthRange.x,
                                m_aimStrengthRange.y
                            );
                        }
                    }
                }
                else
                {
                    var up = m_inputHandler.Up;
                    var down = m_inputHandler.Down;
                    if (up != m_lastUp)
                    {
                        m_lastUp = up;
                        if (up)
                        {
                            PickUp();
                        }
                    }
                    else if (down != m_lastDown)
                    {
                        m_lastDown = down;
                        if (down)
                        {
                            PutDown();
                        }
                    }
                    if (m_inputHandler.Left)
                    {
                        m_rigidBody.MovePosition(m_rigidBody.position + Vector2.left * m_speed * dt);
                        Velocity = -m_speed;
                        TurnToSide = Side.Left;
                    }
                    else if (m_inputHandler.Right)
                    {
                        m_rigidBody.MovePosition(m_rigidBody.position + Vector2.right * m_speed * dt);
                        Velocity = m_speed;
                        TurnToSide = Side.Right;
                    }
                }
            }

            var handPos = m_pickableOrigin == null ? transform.position : m_pickableOrigin.position;
            if (m_pickedUp != null && m_pickedUp.IsPickedUp)
            {
                m_pickedUp.transform.position = handPos;
            }
            if (m_pickedUp != null && m_pickedUp.IsPickedUp && m_ui != null)
            {
                m_ui.UpdateAim(
                    this,
                    handPos,
                    TurnToSide == Side.Left ? 180 - m_aimAngle : m_aimAngle,
                    m_aimStrength
                );
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Interactible")
            {
                m_interactibles.Add(other.transform.parent.gameObject);
                if (isCarryingItem == false)
                {
                    CollidesWithPickable?.Invoke();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Interactible")
            {
                m_interactibles.Remove(other.transform.parent.gameObject);
                CollidesWithPickableEnd?.Invoke();
            }
        }

        private void PickUp()
        {
            var pickable = GetInteractible<Pickable>();
            if (pickable != null)
            {
                m_pickedUp = pickable;
                m_pickedUp.PickUp(TurnToSide);
                m_animator?.SetBool("PickedUp", true);
                isCarryingItem = true;
                CarryItemStart?.Invoke();

            }
        }

        private void PutDown()
        {
            if (m_pickedUp != null && !m_isAiming)
            {
                m_pickedUp.PutDown();
                m_pickedUp = null;
                m_animator?.SetBool("PickedUp", false);
                isCarryingItem = false;
                CarryItemEnd?.Invoke();
            }
        }

        private void Aim()
        {
            if (m_pickedUp != null && !m_isAiming)
            {
                m_isAiming = true;
            }
            if (m_ui != null)
            {
                m_ui.ActivateAim(this);
            }
            m_aimAngle = 0;
            m_aimStrength = m_aimStrengthRange.x;
        }

        private void Throw()
        {
            if (m_pickedUp != null && m_isAiming)
            {
                m_isAiming = false;
                var angle = TurnToSide == Side.Left ? 180 - m_aimAngle : m_aimAngle;
                var force = Quaternion.Euler(0, 0, angle) * Vector2.right * m_aimStrength;
                m_pickedUp.Throw(force);
                m_animator?.SetBool("PickedUp", false);
                isCarryingItem = false;
                CarryItemEnd?.Invoke();
                m_pickedUp = null;
            }
            if (m_ui != null)
            {
                m_ui.DeactivateAim(this);
            }
        }

        private T GetInteractible<T>() where T : MonoBehaviour
        {
            foreach (var go in m_interactibles)
            {
                var c = go.GetComponent<T>();
                if (c != null)
                {
                    return c;
                }
            }
            return null;
        }

        private void OnLastItemShot(object obj)
        {
            m_inputBlocked = true;
        }

        private void OnDeEvacuation(object obj)
        {
            m_inputBlocked = false;
        }

        private void OnPlayerJump(object obj)
        {
            m_inputBlocked = true;
        }

        private void OnEvacuation(object obj)
        {
            m_inputBlocked = false;
        }
    }
}
