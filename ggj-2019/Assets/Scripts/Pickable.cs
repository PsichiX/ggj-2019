using DG.Tweening;
using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(ItemScheme))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pickable : MonoBehaviour
    {
        private const float PICKUP_ANIM_TIME = 1 / 3;

        public bool IsPickedUp => m_rigidBody.bodyType == RigidbodyType2D.Kinematic;
        public ItemScheme Scheme { get; private set; }

        private Rigidbody2D m_rigidBody;
        private Tweener m_currentTweener;

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
            Scheme = GetComponent<ItemScheme>();
        }

        public void PickUp(PlayerController.Side side)
        {
            m_rigidBody.bodyType = RigidbodyType2D.Kinematic;
            gameObject.layer = LayerMask.NameToLayer("FurnitureInUse");
            if (Scheme.vertical)
            {
                m_currentTweener = transform.DORotate(new Vector3(0, 0, 90), PICKUP_ANIM_TIME);
            }
        }

        public void PutDown()
        {
            m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.layer = LayerMask.NameToLayer("Furniture");
            if (m_currentTweener != null)
            {
                m_currentTweener.Kill();
                m_currentTweener = null;
            }
        }

        public void Throw(Vector2 impulse)
        {
            m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
            m_rigidBody.AddForce(impulse, ForceMode2D.Impulse);
            gameObject.layer = LayerMask.NameToLayer("Furniture");
            if (m_currentTweener != null)
            {
                m_currentTweener.Kill();
                m_currentTweener = null;
            }
        }
    }
}
