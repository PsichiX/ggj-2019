using UnityEngine;

namespace GaryMoveOut
{
    public abstract class BaseCatastrophy : ScriptableObject
    {
        public abstract CatastrophyType Type { get; }

        public abstract void DestroyFloor(Building building, int floorIndex);
    }
}