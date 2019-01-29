﻿using DG.Tweening;
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

    public class ItemScheme : MonoBehaviour
    {
		public event Action<ItemScheme> NewItemInTruck;
		[SerializeField] private GameObject explosion;
		[SerializeField] private AudioSource ausource;

		public float value;
        public float weight;
        public bool vertical;
        public ItemType type;
        public Item assignedItem;

        private bool isAlive = true;

		private void Start()
		{
			if (ausource == null)
			{
				ausource = GetComponent<AudioSource>();
			}
		}
		public void DestroyOnGround()
        {
            isAlive = false;
			var ex = Instantiate(explosion, transform);

			ex.transform.localPosition = Vector3.zero;
			ausource.Play();
			DOVirtual.DelayedCall(0.6f, KillMe);
            // TODO: destroy viz
        }

		private void KillMe()
		{
			Destroy(gameObject, 0.5f);
		}

        public void InTruck()
        {
			//NewItemInTruck?.Invoke(this);
			ausource.PlayOneShot(Resources.Load("Sounds/gain") as AudioClip);
			GameplayManager.CallNewItemInTruckEvent(this);
            // TODO: hide object
            KillMe();

        }

        public bool IsItemAlive()
        {
            return isAlive;
        }
    }
}