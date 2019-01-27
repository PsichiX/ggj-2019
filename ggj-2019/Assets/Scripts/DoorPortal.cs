using UnityEngine;

namespace GaryMoveOut
{
    public class DoorPortal : MonoBehaviour
    {
        public const int MaxIndex = 999;
        public const int MinIndex = -999;

        public bool CanGoUp
        {
            get
            {
                if (floorIndexAbove > MinIndex && floorIndexAbove < MaxIndex)
                {
                    if (m_gameplay != null && m_gameplay.currentEvacuationDirection == GameplayManager.EvecuationDirection.Down)
                    {
                        return floorIndexAbove < m_gameplay.currentFloorBadEvent;
                    }
                    return true;
                }
                return false;
            }
        }
        public bool CanGoDown
        {
            get
            {
                if (floorIndexBelow > MinIndex && floorIndexBelow < MaxIndex)
                {
                    if (m_gameplay != null && m_gameplay.currentEvacuationDirection == GameplayManager.EvecuationDirection.Up)
                    {
                        return floorIndexBelow > m_gameplay.currentFloorBadEvent;
                    }
                    return true;
                }
                return false;
            }
        }

        public Building building;
        public int floorIndexBelow;
        public int floorIndex;
        public int floorIndexAbove;

        private UiController m_ui;
        private GameplayManager m_gameplay;
        private int m_usersCount = 0;

        public void Acquire()
        {
            ++m_usersCount;
            if (m_ui != null)
            {
                m_ui.ActivatePortal(this);
            }
        }

        public void Release()
        {
            --m_usersCount;
            if (m_ui != null && m_usersCount <= 0)
            {
                m_ui.DeactivatePortal(this);
            }
        }

        private void Awake()
        {
            m_ui = FindObjectOfType<UiController>();
            if (m_ui != null)
            {
                m_ui.RegisterPortal(this);
            }
            m_gameplay = GameplayManager.GetGameplayManager();
        }

        private void OnDestroy()
        {
            if (m_ui != null)
            {
                m_ui.UnregisterPortal(this);
            }
        }
    }
}