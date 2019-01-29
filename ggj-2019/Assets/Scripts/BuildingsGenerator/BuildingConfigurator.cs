using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class BuildingConfigurator
    {

        public BuildingConfig BuildingParameterGenerator(int buildingId)
        {
            var buildingConfig = new BuildingConfig();
            buildingConfig.floorSegmentsCount = Random.Range(5,10);
            buildingConfig.buildingFloorsCount = Random.Range(4, 9);
            buildingConfig.stairsSegmentIndex = 2;
            buildingConfig.minItemsCountToMaxFreeSegmentsRatio = 0.8f;
            return buildingConfig;
        }
    }
}
