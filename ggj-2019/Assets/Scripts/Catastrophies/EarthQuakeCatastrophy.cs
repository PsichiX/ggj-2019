using UnityEngine;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Earth Quake")]
    public class EarthQuakeCatastrophy : BaseCatastrophy
    {
        public override CatastrophyType Type { get { return CatastrophyType.EarthQuake; } }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Eaaaarthhhh Quuuuaaaakeeeee on the {floorIndex} floor!");
        }
    }
}