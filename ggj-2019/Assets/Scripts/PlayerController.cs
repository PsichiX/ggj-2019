using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
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
        public event Action CollidesWithPortalUp;
        public event Action CollidesWithPortalUpEnd;
        public event Action CollidesWithPortalDown;
        public event Action CollidesWithPortalDownEnd;

        public int FloorIndex = 1;


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
        private BoxCollider2D m_collider;
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
        private GameplayManager m_gameplay;
        private bool m_inputBlocked = false;
        private Animator m_animator;
        private bool isCarryingItem;
        private WindowJumpSite m_window;
        private bool m_isNearPortal = false;
        private bool m_canTeleportUp = false;
        private bool m_canTeleportDown = false;
        private bool m_isAlive = true;
		private bool groundKills = false;

		private AudioSource aus;

        private void Awake()
        {
            m_ui = FindObjectOfType<UiController>();
            m_gameplayEvents = GameplayEvents.GetGameplayEvents();
            m_gameplay = GameplayManager.GetGameplayManager();
        }

        private void Start()
        {
			Setup();
        }

		public void Setup()
		{
			aus = GameObject.FindGameObjectsWithTag("Audio")[0].GetComponent<AudioSource>();
			m_rigidBody = GetComponent<Rigidbody2D>();
			m_collider = GetComponent<BoxCollider2D>();
			m_animator = GetComponentInChildren<Animator>();

			if (m_ui != null)
			{
				m_ui.RegisterPlayer(this);
			}
			if (m_gameplayEvents != null)
			{
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.Evacuation, OnEvacuation);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.PlayerJump, OnPlayerJump);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.TruckStop, OnTruckStop);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.DeEvacuation, OnDeEvacuation);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.LastItemShot, OnLastItemShot);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.PlayerDie, OnPlayerDie);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, OnEvacuationEnd);
				m_gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.GameOver, OnGameOver);
				//m_inputBlocked = true;
			}
		}

		private void OnGameOver(object obj)
		{
			OnPlayerDie(this);
        }

        private void OnPlayerDie(object obj)
        {
            if (obj is PlayerController && obj as PlayerController == this)
            {
                m_inputBlocked = true;
                InputLayout = InputHandler.Layout.None;
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (m_ui != null)
            {
                m_ui.UnregisterPlayer(this);
            }
            if (m_gameplayEvents != null)
            {
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.Evacuation, OnEvacuation);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.PlayerJump, OnPlayerJump);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.DeEvacuation, OnDeEvacuation);
				m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.TruckStop, OnTruckStop);
				m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.LastItemShot, OnLastItemShot);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.PlayerDie, OnPlayerDie);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, OnEvacuationEnd);
                m_gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.GameOver, OnGameOver);
            }
        }

        private void OnEvacuationEnd(object obj)
        {
            if (obj is int && (int)obj == FloorIndex)
            {
                m_gameplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerDie, this);
            }
        }

        private void FixedUpdate()
        {
            Velocity = 0;
            if (m_inputHandler != null && m_inputBlocked == false)
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
                    else if (m_window == null && action)
                    {
                        m_window = GetInteractible<WindowJumpSite>();
                        if (m_window != null)
                        {
                            AimMe();
                        }
                    }
                    else if (m_window != null && !action)
                    {
                        ThrowMe();
                    }
                }
                if (m_isAiming)
                {
                    if (m_inputHandler.Up)
                    {
                        m_aimStrength = Mathf.Clamp(
                            m_aimStrength + dt * m_aimingStrengthSpeed,
                            m_aimStrengthRange.x,
                            m_aimStrengthRange.y
                        );
                    }
                    else if (m_inputHandler.Down)
                    {
                        m_aimStrength = Mathf.Clamp(
                            m_aimStrength - dt * m_aimingStrengthSpeed,
                            m_aimStrengthRange.x,
                            m_aimStrengthRange.y
                        );
                    }
                    if (TurnToSide == Side.Left)
                    {
                        if (m_inputHandler.Right)
                        {
                            m_aimAngle += m_aimingAngleSpeed * dt;
                        }
                        else if (m_inputHandler.Left)
                        {
                            m_aimAngle -= m_aimingAngleSpeed * dt;
                        }
                    }
                    else
                    {
                        if (m_inputHandler.Left)
                        {
                            m_aimAngle += m_aimingAngleSpeed * dt;
                        }
                        else if (m_inputHandler.Right)
                        {
                            m_aimAngle -= m_aimingAngleSpeed * dt;
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
                            if (!PickUp() && m_isNearPortal && m_canTeleportUp)
                            {
                                TeleportUp();
                            }
                        }
                    }
                    else if (down != m_lastDown)
                    {
                        m_lastDown = down;
                        if (down)
                        {
                            if (m_isNearPortal && m_canTeleportUp)
                            {
                                TeleportDown();
                            }
                            else
                            {
                                PutDown();
                            }
                        }
                    }
                    if (m_inputHandler.Left)
                    {
                        //m_rigidBody.AddForce((Vector2.left * m_speed + Vector2.up) * dt * m_rigidBody.mass, ForceMode2D.Impulse);
                        m_rigidBody.MovePosition(m_rigidBody.position + Vector2.left * m_speed * dt);
                        Velocity = -m_speed;
                        TurnToSide = Side.Left;
                    }
                    else if (m_inputHandler.Right)
                    {
                        //m_rigidBody.AddForce((Vector2.right * m_speed + Vector2.up) * dt * m_rigidBody.mass, ForceMode2D.Impulse);
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
            else if (m_window != null && m_ui != null)
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
            if (other.tag == "Ground" && m_isAlive && groundKills == true)
            {
                m_isAlive = false;
                m_gameplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerDie, this);
                Debug.Log("player hit the ground");
            }

            if (other.tag == "TruckLoader" && m_isAlive && groundKills == true)
            {
                m_gameplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerInTruck, null);
                Debug.Log("player in truck");
				aus.PlayOneShot(Resources.Load("Sounds/gain") as AudioClip);
				Destroy(gameObject);
            }

            if (other.tag == "Interactible")
            {
                m_interactibles.Add(other.transform.parent.gameObject);
                if (isCarryingItem == false && other.GetComponent<Pickable>() != null)
                {
                    CollidesWithPickable?.Invoke();
                }
            }
            else if (other.tag == "InteractibleRoot")
            {
                m_interactibles.Add(other.gameObject);
            }
            else if (other.tag == "Portal")
            {
                var portal = other.gameObject.GetComponentInChildren<DoorPortal>();
                if (portal != null)
                {
                    m_interactibles.Add(portal.gameObject);
                    portal.Acquire();
                    m_isNearPortal = true;
                    m_canTeleportUp = portal.CanGoUp;
                    m_canTeleportDown = portal.CanGoDown;
                    if (m_canTeleportUp)
                    {
                        CollidesWithPortalUp?.Invoke();
                    }
                    if (m_canTeleportDown)
                    {
                        CollidesWithPortalDown?.Invoke();
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Interactible")
            {
                m_interactibles.Remove(other.transform.parent.gameObject);
                if (other.GetComponent<Pickable>() != null)
                {
                    CollidesWithPickableEnd?.Invoke();
                }
            }
            else if (other.tag == "InteractibleRoot")
            {
                m_interactibles.Remove(other.gameObject);
            }
            else if (other.tag == "Portal")
            {
                var portal = other.gameObject.GetComponentInChildren<DoorPortal>();
                if (portal != null)
                {
                    m_interactibles.Remove(portal.gameObject);
                    portal.Release();
                    if (m_canTeleportUp)
                    {
                        CollidesWithPortalUpEnd?.Invoke();
                    }
                    if (m_canTeleportDown)
                    {
                        CollidesWithPortalDownEnd?.Invoke();
                    }
                    m_isNearPortal = false;
                    m_canTeleportUp = false;
                    m_canTeleportDown = false;
                }
            }
        }

        private bool PickUp()
        {
            var pickable = GetInteractible<Pickable>();
            if (pickable != null)
            {
                m_pickedUp = pickable;
                m_pickedUp.PickUp(TurnToSide);
                m_animator?.SetBool("PickedUp", true);
                isCarryingItem = true;
                CarryItemStart?.Invoke();
                return true;
            }
            return false;
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

        private void TeleportUp()
        {
            var portal = GetInteractible<DoorPortal>();
            if (portal != null && portal.CanGoUp && portal.building.stairs.TryGetValue(portal.floorIndexAbove, out DoorPortal portalAbove))
            {
                var pos = portalAbove.transform.position;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);

                FloorIndex = portal.floorIndexAbove;
            }
        }

        private void TeleportDown()
        {
            var portal = GetInteractible<DoorPortal>();
            if (portal != null && portal.CanGoDown)
            {
                var portalBelow = portal.building.stairs[portal.floorIndexBelow];
                var pos = portalBelow.transform.position;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);

                FloorIndex = portal.floorIndexBelow;
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

        private void AimMe()
        {
            if (m_window != null && !m_isAiming)
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

        private void ThrowMe()
        {
			GetComponent<AudioSource>().Play();
            if (m_window != null && m_isAiming)
            {
                m_isAiming = false;
                InputLayout = InputHandler.Layout.None;
                var angle = TurnToSide == Side.Left ? 180 - m_aimAngle : m_aimAngle;
                var force = Quaternion.Euler(0, 0, angle) * Vector2.right * m_aimStrength;
                m_rigidBody.AddForce(force, ForceMode2D.Impulse);
                m_rigidBody.constraints = RigidbodyConstraints2D.None;
                m_rigidBody.MovePosition(m_rigidBody.position + new Vector2(0, 1));
                m_collider.size = new Vector2(0.5f, 0.5f);
                m_animator.SetFloat("Jump", 0.2f);
                m_window = null;
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
			groundKills = true;
            m_inputBlocked = true;
        }

		private void OnDeEvacuation(object obj)
        {
			groundKills = false;
            m_inputBlocked = false;
			Debug.Log("is input blocked? : " + m_inputBlocked);
        }

		private void OnTruckStop(object obj)
		{
			groundKills = false;
			m_inputBlocked = false;
		}

        private void OnPlayerJump(object obj)
        {
            m_inputBlocked = true;
        }

        private void OnEvacuation(object obj)
        {
			groundKills = true;
			m_inputBlocked = false;
        }
    }
}
