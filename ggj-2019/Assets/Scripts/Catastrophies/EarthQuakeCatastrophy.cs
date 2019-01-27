using DG.Tweening;
using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Earth Quake")]
    public class EarthQuakeCatastrophy : BaseCatastrophy
    {
        public float floorFallTime = 0.4f;

        public override CatastrophyType Type { get { return CatastrophyType.EarthQuake; } }
        private BuildingSegmentsDatabase buildingsDatabase;


        public override void Initialize()
        {
            buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Eaaaarthhhh Quuuuaaaakeeeee on the {floorIndex} floor!");

            if (building.floors.TryGetValue(floorIndex, out Floor floor))
            {
                var endPos = building.root.transform.position - new Vector3(0f, building.SegmentHeight, 0f);
                building.root.transform.DOMove(endPos, floorFallTime).SetEase(Ease.OutBounce);
                DOVirtual.DelayedCall(floorFallTime, () =>
                {
                    building.DestroyFloor(floorIndex);
                });
            }
        }
    }
}