using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut.Catastrophies
{
    public abstract class BaseCatastrophy : ScriptableObject
    {
        public abstract CatastrophyType Type { get; }
        public abstract EvecuationDirection EvacuationDirection { get; }

        public abstract void Initialize();

        public abstract void Dispose();

        public abstract void DestroyFloor(Building building, int floorIndex);
    }
}