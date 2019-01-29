using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GaryMoveOut
{
    public class TruckManager
    {
        public TruckDatabase TruckDatabase { get; private set; }
        public GameObject Truck { get; private set; }
        private Vector3 truckInPosition;
        private Animator anim;
        private TruckLoader truckLoader;
		private AudioSource truckAudio;

        public TruckManager()
        {
            TruckDatabase = Resources.Load<TruckDatabase>("Databases/TruckDatabase");
        }

        public bool CreateTruck(Transform parent, Vector3 truckOutPosition, Vector3 truckInPosition)
        {
            if (Truck == null)
            {
                if (TruckDatabase != null)
                {
                    if (TruckDatabase.truckPrefabs.Count > 0)
                    {
                        Truck = Object.Instantiate(TruckDatabase.truckPrefabs[Random.Range(0,TruckDatabase.truckPrefabs.Count)], truckOutPosition, parent.rotation, parent);
                        if( Truck != null)
                        {
                            Truck.transform.Rotate(Vector3.up, 180f);
                            this.truckInPosition = truckInPosition;
                            anim = Truck.GetComponent<Animator>();
                            truckLoader = Truck.GetComponentInChildren<TruckLoader>();
                            if(truckLoader == null)
                            {
                                Debug.LogError("Truck Loader not found!");
                            }
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
			Truck.transform.position = truckOutPosition;
            this.truckInPosition = truckInPosition;
            return true;
        }

        public void ResetTruckItemList()
        {
            truckLoader.ResetTruckItemList();
        }

        public List<ItemScheme> GetTruckItemList()
        {
            return truckLoader.GetItemList();
        }

        public void StartTruckMovement(float duration)
        {
			truckAudio = Truck.GetComponent<AudioSource>();
			if (truckAudio != null)
			{
				truckAudio.Play();
				truckAudio.DOFade(0, duration);
			}
			anim.SetFloat("Forward", 1f);
			Truck.transform.DOMove(truckInPosition, duration).SetEase(Ease.OutQuad).OnComplete(() => anim.SetFloat("Forward", 0f));
        }
    }
}
