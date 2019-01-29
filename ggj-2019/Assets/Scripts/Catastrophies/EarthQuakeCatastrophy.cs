using DG.Tweening;
using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Earth Quake")]
    public class EarthQuakeCatastrophy : BaseCatastrophy
    {
        public float floorFallTime = 0.4f;

        public override CatastrophyType Type { get { return CatastrophyType.EarthQuake; } }
        public override EvecuationDirection EvacuationDirection { get { return EvecuationDirection.Up; } }

        private BuildingSegmentsDatabase buildingsDatabase;


        public override void Initialize()
        {
            buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Eaaaarthhhh Quuuuaaaakeeeee on the {floorIndex} floor!");


            var endPos = building.root.transform.position - new Vector3(0f, building.SegmentHeight, 0f);
            building.root.transform.DOMove(endPos, floorFallTime).SetEase(Ease.OutBounce);

            var force = 3f;
            //foreach (var floor in building.floors)
            //{
            //    // apply force to all items:
            //    foreach (var floorObj in building.floors.Values)
            //    {
            //        foreach (var item in floorObj.itemGOs.Values)
            //        {
            //            if (item != null)
            //            {
            //                var rigidBody = item.GetComponent<Rigidbody2D>();
            //                if (rigidBody != null)
            //                {
            //                    rigidBody.AddForce(new Vector2(0f, -force), ForceMode2D.Impulse);
            //                }
            //            }
            //        }
            //    }
            //}
            DOVirtual.DelayedCall(floorFallTime, () =>
            {
                building.DestroyFloor(floorIndex);
            });
        }
    }
}