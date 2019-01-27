using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class GameplayManager : MonoBehaviour
    {
        public enum EvecuationDirection
        {
            Up,
            Down
        }

        private static GameplayManager _instance;
        public static GameplayManager GetGameplayManager()
        {
            return _instance;
        }

        private GameplayEvents events;
        private BuildingsGenerator buildingsGenerator;
        private BuildingConfigurator buildingConfigurator;
        private TruckManager truckManager;
        private PlayerController[] players;

        [SerializeField] private GameObject placeBuildingOut;
        private Building buildingOut;
        [SerializeField] private GameObject placeBuildingIn;
        private Building buildingIn;
        [SerializeField] private Vector3 playerSpawnOffset;

        private Dictionary<int, List<Item>> itemsFromLastInBuilding;
        [SerializeField] private GameObject prefabPlayer;
        [SerializeField] private int playersCount = 1;

        public List<ItemScheme> DeEvacuationTruckItemList;
        private CatastrophiesDatabase catastrophiesDatabase;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }

            events = new GameplayEvents();
            buildingsGenerator = new BuildingsGenerator();
            buildingConfigurator = new BuildingConfigurator();
            truckManager = new TruckManager();
            players = new PlayerController[playersCount];

            catastrophiesDatabase = Resources.Load<CatastrophiesDatabase>("Databases/CatastrophiesDatabase");
            catastrophiesDatabase.LoadDataFromResources();
        }

        private void Start()
        {
            AttachReactionsToEvents();
            PhaseStartGame();
        }

        private void AttachReactionsToEvents()
        {
            events.AttachToEvent(GamePhases.GameplayPhase.FadeIn, ReactionFadeIn);
            events.AttachToEvent(GamePhases.GameplayPhase.FadeOut, ReactionFadeOut);
            events.AttachToEvent(GamePhases.GameplayPhase.PlayerJump, ReactionPlayerJump);
            events.AttachToEvent(GamePhases.GameplayPhase.PlayerInTruck, ReactionPlayerInTruck);
            events.AttachToEvent(GamePhases.GameplayPhase.LastItemShot, ReactionLastItemShot);
            events.AttachToEvent(GamePhases.GameplayPhase.PlayerDie, ReactionPlayerDie);
            events.AttachToEvent(GamePhases.GameplayPhase.GameOver, ReactionGameOver);
            events.AttachToEvent(GamePhases.GameplayPhase.StartNewGame, PhaseStartNewGame);

            events.AttachToEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, ReactionEvacuationEnd);
        }

        void Update()
        {
            EvacuationProcess();
        }


        private void PhaseStartNewGame(object param)
        {
            PhaseStartGame();
        }

        private void PhaseStartGame()
        {
            SetupBuildingOut();
            SetupTruck();
            SetupCatastrophy();
            events.CallEvent(GamePhases.GameplayPhase.FadeIn, null);
            float fadeDealy = 1f;
            DOVirtual.DelayedCall(fadeDealy, PhaseBadEventStart);
            Debug.Log("PhaseStart");
        }

        private void SetupTruck()
        {
            var truckOutPosition = placeBuildingOut.transform.position;
            truckOutPosition.x += 20f;
            truckOutPosition.z += -3.5f;

            var truckInPosition = placeBuildingIn.transform.position;
            truckInPosition.x -= 7f;
            truckInPosition.z += -3.5f;
            truckManager.CreateTruck(gameObject.transform, truckOutPosition, truckInPosition);
        }

        private void SetupBuildingOut()
        {
            if (buildingsGenerator == null)
            {
                Debug.Log("building generator == null");
                return;
            }

            if (placeBuildingOut != null)
            {
                var buildingConfig = buildingConfigurator.BuildingParameterGenerator(currentBuildingId);

                var maxFreeSegments = (buildingConfig.floorSegmentsCount - 1) * buildingConfig.buildingFloorsCount;
                var minItemsCount = (int)(buildingConfig.minItemsCountToMaxFreeSegmentsRatio * maxFreeSegments);
                var itemsCount = UnityEngine.Random.Range(minItemsCount, maxFreeSegments);
                var items = buildingsGenerator.ItemsDatabase.GetRandomItems(itemsCount);

                if (itemsFromLastInBuilding != null)
                {
                    buildingOut = buildingsGenerator.GenerateBuildingWithItems(placeBuildingOut.transform,
                                                                               buildingConfig.floorSegmentsCount,
                                                                               buildingConfig.buildingFloorsCount,
                                                                               buildingConfig.stairsSegmentIndex,
                                                                               itemsFromLastInBuilding);
                }
                else
                {
                    buildingOut = buildingsGenerator.GenerateBuildingWithItems(placeBuildingOut.transform,
                                                           buildingConfig.floorSegmentsCount,
                                                           buildingConfig.buildingFloorsCount,
                                                           buildingConfig.stairsSegmentIndex,
                                                           items);
                }
                buildingFloorNumber = buildingConfig.buildingFloorsCount;
            }
        }

        private void SetupBuildingIn()
        {
            if (buildingsGenerator == null)
            {
                Debug.Log("building generator == null");
                return;
            }

            if (placeBuildingIn != null)
            {
                var buildingConfig = buildingConfigurator.BuildingParameterGenerator(currentBuildingId);
                buildingIn = buildingsGenerator.GenerateBuilding(placeBuildingIn.transform, buildingConfig.floorSegmentsCount, buildingConfig.buildingFloorsCount, buildingConfig.stairsSegmentIndex);
            }
        }

        private BaseCatastrophy currentCatastrophy;
        private void SetupCatastrophy()
        {
            currentCatastrophy = catastrophiesDatabase.GetRandomCatastrophy();

            // debug:
            ProcessCatastrophy();
        }


        private void PhaseBadEventStart()
        {
            events.CallEvent(GamePhases.GameplayPhase.BadEventStart, null);
            float delay = 2f;
            DOVirtual.DelayedCall(delay, PhaseStartEvacuation);
            Debug.Log("PhaseBadEventStart");
        }

        private void PhaseStartEvacuation()
        {
            events.CallEvent(GamePhases.GameplayPhase.Evacuation, null);
            Debug.Log("PhaseStartEvacuation");
            isEvacuation = true;
            currentEvacuationDirection = EvecuationDirection.Up;
            currentFloorBadEvent = -1;
            NextFloorBadEvent();
        }

        private void PhaseFloorEvacuationStart(int floor)
        {
            events.CallEvent(GamePhases.GameplayPhase.FloorEvacuationStart, floor);
            Debug.Log($"PhaseFloorEvacuationStart Floor [{floor}]");
        }

        private void PhaseFloorEvacuationBreakPoint(int floor)
        {
            events.CallEvent(GamePhases.GameplayPhase.FloorEvacuationBreakPoint, floor);
            Debug.Log($"PhaseFloorEvacuationBreakPoint Floor [{floor}]");
        }

        private void PhaseFloorEvacuationEnd(int floor)
        {
            events.CallEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, floor);
            Debug.Log($"PhaseFloorEvacuationEnd Floor [{floor}]");
        }

        private int buildingFloorNumber = 0;
        public int currentFloorBadEvent = 0;
        public int currentBuildingId = 0;
        private bool isEvacuation = false;
        private float evacuationTimeStartToBreakPoint = 5f;     //start to break point
        private float evacuationTimeStartToEnd = 10f;           //start to end

        public EvecuationDirection currentEvacuationDirection;
        private float currentEvacuationTime = 0;
        private float currentEvacuationTimeStartToBreakPoint = 0;
        private float currentEvacuationTimeStartToEnd = 0;

        private void EvacuationProcess()
        {
            if (isEvacuation == false)
            {
                return;
            }

            currentEvacuationTime += Time.deltaTime;

            if (currentEvacuationTimeStartToBreakPoint > 0 && currentEvacuationTime > currentEvacuationTimeStartToBreakPoint)
            {
                currentEvacuationTimeStartToBreakPoint = -1f;
                PhaseFloorEvacuationBreakPoint(currentFloorBadEvent);
            }

            if (currentEvacuationTimeStartToEnd > 0 && currentEvacuationTime > currentEvacuationTimeStartToEnd)
            {
                currentEvacuationTimeStartToEnd = -1f;
                PhaseFloorEvacuationEnd(currentFloorBadEvent);
                NextFloorBadEvent();
            }
        }

        private void NextFloorBadEvent()
        {
            currentFloorBadEvent++;
            if (currentFloorBadEvent > buildingFloorNumber)
            {
                isEvacuation = false;
                events.CallEvent(GamePhases.GameplayPhase.GameOver, null);
                return;
            }

            currentEvacuationTime = 0f;
            currentEvacuationTimeStartToBreakPoint = evacuationTimeStartToBreakPoint;
            currentEvacuationTimeStartToEnd = evacuationTimeStartToEnd;
            PhaseFloorEvacuationStart(currentFloorBadEvent);
        }

        private void EndEvacuation()
        {
            isEvacuation = false;
        }

        private void ReactionFadeIn(object param)
        {
            for (var i = 0; i < players.Length; ++i)
            {
                var instance = Instantiate(prefabPlayer);
                var spawnPos = buildingOut.GetSpawnPosition();
                var pos = spawnPos.HasValue ? new Vector3(spawnPos.Value.x, spawnPos.Value.y, 0) : Vector3.zero;
                instance.transform.position = pos + playerSpawnOffset;
                instance.transform.rotation = Quaternion.identity;
                var player = players[i] = instance.GetComponent<PlayerController>();
                player.InputLayout = (InputHandler.Layout)(1 + i);
            }
        }

        private void ReactionFadeOut(object param)
        {
            for (var i = 0; i < players.Length; ++i)
            {
                GameObject.Destroy(players[i]);
                players[i] = null;
            }
        }

        private void ReactionPlayerDie(object param)
        {
            EndEvacuation();
            float delay = 1f;
            DOVirtual.DelayedCall(delay, () => events.CallEvent(GamePhases.GameplayPhase.GameOver, null));
        }

        private void ReactionPlayerJump(object param)
        {
            EndEvacuation();
        }

        private void ReactionGameOver(object param)
        {
            Debug.Log("GameOver");
        }

        private void ReactionPlayerInTruck(object param)
        {
            PhaseTruckStart();
        }

        private void ReactionEvacuationEnd(object obj)
        {
            ProcessCatastrophy();
        }

        private void ProcessCatastrophy()
        {
            if(currentCatastrophy != null)
            {
                currentCatastrophy.DestroyFloor(buildingOut, currentFloorBadEvent);
            }
        }

        private void PhaseTruckStart()
        {
            events.CallEvent(GamePhases.GameplayPhase.TruckStart, null);
            currentBuildingId++;
            SetupBuildingIn();

            float delay = 2f;
            truckManager.StartTruckMovement(delay);
            DOVirtual.DelayedCall(delay, PhaseTruckStop);
            Debug.Log("PhaseTruckStart");
        }

        private void PhaseTruckStop()
        {
            events.CallEvent(GamePhases.GameplayPhase.TruckStop, null);

            buildingsGenerator.DestroyBuildingOut(ref buildingOut);

            float delay = 1f;
            DOVirtual.DelayedCall(delay, PhaseDeEvacuation);
            Debug.Log("PhaseTruckStop");
        }

        private void PhaseDeEvacuation()
        {
            DeEvacuationTruckItemList = truckManager.GetTruckItemList();
            events.CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
            Debug.Log("PhaseDeEvacuation");
        }

        private void ReactionLastItemShot(object param)
        {
            truckManager.ResetTruckItemList();
            events.CallEvent(GamePhases.GameplayPhase.FadeOut, null);
            float delay = 1f;
            DOVirtual.DelayedCall(delay, PhaseFewDaysLater);
            Debug.Log("FadeOut");
        }

        private void PhaseFewDaysLater()
        {
            events.CallEvent(GamePhases.GameplayPhase.FewDaysLater, null);
            itemsFromLastInBuilding = buildingIn.GetItems();
            buildingsGenerator.DestroyBuildingOut(ref buildingIn);
            float delay = 1f;
            DOVirtual.DelayedCall(delay, PhaseStartGame);
            Debug.Log("PhaseFewDaysLater");
        }
    }
}