using UnityEngine;
using DG.Tweening;


namespace GaryMoveOut
{
    public class TruckManager
    {
        public TruckDatabase TruckDatabase { get; private set; }
        private GameObject truck;
        private Vector3 truckInPosition;

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
                        //truck = Object.Instantiate(TruckDatabase.truckPrefabs[0], truckOutPosition, parent.rotation, parent);
                        truck = Object.Instantiate(TruckDatabase.truckPrefabs[0], truckOutPosition, parent.rotation, parent);
                        if( truck != null)
                        {
                            truck.transform.Rotate(Vector3.up, 180f);
                            this.truckInPosition = truckInPosition;
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

        public void StartTruckMovement(float duration)
        {
            truck.transform.DOMove(truckInPosition, duration).SetEase(Ease.InOutQuad);
        }
    }
}
