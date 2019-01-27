using UnityEngine;
using UnityEditor;

namespace GaryMoveOut
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Fire")]
    public class FireCatastrophy : BaseCatastrophy
    {
        public override CatastrophyType Type { get { return CatastrophyType.Fire; } }

        public override void Initialize()
        {
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Fire on the {floorIndex} dance floor!");
        }
    }
}