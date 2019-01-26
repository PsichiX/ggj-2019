using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayTest : MonoBehaviour
{
    private GameplayManager gamplayManager;
    void Start()
    {
        gamplayManager = GameplayManager.GetGameplayManager();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gamplayManager.AttachToEvent(GamePhases.GameplayPhase.FloorEvacuationStart, EvacuationStart);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            gamplayManager.DetachFromEvent(GamePhases.GameplayPhase.FloorEvacuationStart, EvacuationStart);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            int level = 77;
            gamplayManager.CallEvent(GamePhases.GameplayPhase.FloorEvacuationStart, level);
        }
    }

    private void EvacuationStart(System.Object param)
    {
        if(param is int)
        {
            int level = (int)param;

            Debug.Log($"ddddd level {level}");
        }

    }



}
