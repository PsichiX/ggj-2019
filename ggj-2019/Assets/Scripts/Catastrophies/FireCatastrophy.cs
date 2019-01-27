using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Fire")]
    public class FireCatastrophy : BaseCatastrophy
    {
        public GameObject firePrefab;

        public override CatastrophyType Type { get { return CatastrophyType.Fire; } }

        public override void Initialize()
        {
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Fire on the {floorIndex} dance floor!");

            if (building.floors.TryGetValue(floorIndex, out Floor floor))
            {
                for(int i = 1; i < floor.segments.Count - 1; i++)
                {
                    var fire = GameObject.Instantiate(firePrefab, floor.segments[i].transform);
                }
            }
        }
    }
}