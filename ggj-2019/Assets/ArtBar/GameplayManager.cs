using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager _instance;
    public static GameplayManager GetGameplayManager()
    {
        return _instance;
    }

    private GameplayEvents events;

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

        events = new GameplayEvents();
    }

    private void Start()
    {
        AttachReactionsToEvents();
        PhaseStartGame();
    }

    private void AttachReactionsToEvents()
    {
        events.AttachToEvent(GamePhases.GameplayPhase.PlayerJump, ReactionPlayerJump);
        events.AttachToEvent(GamePhases.GameplayPhase.PlayerInTruck, ReactionPlayerInTruck);
        events.AttachToEvent(GamePhases.GameplayPhase.LastItemShot, ReactionLastItemShot);
        events.AttachToEvent(GamePhases.GameplayPhase.PlayerDie, ReactionPlayerDie);
        events.AttachToEvent(GamePhases.GameplayPhase.GameOver, ReactionGameOver);
        events.AttachToEvent(GamePhases.GameplayPhase.StartNewGame, PhaseStartNewGame);
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
        SetupNewBuilding();
        events.CallEvent(GamePhases.GameplayPhase.FadeIn, null);
        float fadeDealy = 1f;
        DOVirtual.DelayedCall(fadeDealy, PhaseBadEventStart);
        Debug.Log("PhaseStart");
    }

    private void SetupNewBuilding()
    {

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

    private void PhaseTruckStart()
    {
        events.CallEvent(GamePhases.GameplayPhase.TruckStart, null);
        float delay = 2f;
        DOVirtual.DelayedCall(delay, PhaseTruckStop);
        Debug.Log("PhaseTruckStart");
    }

    private void PhaseTruckStop()
    {
        events.CallEvent(GamePhases.GameplayPhase.TruckStop, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseDeEvacuation);
        Debug.Log("PhaseTruckStop");
    }

    private void PhaseDeEvacuation()
    {
        events.CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
        Debug.Log("PhaseDeEvacuation");
    }

    private void ReactionLastItemShot(object param)
    {
        events.CallEvent(GamePhases.GameplayPhase.FadeOut, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseFewDaysLater);
        Debug.Log("FadeOut");
    }

    private void PhaseFewDaysLater()
    {
        events.CallEvent(GamePhases.GameplayPhase.FewDaysLater, null);
        float delay = 1f;
        DOVirtual.DelayedCall(delay, PhaseStartGame);
        Debug.Log("PhaseFewDaysLater");
    }

}
