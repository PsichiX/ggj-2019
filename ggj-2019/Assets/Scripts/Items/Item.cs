using DG.Tweening;
using UnityEngine;

namespace GaryMoveOut.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private Rigidbody2D itemRigidbody2D;
        [SerializeField] private Pickable pickable;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ItemScheme scheme;
		public bool IsAlive { get; private set; }
        public ItemScheme Scheme { get { return scheme; } }
		public bool cantKillMe = false;

		private void Start()
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
            itemRigidbody2D = GetComponent<Rigidbody2D>();
            pickable = GetComponent<Pickable>();
            audioSource = GetComponent<AudioSource>();
        }

        public void Setup(ItemScheme scheme)
        {
            this.scheme = scheme;
        }

        public void InTruck()
        {
            audioSource.PlayOneShot(Resources.Load("Sounds/gain") as AudioClip);    // => Fix me: sounds manager needed
            GameplayManager.GetGameplayManager().CallNewItemInTruckEvent(Scheme);
            FreezeMe();
        }

        private void FreezeMe()
        {
            Destroy(pickable);
            itemRigidbody2D.Sleep();
            transform.parent = null;
            boxCollider2D.enabled = false;
			cantKillMe = true;
		}

		public void HideMe()
        {
            foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
		}


        public void DestroyOnGround()
        {
            if (cantKillMe)
            {
                return;
            }
			itemRigidbody2D.Sleep();
            IsAlive = false;
            var ex = Instantiate(Scheme.explosion, transform);
			HideMe();
            ex.transform.localPosition = Vector3.zero;
            audioSource.Play();
            DOVirtual.DelayedCall(0.6f, KillMe);
            // TODO: destroy viz
        }

        private void KillMe()
        {
			if (transform == null)
			{
				return;
			}
            transform.SetParent(null);
            Destroy(gameObject, 0.2f);
        }

        public void UnKillMe()
        {
            cantKillMe = true;
            boxCollider2D.enabled = true;
			itemRigidbody2D.WakeUp();
			itemRigidbody2D.velocity = Vector2.zero;

			if (pickable == null)
            {
				pickable = gameObject.AddComponent<Pickable>();
            }
            foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
        }

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Ground")
			{
				if (itemRigidbody2D.velocity.magnitude > 1f)
				{
					DestroyOnGround();
				}
			}
		}
	}
}