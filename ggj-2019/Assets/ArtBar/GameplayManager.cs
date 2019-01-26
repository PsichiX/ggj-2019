using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PhaseStartGame();
    }

    void Update()
    {

        EvacuationProcess();
        
    }

    public bool AttachToEvent(GamePhases.GameplayPhase gamePhase, Action<System.Object> action)
    {
        if (eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase] += action;
            return true;
        }
        return false;
    }

    public void CallEvent(GamePhases.GameplayPhase gamePhase, System.Object param)
    {
        if(eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase].Invoke(param);
        }
    }

    private void PhaseStartGame()
    {
        SetupNewBuilding();
        CallEvent(GamePhases.GameplayPhase.FadeIn, null);
        //delay call 1s PhaseBadEventStart()
    }

    private void SetupNewBuilding()
    {

    }
   
    private void PhaseBadEventStart()
    {
        CallEvent(GamePhases.GameplayPhase.BadEventStart, null);
        float delay = 2f;
        //delay call 1s   PhaseStartEvacuation();
    }

    private void PhaseStartEvacuation()
    {
        CallEvent(GamePhases.GameplayPhase.Evacuation, null);
        isEvacuation = true;
    }

    private void PhaseFloorEvacuationStart(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationStart, floor);
    }

    private void PhaseFloorEvacuationBreakPoint(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationBreakPoint, floor);
    }

    private void PhaseFloorEvacuationEnd(int floor)
    {
        CallEvent(GamePhases.GameplayPhase.FloorEvacuationEnd, floor);
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
        if(currentFloorBadEvent > buildingFloorNumber)
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

    private void ReactionPlayerJump(System.Object param)
    {
        EndEvacuation();
    }

    private void ReactionPlayerInTruck(System.Object param)
    {
        PhaseTruckStart();
    }

    private void PhaseTruckStart()
    {
        CallEvent(GamePhases.GameplayPhase.TruckStart, null);
        float delay = 2f;
        //delay call 1s   PhaseTruckStop();
    }

    private void PhaseTruckStop()
    {
        CallEvent(GamePhases.GameplayPhase.TruckStop, null);
        float delay = 1f;
        //delay call PhaseDeEvacuation();
    }

    private void PhaseDeEvacuation()
    {
        CallEvent(GamePhases.GameplayPhase.DeEvacuation, null);
    }

    private void ReactionLastItemShot(System.Object param)
    {
        CallEvent(GamePhases.GameplayPhase.FadeOut, null);
        float delay = 1f;
        //delay call PhaseFewDaysLater();
    }

    private void PhaseFewDaysLater()
    {
        CallEvent(GamePhases.GameplayPhase.FewDaysLater, null);
        float delay = 1f;
        //delay call PhaseStartGame();
    }

}
