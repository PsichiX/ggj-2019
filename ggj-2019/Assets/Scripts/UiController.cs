using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut
{
	public class UiController : MonoBehaviour
	{
		[SerializeField] private List<RectTransform> m_aims;
		[SerializeField] private Camera m_camera;
		[SerializeField] private RectTransform m_canvasRect;
		[SerializeField] private Vector2 m_arrowScaleMapFrom = new Vector2(0, 1);
		[SerializeField] private Vector2 m_arrowScaleMapTo = new Vector2(0, 1);
		[SerializeField] private GameObject pressToPickText;
		[SerializeField] private GameObject pressToThrowText;
		[SerializeField] private List<RectTransform> portalUpArrows;
		[SerializeField] private List<RectTransform> portalDownArrows;
		[SerializeField] private TextMeshProUGUI buildingCounter;
		[SerializeField] private TextMeshProUGUI pointsCounter;
		[SerializeField] private Vector3 m_portalArrowsOffset;
		[SerializeField] private GameObject youLostText;
		[SerializeField] private GameObject youWonText;
		[SerializeField] private UnityEngine.UI.Image black;
		[SerializeField] private AudioSource backgroundMusic;

		public int CurrentFloorBadEvent { get { return gameplayManager.currentFloorBadEvent; } }
		public EvecuationDirection CurrentEvecuationDirection { get { return gameplayManager.currentEvacuationDirection; } }


		private PlayerController[] m_players;
		private DoorPortal[] m_portals;
		private GameplayManager gameplayManager;
		private GameplayEvents gameplayEvents;

		public void UpdateCounter()
		{
			if (gameplayManager != null)
			{
				buildingCounter.text = gameplayManager.currentBuildingId.ToString("0");
			}
		}

		public void UpdatePoints(int pointsCount)
		{
			pointsCounter.text = pointsCount.ToString("0");
		}

		// FixMe:
		public void Update()
		{
			UpdateCounter();
			UpdatePortals();
		}

		private void Awake()
		{

			m_players = new PlayerController[m_aims.Count];
			m_portals = new DoorPortal[Mathf.Min(portalUpArrows.Count, portalDownArrows.Count)];

			DOVirtual.DelayedCall(0.1f, Attach);
		}

		private void Attach()
		{
			gameplayEvents = GameplayEvents.GetGameplayEvents();
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.GameOver, ShowGameOverText);
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.Summary, ShowWinText);
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.BadEventStart, ReactionFadeIn);
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.GameOver, ReactionFadeOut);
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.PlayerInTruck, ReactionPlayerInTruck);
			gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.FewDaysLater, ReactionFewDaysLater);

			GameplayManager.GetGameplayManager().PointsCollectedUpdate += UpdatePointsOnItemAdd;
		}

		private void ReactionPlayerInTruck(object obj)
		{
			backgroundMusic.DOFade(0, 5f).OnComplete(SetCalmMusic);
		}

		private void ReactionFewDaysLater(object obj)
		{
			backgroundMusic.DOFade(0, 5f).OnComplete(SetActionMusic);
		}

		private void SetActionMusic()
		{
			backgroundMusic.clip = Resources.Load("Sounds/katastrofa") as AudioClip;
			backgroundMusic.DOFade(1, 2f);
		}

		private void SetCalmMusic()
		{
			backgroundMusic.clip = Resources.Load("Sounds/sasiedzi") as AudioClip;
			backgroundMusic.DOFade(1, 2f);
			backgroundMusic.Play();
		}

		private void UpdatePointsOnItemAdd(int points)
		{
			UpdatePoints(points);
		}

		private void Start()
		{
			youLostText.SetActive(false);
            youWonText.SetActive(false);
            gameplayManager = GameplayManager.GetGameplayManager();

		}

		private void ReactionFadeIn(object obj)
		{
			black.DOColor(new Color(0, 0, 0, 0), 1f);
		}

		private void ReactionFadeOut(object obj)
		{
			black.DOColor(new Color(0, 0, 0, 1), 1f);
		}

		private void OnDestroy()
		{
			gameplayEvents.DetachFromEvent(GamePhases.GameplayPhase.GameOver, ShowGameOverText);
		}

		private void ShowGameOverText(object obj)
		{
			youLostText.SetActive(true);
		}

        private void ShowWinText(object obj)
        {
            youWonText.SetActive(true);
        }

        public bool RegisterPlayer(PlayerController player)
		{
			for (var i = 0; i < m_players.Length; ++i)
			{
				if (m_players[i] == null)
				{
					m_players[i] = player;
					player.CollidesWithPickable += ShowPickupText;
					player.CollidesWithPickableEnd += HidePickupText;
					player.CarryItemStart += ShowThrowText;
					player.CarryItemEnd += HideThrowText;
					return true;
				}
			}
			return false;
		}

		public bool UnregisterPlayer(PlayerController player)
		{
			for (int i = 0; i < m_players.Length; ++i)
			{
				if (m_players[i] == player)
				{
					m_aims[i].gameObject.SetActive(false);
					m_players[i] = null;
					player.CollidesWithPickable -= ShowPickupText;
					player.CollidesWithPickableEnd -= HidePickupText;
					player.CarryItemStart -= ShowThrowText;
					player.CarryItemEnd -= HideThrowText;
					return true;
				}
			}
			return false;
		}

		public bool ActivateAim(PlayerController player)
		{
			for (int i = 0; i < m_aims.Count; ++i)
			{
				if (m_players[i] == player)
				{
					m_aims[i].gameObject.SetActive(true);
					return true;
				}
			}
			return false;
		}

		public bool DeactivateAim(PlayerController player)
		{
			for (int i = 0; i < m_players.Length; ++i)
			{
				if (m_players[i] == player)
				{
					m_aims[i].gameObject.SetActive(false);
					return true;
				}
			}
			return false;
		}

		public void UpdateAim(PlayerController player, Vector3 pos, float angle, float strength)
		{
			if (m_camera == null || m_canvasRect == null)
			{
				return;
			}
			for (int i = 0; i < m_players.Length; ++i)
			{
				if (m_players[i] == player)
				{
					var sp = RectTransformUtility.WorldToScreenPoint(m_camera, pos);
					var aim = m_aims[i];
					aim.anchoredPosition = sp;
					aim.rotation = Quaternion.Euler(0, 0, angle);
					var sf = Mathf.InverseLerp(m_arrowScaleMapFrom.x, m_arrowScaleMapFrom.y, strength);
					var st = Mathf.Lerp(m_arrowScaleMapTo.x, m_arrowScaleMapTo.y, sf);
					aim.localScale = new Vector3(st, 1, 1);
				}
			}
		}

		public bool RegisterPortal(DoorPortal portal)
		{
			for (var i = 0; i < m_portals.Length; ++i)
			{
				if (m_portals[i] == null)
				{
					m_portals[i] = portal;
					return true;
				}
			}
			return false;
		}

		public bool UnregisterPortal(DoorPortal portal)
		{
			for (int i = 0; i < m_portals.Length; ++i)
			{
				if (m_portals[i] == portal)
				{
					portalUpArrows[i].gameObject.SetActive(false);
					portalDownArrows[i].gameObject.SetActive(false);
					m_portals[i] = null;
					return true;
				}
			}
			return false;
		}

		public bool ActivatePortal(DoorPortal portal)
		{
			for (int i = 0; i < m_portals.Length; ++i)
			{
				if (m_portals[i] == portal)
				{
					portalUpArrows[i].gameObject.SetActive(portal.CanGoUp);
					portalDownArrows[i].gameObject.SetActive(portal.CanGoDown);
					return true;
				}
			}
			return false;
		}

		public bool DeactivatePortal(DoorPortal portal)
		{
			for (int i = 0; i < m_portals.Length; ++i)
			{
				if (m_portals[i] == portal)
				{
					portalUpArrows[i].gameObject.SetActive(false);
					portalDownArrows[i].gameObject.SetActive(false);
					return true;
				}
			}
			return false;
		}

		private void UpdatePortals()
		{
			for (var i = 0; i < m_portals.Length; ++i)
			{
				var portal = m_portals[i];
				var up = portalUpArrows[i];
				var down = portalDownArrows[i];
				if (up.gameObject.activeSelf || down.gameObject.activeSelf)
				{
					var sp = RectTransformUtility.WorldToScreenPoint(
						m_camera,
						portal.transform.position + m_portalArrowsOffset
					);
					if (up.gameObject.activeSelf)
					{
						up.anchoredPosition = sp;
					}
					if (down.gameObject.activeSelf)
					{
						down.anchoredPosition = sp;
					}
				}
			}
		}

		public void ShowPickupText()
		{
			pressToPickText.SetActive(true);
		}

		public void HidePickupText()
		{
			pressToPickText.SetActive(false);
		}

		public void ShowThrowText()
		{
			pressToPickText.SetActive(false);
			pressToThrowText.SetActive(true);
		}

		public void HideThrowText()
		{
			pressToThrowText.SetActive(false);
		}
	}
}
