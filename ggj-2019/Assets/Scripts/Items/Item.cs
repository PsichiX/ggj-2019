using UnityEngine;

namespace GaryMoveOut.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ItemScheme scheme;
        public ItemScheme Scheme { get { return scheme; } }


        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Setup(ItemScheme scheme)
        {
            this.scheme = scheme;
        }
    }
}