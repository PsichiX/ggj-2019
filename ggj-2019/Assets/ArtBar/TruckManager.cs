using UnityEngine;
using DG.Tweening;


namespace GaryMoveOut
{
    public class TruckManager
    {
        public TruckDatabase TruckDatabase { get; private set; }
        private GameObject truck;
        private Vector3 truckInPosition;
        private Animator anim;
        private TruckLoader truckLoader;

        public TruckManager()
        {
            TruckDatabase = Resources.Load<TruckDatabase>("Databases/TruckDatabase");
        }

        public bool CreateTruck(Transform parent, Vector3 truckOutPosition, Vector3 truckInPosition)
        {
            if (truck == null)
            {
                if (TruckDatabase != null)
                {
                    if (TruckDatabase.truckPrefabs.Count > 0)
                    {
                        truck = Object.Instantiate(TruckDatabase.truckPrefabs[0], truckOutPosition, parent.rotation, parent);
                        if( truck != null)
                        {
                            truck.transform.Rotate(Vector3.up, 180f);
                            this.truckInPosition = truckInPosition;
                            anim = truck.GetComponent<Animator>();
                            truckLoader = truck.GetComponentInChildren<TruckLoader>();
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

            truck.transform.position = truckOutPosition;
            this.truckInPosition = truckInPosition;
            return true;
        }

        public void ResetTruckItemList()
        {
            truckLoader.ResetTruckItemList();
        }

        public void StartTruckMovement(float duration)
        {
            //anim.SetFloat("Forward", 1f);
            truck.transform.DOMove(truckInPosition, duration).SetEase(Ease.InOutQuad);
            //DOVirtual.DelayedCall(duration - 0.2f, () => { anim.SetFloat("Forward", 0f); });
        }
    }
}
