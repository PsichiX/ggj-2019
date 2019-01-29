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
        public ItemScheme Scheme { get { return scheme; } }


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
            Destroy(itemRigidbody2D);
            transform.parent = null;
            boxCollider2D.enabled = false;
        }

        public void HideMe()
        {
            foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = false;
            }
        }

        public bool cantKillMe = false;
        public bool IsAlive { get; private set; }

        public void DestroyOnGround()
        {
            if (cantKillMe)
            {
                return;
            }
            IsAlive = false;
            var ex = Instantiate(Scheme.explosion, transform);

            ex.transform.localPosition = Vector3.zero;
            audioSource.Play();
            DOVirtual.DelayedCall(0.6f, KillMe);
            // TODO: destroy viz
        }

        private void KillMe()
        {
            transform.parent = null;
            Destroy(gameObject, 0.2f);
        }

        public void UnKillMe()
        {
            if (itemRigidbody2D == null)
            {
                itemRigidbody2D = gameObject.AddComponent<Rigidbody2D>();
            }
            if (pickable == null)
            {
                gameObject.AddComponent<Pickable>();
            }
            foreach (var mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.enabled = true;
            }
            boxCollider2D.enabled = true;

            cantKillMe = true;
        }
    }
}