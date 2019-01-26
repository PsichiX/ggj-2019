using UnityEngine;

namespace GaryMoveOut
{
    public class DoorPortal : MonoBehaviour
    {
        public const int MaxIndex = 999;
        public const int MinIndex = -999;

        public int floorIndexBelow;
        public int floorIndex;
        public int floorIndexAbove;
    }
}