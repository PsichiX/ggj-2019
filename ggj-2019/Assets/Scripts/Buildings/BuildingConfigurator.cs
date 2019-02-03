using UnityEngine;

namespace GaryMoveOut
{
    public class BuildingConfigurator
    {

        public BuildingConfig BuildingParameterGenerator(int buildingId)
        {
            var buildingConfig = new BuildingConfig();

            buildingConfig.floorSegmentsCount = Random.Range(5, 10);
            buildingConfig.stairsSegmentIndex = Random.Range(1, buildingConfig.floorSegmentsCount - 1);
            buildingConfig.buildingFloorsCount = Random.Range(4, 9);
            buildingConfig.minItemsCountToMaxFreeSegmentsRatio = 0.8f;
            buildingConfig.wallsColor = Random.ColorHSV(0, 1, 0, 1, 0.65f, 0.95f);

            return buildingConfig;
        }
    }
}
