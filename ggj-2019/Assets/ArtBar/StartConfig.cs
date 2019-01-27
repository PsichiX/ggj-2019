using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConfig : MonoBehaviour
{
    private static StartConfig _instance;
    public static StartConfig GetStartConfig()
    {
        return _instance;
    }

    public bool[] players;
    private int playerMaxNumber = 3;

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
            DontDestroyOnLoad(this);
            InitPlayers();
        }

    }

    private void InitPlayers()
    {
        players = new bool[playerMaxNumber];
        for(int i=0; i<playerMaxNumber; i++)
        {
            players[i] = false;
        }
    }

	private int lastPlayer;
    public void ActivePlayer(int playerNumber)
    {
        if( playerNumber > 0 && playerNumber <= playerMaxNumber)
        {
            players[playerNumber-1] = !players[playerNumber - 1];
        }
    }

    public void DeactivePlayer(int playerNumber)
    {
        if (playerNumber > 0 && playerNumber <= playerMaxNumber)
        {
            players[playerNumber-1] = false;
        }
    }
    
    public int GetMaxPlayerNumber()
    {
        return playerMaxNumber;
    }

    public int GetActivePlayerCount()
    {
        int activePlayersCounter = 0;
        for (int i = 0; i < playerMaxNumber; i++)
        {
            if( players[i] )
            {
                activePlayersCounter++;
            }
        }
        return activePlayersCounter;
    }

    public bool IsPlayerActive(int playerNumber)
    {
        if (playerNumber > 0 && playerNumber <= playerMaxNumber)
        {
            return players[playerNumber - 1];
        }
        return false;
    }
}
