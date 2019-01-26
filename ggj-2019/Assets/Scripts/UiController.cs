using System.Collections.Generic;
using UnityEngine;

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

		private PlayerController[] m_players;

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
                    return true;
                    m_aims[i].gameObject.SetActive(true);
					m_players[i].CollidesWithPickable += ShowPickupText;
					m_players[i].CollidesWithPickableEnd += HidePickupText;
					m_players[i].CarryItemStart += ShowThrowText;
					m_players[i].CarryItemEnd += HideThrowText;
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
