using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        [SerializeField] private GameObject portalUpArrow;
        [SerializeField] private GameObject portalDownArrow;
        [SerializeField] private GameplayManager gameplayManager;
		[SerializeField] private TextMeshProUGUI buildingCounter;
		[SerializeField] private TextMeshProUGUI pointsCounter;

        public int CurrentFloorBadEvent { get { return gameplayManager.currentFloorBadEvent; } }
        public EvecuationDirection CurrentEvecuationDirection { get { return gameplayManager.currentEvacuationDirection; } }


        private PlayerController[] m_players;

		public void UpdateCounter()
		{
			buildingCounter.text = gameplayManager.currentBuildingId.ToString("0");
		}

		public void UpdatePoints(int pointsCount)
		{
			pointsCounter.text = pointsCount.ToString("0");
		}

		// FixMe:
		public void Update()
		{
			UpdateCounter();
		}

        private void Awake()
        {
            m_players = new PlayerController[m_aims.Count];
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
                    player.CollidesWithPortalUp += ShowPortalUpText;
                    player.CollidesWithPortalUpEnd += HidePortalUpText;
                    player.CollidesWithPortalDown += ShowPortalDownText;
                    player.CollidesWithPortalDownEnd += HidePortalDownText;
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
                    m_players[i] = null;
                    player.CollidesWithPickable -= ShowPickupText;
                    player.CollidesWithPickableEnd -= HidePickupText;
                    player.CarryItemStart -= ShowThrowText;
                    player.CarryItemEnd -= HideThrowText;
                    player.CollidesWithPortalUp -= ShowPortalUpText;
                    player.CollidesWithPortalUpEnd -= HidePortalUpText;
                    player.CollidesWithPortalDown -= ShowPortalDownText;
                    player.CollidesWithPortalDownEnd -= HidePortalDownText;
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

        public void ShowPickupText()
        {
            pressToPickText.SetActive(true);
        }

        public void HidePickupText()
        {
            pressToPickText.SetActive(false);
        }

        private float arrowXOffset = 1f;
        private float arrowYOffset = 0.5f;
        public void SetupPortalUpArrow(Transform portal)
        {
            var position = portalUpArrow.transform.position;
            position = new Vector3(portal.position.x + arrowXOffset, portal.position.y + arrowYOffset, position.z);
            portalUpArrow.transform.position = position;
        }

        public void SetupPortalDownArrow(Transform portal)
        {
            var position = portalDownArrow.transform.position;
            position = new Vector3(portal.position.x + arrowXOffset, portal.position.y - arrowYOffset, position.z);
            portalDownArrow.transform.position = position;
        }

        public void ShowPortalUpText()
        {
            portalUpArrow.SetActive(true);
        }

        public void HidePortalUpText()
        {
            portalUpArrow.SetActive(false);
        }

        public void ShowPortalDownText()
        {
            portalDownArrow.SetActive(true);
        }

        public void HidePortalDownText()
        {
            portalDownArrow.SetActive(false);
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
