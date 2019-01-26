using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(ItemScheme))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pickable : MonoBehaviour
    {
        public bool IsPickedUp => m_rigidBody.bodyType == RigidbodyType2D.Kinematic;

        private Rigidbody2D m_rigidBody;

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
        }

        public void PickUp()
        {
            m_rigidBody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void PutDown()
        {
            m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        public void Throw(Vector2 impulse)
        {
            m_rigidBody.bodyType = RigidbodyType2D.Dynamic;
            m_rigidBody.AddForce(impulse, ForceMode2D.Impulse);
        }
    }
}
