using DG.Tweening;
using System;
using UnityEngine;

namespace GaryMoveOut
{
    public enum ItemType
    {
        None = 0,
        Fragile = 1,
        Heavy = 2,
        Fluffy = 3,
    }

    [System.Obsolete]
    public class ItemScheme_OLD : MonoBehaviour
    {
		public event Action<ItemScheme_OLD> NewItemInTruck;
		[SerializeField] private GameObject explosion;
		[SerializeField] private AudioSource ausource;

		public float value;
        public float weight;
        public bool vertical;
        public ItemType type;
        public Item_OLD assignedItem;

        private bool isAlive = true;

		private void Start()
		{
			if (ausource == null)
			{
				ausource = GetComponent<AudioSource>();
			}
		}

		public bool cantkillme = false;

		public void DestroyOnGround()
        {
			if (cantkillme)
			{
				return;
			}
            isAlive = false;
			var ex = Instantiate(explosion, transform);

			ex.transform.localPosition = Vector3.zero;
			ausource.Play();
			DOVirtual.DelayedCall(0.6f, KillMe);
            // TODO: destroy viz
        }

		private void KillMe()
		{
			transform.parent = null;
			Destroy(gameObject, 0.2f);
		}

		private void FreezeMe()
		{
			Destroy(GetComponent<Pickable>());
			Destroy(GetComponent<Rigidbody2D>());
			transform.parent = null;
			GetComponent<BoxCollider2D>().enabled = false;
		}

		public void HideMe()
		{
			foreach (var mr in GetComponentsInChildren<MeshRenderer>())
			{
				mr.enabled = false;
			}
		}

		public void UnKillMe()
		{
			gameObject.AddComponent<Rigidbody2D>();
			gameObject.AddComponent<Pickable>();
			foreach (var mr in GetComponentsInChildren<MeshRenderer>())
			{
				mr.enabled = true;
			}
			GetComponent<BoxCollider2D>().enabled = true;
			
			cantkillme = true;
		}

        public void InTruck()
        {
			//NewItemInTruck?.Invoke(this);
			ausource.PlayOneShot(Resources.Load("Sounds/gain") as AudioClip);
			GameplayManager.CallNewItemInTruckEvent(this);
			FreezeMe();
        }

		public void CopyValues(ItemScheme_OLD source)
		{
			assignedItem = source.assignedItem;
			value = source.value;
			weight = source.weight;
			vertical = source.vertical;
			type = source.type;
		}

        public bool IsItemAlive()
        {
            return isAlive;
        }
    }
}