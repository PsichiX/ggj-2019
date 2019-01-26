using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void CallEvent(GamePhases.GameplayPhase gamePhase, object param)
    {
        if (eventDict.ContainsKey(gamePhase))
        {
            eventDict[gamePhase]?.Invoke(param);
        }
    }
}
