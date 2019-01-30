using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    public class GameplayEvents
    {
        private Dictionary<GamePhases.GameplayPhase, Action<object>> eventDict;
        private static GameplayEvents events;
        public static GameplayEvents GetGameplayEvents()
        {
            return events;
        }

        public GameplayEvents()
        {
            eventDict = new Dictionary<GamePhases.GameplayPhase, Action<object>>();
            AddAllEventsToDict();
            events = this;
        }

        private void AddAllEventsToDict()
        {
            foreach (GamePhases.GameplayPhase phases in (GamePhases.GameplayPhase[])Enum.GetValues(typeof(GamePhases.GameplayPhase)))
            {
                eventDict.Add(phases, null);
            }
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

        public event System.Action<GamePhases.GameplayPhase> GameplayPhaseChanged;
        public void CallEvent(GamePhases.GameplayPhase gamePhase, object param)
        {
            if (eventDict.ContainsKey(gamePhase))
            {
                eventDict[gamePhase]?.Invoke(param);
                GameplayPhaseChanged?.Invoke(gamePhase);
            }
        }

        public event System.Action<GameObject> PlayerDied;
        public void CallPlayerDied(GameObject go)
        {
            PlayerDied?.Invoke(go);
        }

        public event System.Action<Items.Item> ItemDestroyed;
        public void CallItemDestroyed(Items.Item item)
        {
            ItemDestroyed?.Invoke(item);
        }

        public event System.Action<Items.Item> ItemAddedToFloor;
        public void CallItemAddedToFloor(Items.Item item)
        {
            ItemAddedToFloor?.Invoke(item);
        }
    }
}
