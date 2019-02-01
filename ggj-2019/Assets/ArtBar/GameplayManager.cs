using DG.Tweening;
using GaryMoveOut.Catastrophies;
using GaryMoveOut.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public event System.Action<int> PointsCollectedUpdate;
        public event System.Action<Item> NewItemInTruck;
        public void CallNewItemInTruckEvent(Item item)
        {
            NewItemInTruck?.Invoke(item);
        }
        public int pointsCollected = 0;

        private static GameplayManager _instance;
        public static GameplayManager GetGameplayManager()
        {
            return _instance;
        }

        public Vector3 PlayerSpawnOffset => playerSpawnOffset;
		public float truckSpeedModifier = 15f;

		private GameplayEvents events;
        private BuildingsGenerator buildingsGenerator;
        private BuildingConfigurator buildingConfigurator;
        private TruckManager truckManager;
        private PlayerController[] players;

		[SerializeField] private GameObject dustStorm;
		[SerializeField] private GameObject loongo;
		[SerializeField] private GameObject placeBuildingOut;
        [SerializeField] private GameObject placeBuildingIn;
        [SerializeField] private Vector3 playerSpawnOffset;
        [SerializeField] private GameObject prefabPlayer;
        private Building buildingOut;
        private Building buildingIn;

        private List<ItemScheme> itemsInTruck;
		private Dictionary<int, List<ItemScheme>> itemsFromLastInBuilding;

		private int playersCount = 1;

        private int currentPlayersInTruck = 0;
        private int currentPlayersDead = 0;

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
            multiTargetCamera = camera.GetComponent<CameraMultiTarget>();
            cameraTargets.Clear();

            NewItemInTruck += GainPointsForItem;
        }

        public void GainPointsForItem(Item item)
        {
			RemoveFromCachedList(item);
            pointsCollected += (int)item.Scheme.value;
            PointsCollectedUpdate?.Invoke(pointsCollected);
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

        private void PhaseStartGame(bool useOldBuilding = false)
        {
			if (buildingIn != null)
			{
				Destroy(buildingIn.root.GetChild(0).gameObject);
			}
			var pos = placeBuildingIn.transform.position;
			pos.y = 0;
			placeBuildingIn.transform.position = pos;
			SetupBuildingOut(useOldBuilding);
            SetupTruck();
            SetupCatastrophy();

            multiTargetCamera.SetTargets(cameraTargets.ToArray());

            events.CallEvent(GamePhases.GameplayPhase.FadeIn, null);
            float fadeDelay = 1f;
            DOVirtual.DelayedCall(fadeDelay, PhaseBadEventStart);
            Debug.Log("PhaseStart");
        }

		private void SetupTruck()
        {
			var pos = placeBuildingOut.transform.position;
			pos.y = 0;
			placeBuildingOut.transform.position = pos;
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
            }
        }

        private void SetupBuildingOut(bool useOldItems = false)
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

				if (useOldItems == false)
				{
					var minItemsCount = (int)(buildingConfig.minItemsCountToMaxFreeSegmentsRatio * maxFreeSegments);
					var itemsCount = UnityEngine.Random.Range(minItemsCount, maxFreeSegments);
					var items = buildingsGenerator.ItemsSpawner.ItemsDatabase.GetRandomItems(itemsCount);
	                var floorSize = new FloorSize()
		            {
					    segmentsCount = buildingConfig.floorSegmentsCount,
				        stairsSegmentIndex = buildingConfig.stairsSegmentIndex
			        };
					buildingOut = buildingsGenerator.GenerateBuilding(placeBuildingOut.transform, buildingConfig.buildingFloorsCount, floorSize, items);
                }
                else
                {
					buildingOut = buildingsGenerator.GenerateBuilding(placeBuildingOut.transform, oldFloorCount, oldFloorSize, itemsFromLastInBuilding);
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
                var floorSize = new FloorSize()
                {
                    segmentsCount = buildingConfig.floorSegmentsCount,
                    stairsSegmentIndex = buildingConfig.stairsSegmentIndex
                };

                buildingIn = buildingsGenerator.GenerateBuilding(placeBuildingIn.transform, buildingConfig.buildingFloorsCount, floorSize);
			}
        }

        public BaseCatastrophy currentCatastrophy;
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
                        currentFloorBadEvent = buildingOut.Floors.Count;
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

        public int currentFloorBadEvent = 0;
        public int currentBuildingId = 0;
        public float evacuationTimeStartToEnd = 12f;           //start to end
        private float evacuationTimeStartToBreakPoint = 7f;     //start to break point
        private bool isEvacuation = false;
        private int buildingFloorNumber = 0;

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
			// FixMe: this needs fixing
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
			Debug.Log(currentPlayersDead + " currentPlayersDead");
			Debug.Log(currentPlayersInTruck + " currentPlayersInTruck");
			Debug.Log(playersCount + " playersCount");
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
                    float delay2 = 4f;
                    DOVirtual.DelayedCall(delay2, LoadMainMenu);
                }

                return true;
            }
            return false;
        }

        private void ReactionFadeIn(object param)
        {
            for (var i = 0; i < players.Length; ++i)
            {
				var spawnPos = buildingOut.GetSpawnPosition();
				var pos = spawnPos.HasValue ? new Vector3(spawnPos.Value.x, spawnPos.Value.y, 0) : Vector3.zero;
				if (players[i] == null || players[i].gameObject == null)
				{
					var instance = Instantiate(prefabPlayer);
					instance.transform.position = pos + playerSpawnOffset;
					instance.transform.rotation = Quaternion.identity;
					var player = players[i] = instance.GetComponent<PlayerController>();
					player.SetPlayerId(i);

					AddMultiCameraTarget(instance.gameObject);
					player.InputLayout = (InputHandler.Layout)(playerInputs[i]);
				}
				else
				{
					cameraTargets.Clear();
					AddMultiCameraTarget(players[i].gameObject);
					AddMultiCameraTarget(truckManager.Truck);
					multiTargetCamera.SetTargets(cameraTargets.ToArray());
					players[i].transform.position = pos + playerSpawnOffset;
				}
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
			SaveHiScore();
			Debug.Log("GameOver");
        }

		private void SaveHiScore()
		{
			StartConfig.GetStartConfig().SaveHiScore(pointsCollected);
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

			foreach (var i in truckManager.GetTruckItemList())
			{
				if (i != null)
				{
					i.HideMe();
				}
			}

			if (currentBuildingId > 1)
			{
				truckSpeedModifier *= 2.5f;
				loongo.SetActive(false);
			}
			float delay = Vector2.Distance(placeBuildingOut.transform.position, placeBuildingIn.transform.position) / truckSpeedModifier;
            truckManager.StartTruckMovement(delay);
            DOVirtual.DelayedCall(delay, PhaseTruckStop);
            Debug.Log("PhaseTruckStart");
        }

        private void PhaseTruckStop()
        {
			// fade out?
            events.CallEvent(GamePhases.GameplayPhase.TruckStop, null);
            buildingsGenerator.DestroyBuildingOut(ref buildingOut);

            float delay = 1f;
			DOVirtual.DelayedCall(delay, PhaseDeEvacuation);
			Debug.Log("PhaseTruckStop");
        }

        private void GameSummary()
        {
			SaveHiScore();
			events.CallEvent(GamePhases.GameplayPhase.Summary, null);
            float delay = 2f;

            DOVirtual.DelayedCall(delay, LoadMainMenu);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        private void PhaseDeEvacuation()
        {
			// fade in?
			//DOVirtual.DelayedCall(delay, PhaseStartGame);
			Debug.Log("PhaseDeEvacuation");
			cameraTargets.Clear();
			for (int i = 0; i < currentPlayersInTruck; i++)
			{
				if (players[i] == null)
				{
					return;
				}
				// FixMe : here be bugs
				players[i].transform.position = truckManager.Truck.transform.position + new Vector3(0, 0.5f, 0);
				players[i].ResetPlayer();
				players[i].InputLayout = (InputHandler.Layout)(playerInputs[i]);
				cameraTargets.Add(players[i].gameObject);
				//p.GetComponent<PlayerController>().Setup();
			}
			currentPlayersInTruck = 0;
			//cameraTargets.Add(placeBuildingIn);
			cameraTargets.Add(buildingIn.Floors[(int)buildingIn.Floors.Count / 2].segments[0].gameObject);
			multiTargetCamera.SetTargets(cameraTargets.ToArray());
			cachedTruckItems = new List<Item>();
			foreach (var i in truckManager.GetTruckItemList())
			{
				cachedTruckItems.Add(i);
				i.transform.position = truckManager.Truck.transform.position;
				i.UnKillMe();
			}
			truckManager.ResetTruckItemList();
			Destroy(truckManager.Truck);
			events.CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
			InvokeRepeating("AreThereTruckItemsLeft", 1f, 0.5f);
        }

		private List<Item> cachedTruckItems;
		private bool AreThereTruckItemsLeft()
		{
			foreach (var i in cachedTruckItems)
			{
				if (i != null)
				{
					justInCaseTimer++;
					if (justInCaseTimer > 60)
					{
						justInCaseTimer = 0;
						CancelInvoke();
						ReactionLastItemShot(null);
						return false;
					}
					return true;
				}
			}
			CancelInvoke();
			ReactionLastItemShot(null);
			return false;
		}

		private bool atLeastOneItemInNewBuilding;
		public void RemoveFromCachedList(Item it)
		{
			if (cachedTruckItems == null)
			{
				return;
			}
			if (cachedTruckItems.Contains(it))
			{
				cachedTruckItems.Remove(it);
			}
		}
		public void SetItemInBuilding(Item it)
		{
			if (cachedTruckItems == null)
			{
				return;
			}
			if (cachedTruckItems.Contains(it))
			{
				atLeastOneItemInNewBuilding = true;
			}
		}

		private int justInCaseTimer = 0;

        private void ReactionLastItemShot(object param)
        {
			if (atLeastOneItemInNewBuilding == false)
			{
				GameSummary();
				return;
			}
			//events.CallEvent(GamePhases.GameplayPhase.FadeIn, null);
			atLeastOneItemInNewBuilding = false;
			float delay = 1f;
			DOVirtual.DelayedCall(delay, PhaseManyMonthsLater);
			Debug.Log("FadeIn");
		}

		private int oldFloorCount;
		private FloorSize oldFloorSize;
        private void PhaseManyMonthsLater()
        {
            events.CallEvent(GamePhases.GameplayPhase.ManyMonthsLater, null);
			oldFloorCount = buildingIn.Floors.Count;
			oldFloorSize = buildingIn.FloorSize;
			itemsFromLastInBuilding = buildingIn.GetItems();
			foreach (var i in cachedTruckItems)
			{
				if (i != null)
				{
					Destroy(i.gameObject);
				}
			}
			cachedTruckItems.Clear();
			float delay = 1f;
            DOVirtual.DelayedCall(delay, () => PhaseStartGame(true));
            Debug.Log("PhaseFewDaysLater");
        }
    }
}
