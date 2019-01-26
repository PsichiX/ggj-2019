using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class GameplayTest : MonoBehaviour
    {
        private GameplayEvents gamplayEvents;
        void Start()
        {
            gamplayEvents = GameplayEvents.GetGameplayEvents();
        }


        void Update()
        {
            /*
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
            }*/

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gamplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerJump, null);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gamplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerInTruck, null);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gamplayEvents.CallEvent(GamePhases.GameplayPhase.LastItemShot, null);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                gamplayEvents.CallEvent(GamePhases.GameplayPhase.PlayerDie, null);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                gamplayEvents.CallEvent(GamePhases.GameplayPhase.StartNewGame, null);
            }

        }

        private void EvacuationStart(System.Object param)
        {
            if (param is int)
            {
                int level = (int)param;

                Debug.Log($"ddddd level {level}");
            }

        }



    }
}