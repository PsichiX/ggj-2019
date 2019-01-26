using UnityEngine;

namespace GaryMoveOut
{
    [System.Serializable]
    public class FloorScheme
    {
        public float segmentWidth = 2f;
        public float segmentHeight = 3f;
        public float segmentDepth = 3f;

        public GameObject EmptyWall;
        public GameObject Stairs;
        public GameObject SideWall;
        [SerializeField] private GameObject sideWallRight;
        public GameObject SideWallR { get { return (sideWallRight != null) ? sideWallRight : SideWall; } }
        [SerializeField] private GameObject sideWallLeft;
        public GameObject SideWallL { get { return (sideWallLeft != null) ? sideWallLeft : SideWall; } }
        public GameObject SideDoor;
        public GameObject SideWindowR;
        public GameObject SideWindowL;


        public GameObject GetRandomSideWall(bool rightSide)
        {
            GameObject prefab = null;

            prefab = rightSide ? SideWallR : SideWallL;
            //var rnd = Random.Range(0, 2);
            //switch (rnd)
            //{
            //    case 0:
            //        prefab = SideWindow;
            //        break;
            //    default:
            //        prefab = rightSide ? SideWallR : SideWallL;
            //        break;
            //}

            return prefab;
        }
    }

}