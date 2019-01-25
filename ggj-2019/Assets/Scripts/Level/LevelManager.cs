using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform homeRoot;
    [SerializeField] private int floorSegmentsCount;
    [SerializeField] private int buildingFloorsCount;
    [SerializeField] private int stairsSegmentIndex;


    private BuildingsGenerator buildingsGenerator;


    // Use this for initialization
    void Start()
    {
        buildingsGenerator = new BuildingsGenerator();

        if (homeRoot != null)
        {
            buildingsGenerator.GenerateBuilding(homeRoot, floorSegmentsCount, buildingFloorsCount, stairsSegmentIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
