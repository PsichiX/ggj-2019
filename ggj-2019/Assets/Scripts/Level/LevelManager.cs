using UnityEngine;
using System.Collections;

namespace GaryMoveOut
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Transform homeRoot;
        [SerializeField] private int floorSegmentsCount;
        [SerializeField] private int buildingFloorsCount;
        [SerializeField] private int stairsSegmentIndex;


        private BuildingsGenerator buildingsGenerator;
        private Building building;


        // Use this for initialization
        void Start()
        {
            buildingsGenerator = new BuildingsGenerator();

            if (homeRoot != null)
            {
                building = buildingsGenerator.GenerateBuilding(homeRoot, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex);

                buildingsGenerator.DebugGenerateItems(ref building);

                //building.RelocateItems2();
                building.RelocateItems();
            }
        }
    }

}