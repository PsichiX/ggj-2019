using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameplayManager : MonoBehaviour
{
    private Dictionary<GamePhases.GameplayPhase, Action<System.Object>> eventDict = new Dictionary<GamePhases.GameplayPhase, Action<System.Object>>();

    private static GameplayManager _instance;
    public static GameplayManager GetGameplayManager()
    {
        return _instance;
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        AddAllEventsToDict();
    }

    private void AddAllEventsToDict()
    {
        foreach (GamePhases.GameplayPhase phases in (GamePhases.GameplayPhase[])Enum.GetValues(typeof(GamePhases.GameplayPhase)))
        {
            eventDict.Add(phases, null);
        }
    }

    private void Start()
    {
        AttachReactionsToEvents();
        PhaseStartGame();
    }

    private void AttachReactionsToEvents()
    {
        AttachToEvent(GamePhases.GameplayPhase.PlayerJump, ReactionPlayerJump);
        AttachToEvent(GamePhases.GameplayPhase.PlayerInTruck, ReactionPlayerInTruck);
        AttachToEvent(GamePhases.GameplayPhase.LastItemShot, ReactionLastItemShot);
        AttachToEvent(GamePhases.GameplayPhase.PlayerDie, ReactionPlayerDie);
        AttachToEvent(GamePhases.GameplayPhase.GameOver, ReactionGameOver);
        AttachToEvent(GamePhases.GameplayPhase.StartNewGame, PhaseStartNewGame);
    }

    void Update()
    {

        EvacuationProcess();

    }

    public bool AttachToEvent(GamePhases.GameplayPhase gamePhase, Action<object> action)
    {
        if (eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase] += action;
            return true;
        }
        return false;
    }

    public bool DetachFromEvent(GamePhases.GameplayPhase gamePhase, Action<object> action)
    {
        if (eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase] -= action;
            return true;
        }
        return false;
    }

    public void CallEvent(GamePhases.GameplayPhase gamePhase, object param)
    {
        if (eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase]?.Invoke(param);
        }
    }

    private void PhaseStartNewGame(object param)
    {
        PhaseStartGame();
    }

    private void PhaseStartGame()
    {
        SetupNewBuilding();
        CallEvent(GamePhases.GameplayPhase.FadeIn, null);
        float fadeDealy = 1f;
        DOVirtual.DelayedCall(fadeDealy, PhaseBadEventStart);
        Debug.Log("PhaseStart");
    }

    private void SetupNewBuilding()
    {

    }

    private void PhaseBadEventStart()
    {
        CallEvent(GamePhases.GameplayPhase.BadEventStart, null);
        float delay = 2f;
        DOVirtual.DelayedCall(delay, PhaseStartEvacuation);
        Debug.Log("PhaseBadEventStart");
    }

    private void PhaseStartEvacuation()
    {
        CallEvent(GamePhases.GameplayPhase.Evacuation, null);
        Debug.Log("PhaseStartEvacuation");
        isEvacuation = true;
        currentFloorBadEvent = -1;
        NextFloorBadEvent();
    }

    private void PhaseFloorEvacuationStart(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationStart, floor);
        Debug.Log($"PhaseFloorEvacuationStart Floor [{floor}]");
    }

    private void PhaseFloorEvacuationBreakPoint(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationBreakPoint, floor);
        Debug.Log($"PhaseFloorEvacuationBreakPoint Floor [{floor}]");
    }

    private void PhaseFloorEvacuationEnd(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, floor);
        Debug.Log($"PhaseFloorEvacuationEnd Floor [{floor}]");
    }

    public int buildingFloorNumber = 4; //0,1,2,3,4
    public int currentFloorBadEvent = 0;
    private bool isEvacuation = false;
    private float evacuationTimeStartToBreakPoint = 5f;     //start to break point
    private float evacuationTimeStartToEnd = 10f;           //start to end

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
            CallEvent(GamePhases.GameplayPhase.GameOver, null);
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

    private void ReactionPlayerDie(object param)
    {
        EndEvacuation();
        float delay = 1f;
        DOVirtual.DelayedCall(delay, () => CallEvent(GamePhases.GameplayPhase.GameOver, null));       
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

    private void PhaseTruckStart()
    {
        CallEvent(GamePhases.GameplayPhase.TruckStart, null);
        float delay = 2f;
        DOVirtual.DelayedCall(delay, PhaseTruckStop);
        Debug.Log("PhaseTruckStart");
    }

    private void PhaseTruckStop()
    {
        CallEvent(GamePhases.GameplayPhase.TruckStop, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseDeEvacuation);
        Debug.Log("PhaseTruckStop");
    }

    private void PhaseDeEvacuation()
    {
        CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
        Debug.Log("PhaseDeEvacuation");
    }

    private void ReactionLastItemShot(object param)
    {
        CallEvent(GamePhases.GameplayPhase.FadeOut, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseFewDaysLater);
        Debug.Log("FadeOut");
    }

    private void PhaseFewDaysLater()
    {
        CallEvent(GamePhases.GameplayPhase.FewDaysLater, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseStartGame);
        Debug.Log("PhaseFewDaysLater");
    }

}
