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
            Up = 0,
            Down =1,
            Both = 2
        }

        private static GameplayManager _instance;
        public static GameplayManager GetGameplayManager()
        {
            return _instance;
        }

        public Vector3 PlayerSpawnOffset => playerSpawnOffset;

        private GameplayEvents events;
        private BuildingsGenerator buildingsGenerator;
        private BuildingConfigurator buildingConfigurator;
        private TruckManager truckManager;
        private PlayerController[] players;

		[SerializeField] private GameObject dustStorm;
		[SerializeField] private GameObject placeBuildingOut;
        private Building buildingOut;
        [SerializeField] private GameObject placeBuildingIn;
        private Building buildingIn;
        [SerializeField] private Vector3 playerSpawnOffset;

        private Dictionary<int, List<Item>> itemsFromLastInBuilding;
        private List<ItemScheme> itemsFromTruck;
        private List<Item> itemsFormTruck2;
        [SerializeField] private GameObject prefabPlayer;
        private int playersCount = 1;

        private int currentPlayersInTruck = 0;
        private int currentPlayersDead = 0;

        public List<ItemScheme> DeEvacuationTruckItemList;
        private CatastrophiesDatabase catastrophiesDatabase;

        private CameraMultiTarget multiTargetCamera;
        [SerializeField] private List<GameObject> cameraTargets;

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

            SetupPlayerCounter();

            events = new GameplayEvents();
            buildingsGenerator = new BuildingsGenerator();
            buildingConfigurator = new BuildingConfigurator();
            truckManager = new TruckManager();
            players = new PlayerController[playersCount];

            catastrophiesDatabase = Resources.Load<CatastrophiesDatabase>("Databases/CatastrophiesDatabase");
            catastrophiesDatabase.LoadDataFromResources();


            var camera = Camera.main;
            multiTargetCamera = camera.gameObject.AddComponent<CameraMultiTarget>();
            cameraTargets.Clear();
            //multiTargetCamera.SetTargets(cameraTargets.ToArray());
        }

        private List<int> playerInputs = new List<int>();
        private void SetupPlayerCounter()
        {
            playerInputs.Clear();
            playersCount = 0;
            var playerConfig = StartConfig.GetStartConfig();
            if(playerConfig != null )
            {
                for(int i=0; i<playerConfig.GetMaxPlayerNumber(); i++)
                {
                    if (playerConfig.IsPlayerActive(i + 1))
                    {
                        playerInputs.Add(i+1);
                    }
                }
                playersCount = playerInputs.Count;
            }

            if(playersCount == 0)
            {
                playersCount = 1;
                playerInputs.Add(1);
            }
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


        public void AddMultiCameraTarget(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            cameraTargets.Add(target);
            multiTargetCamera.SetTargets(cameraTargets.ToArray());
        }

        public void RemoveMultiCameraTarget(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            cameraTargets.Remove(target);
            multiTargetCamera.SetTargets(cameraTargets.ToArray());
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

            multiTargetCamera.SetTargets(cameraTargets.ToArray());

            events.CallEvent(GamePhases.GameplayPhase.FadeIn, null);
            float fadeDealy = 1f;
            DOVirtual.DelayedCall(fadeDealy, PhaseBadEventStart);
            Debug.Log("PhaseStart");
        }

        private void SetupTruck()
        {
            var truckOutPosition = placeBuildingOut.transform.position;
            truckOutPosition.x += 20f;
            //truckOutPosition.z += -3.5f;

            var truckInPosition = placeBuildingIn.transform.position;
            truckInPosition.x -= 7f;
			//truckInPosition.z += -3.5f;
			truckInPosition.z = 0;
            if (truckManager.CreateTruck(gameObject.transform, truckOutPosition, truckInPosition))
            {
                AddMultiCameraTarget(truckManager.Truck.gameObject);
                var cameraMocap = truckManager.Truck.GetComponentInChildren<CameraMocap>();
                if (cameraMocap != null)
                {
                    AddMultiCameraTarget(cameraMocap.gameObject);
                }
            }
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

                if (itemsFormTruck2 == null || itemsFormTruck2.Count == 0)
                {
                    buildingOut = buildingsGenerator.GenerateBuildingWithItems(placeBuildingOut.transform,
                                                                               buildingConfig.floorSegmentsCount,
                                                                               buildingConfig.buildingFloorsCount,
                                                                               buildingConfig.stairsSegmentIndex,
                                                                               items);
                }
                else
                {
                    buildingOut = buildingsGenerator.GenerateBuildingWithItems(placeBuildingOut.transform,
                                                           buildingConfig.floorSegmentsCount,
                                                           buildingConfig.buildingFloorsCount,
                                                           buildingConfig.stairsSegmentIndex,
                                                           itemsFormTruck2);
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
            //DOVirtual.DelKayedCall(1f, () => { ProcessCatastrophy(); });
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
            if (currentCatastrophy != null)
            {
                currentEvacuationDirection = currentCatastrophy.EvacuationDirection;
                switch (currentEvacuationDirection)
                {
                    case EvecuationDirection.Down:
                        currentFloorBadEvent = buildingOut.floors.Count;
                        break;
                    default:
                        currentFloorBadEvent = -1;
                        break;
                }
            }
            else
            {
                currentEvacuationDirection = EvecuationDirection.Up;
                currentFloorBadEvent = -1;
            }
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
			if (currentCatastrophy.Type == CatastrophyType.EarthQuake)
			{
				var dust = Instantiate(dustStorm);
				//dust.transform.localRotation.eulerAngles = new Vector3(0)
				var pos = placeBuildingOut.transform.position;
				pos.z = -6;
				dust.transform.position = pos;
				Destroy(dust.gameObject, 5f);
			}
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
            bool isGameOver = false;
            switch (currentEvacuationDirection)
            {
                case EvecuationDirection.Down:
                    currentFloorBadEvent--;
                    if (currentFloorBadEvent < 1)
                    {
                        isGameOver = true;
                    }
                    break;
                default:
                    currentFloorBadEvent++;
                    if (currentFloorBadEvent > buildingFloorNumber)
                    {
                        isGameOver = true;
                    }
                    break;
            }
            if (isGameOver)
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

        private bool EndEvacuation()
        {
            if (currentPlayersDead + currentPlayersInTruck == playersCount)
            {
                isEvacuation = false;

                if(currentPlayersInTruck > 0)
                {
                    // go go go
                    PhaseTruckStart();
                }
                else
                {
                    // game over
                    float delay = 1f;
                    DOVirtual.DelayedCall(delay, () => events.CallEvent(GamePhases.GameplayPhase.GameOver, null));
                }

                return true;
            }
            return false;
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

                AddMultiCameraTarget(instance.gameObject);
                player.InputLayout = (InputHandler.Layout)(playerInputs[i]);

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
            if (param is PlayerController)
            {
                PlayerController pc = param as PlayerController;
                RemoveMultiCameraTarget(pc.gameObject);
            }

            currentPlayersDead++;
            EndEvacuation();
        }

        private void ReactionPlayerJump(object param)
        {
            //EndEvacuation();
        }

        private void ReactionGameOver(object param)
        {
            Debug.Log("GameOver");
        }

        private void ReactionPlayerInTruck(object param)
        {
            currentPlayersInTruck++;
            EndEvacuation();
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
            itemsFormTruck2 = truckManager.GetTruckItemList2();
            float delay = 0.2f;
            DOVirtual.DelayedCall(delay, PhaseStartGame);
            //itemsFromTruck = truckManager.GetTruckItemList();
            //events.CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
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
            //itemsFromLastInBuilding = buildingIn.GetItems();
            buildingsGenerator.DestroyBuildingOut(ref buildingIn);
            float delay = 1f;
            DOVirtual.DelayedCall(delay, PhaseStartGame);
            Debug.Log("PhaseFewDaysLater");
        }
    }
}
