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
            buildingConfig.floorSegmentsCount = 5;
            buildingConfig.buildingFloorsCount = 4;
            buildingConfig.stairsSegmentIndex = 2;
            buildingConfig.minItemsCountToMaxFreeSegmentsRatio = 0.8f;
            return buildingConfig;
        }
    }
}
