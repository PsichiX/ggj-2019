using DG.Tweening;
using GaryMoveOut.Items;
using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pickable : MonoBehaviour
    {
        private const float PICKUP_ANIM_TIME = 1 / 3;

        public bool IsPickedUp => m_rigidBody.bodyType == RigidbodyType2D.Kinematic;

        private Item item;
        private Rigidbody2D m_rigidBody;
        private Tweener m_currentTweener;

        private void Start()
        {
			InvokeRepeating("Tick", 10f, 1f);
            m_rigidBody = GetComponent<Rigidbody2D>();
            item = GetComponent<Item>();
        }

        public void PickUp(PlayerController.Side side)
        {
			item.cantKillMe = true;
            m_rigidBody.bodyType = RigidbodyType2D.Kinematic;
			m_rigidBody.velocity = Vector2.zero;

			gameObject.layer = LayerMask.NameToLayer("FurnitureInUse");
            if (item.Scheme.vertical)
            {
                m_currentTweener = transform.DORotate(new Vector3(0, 0, 90), PICKUP_ANIM_TIME);
            }
        }

        public void PutDown()
        {
			item.cantKillMe = false;
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
			if (item != null)
			{
                item.gameObject.transform.SetParent(null);
				item.cantKillMe = false;
			}
			m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
            m_rigidBody.AddForce(impulse, ForceMode2D.Impulse);
            gameObject.layer = LayerMask.NameToLayer("Furniture");
            if (m_currentTweener != null)
            {
                m_currentTweener.Kill();
                m_currentTweener = null;
            }
        }

		private void Tick()
		{
			if (transform.position.y < -50)
			{
				// invoke or something, to tell its destroyed?
				Destroy(gameObject);
			}
		}
    }
}
