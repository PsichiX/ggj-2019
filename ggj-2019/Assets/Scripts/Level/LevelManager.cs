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
            //buildingsGenerator = new BuildingsGenerator();

            //if (homeRoot != null)
            //{

            //    var itemsCount = Random.Range(5, 8);
            //    var items = buildingsGenerator.ItemsDatabase.GetRandomItems_OLD(itemsCount);

            //    building = buildingsGenerator.GenerateBuildingWithItems(homeRoot, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex, items);
            //}
        }
    }

}