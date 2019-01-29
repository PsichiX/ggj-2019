using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToggle : MonoBehaviour
{
	[SerializeField] private Toggle player1;
	[SerializeField] private Toggle player2;
	[SerializeField] private Toggle player3;

	private StartConfig startConfig;
	void Start()
    {
		startConfig = StartConfig.GetStartConfig();
		for (int i = 0; i < startConfig.players.Length; i++)
		{
			startConfig.DeactivePlayer(i);
		}
		//player1.isOn = startConfig.IsPlayerActive(1);
		//player2.isOn = startConfig.IsPlayerActive(2);
		//player3.isOn = startConfig.IsPlayerActive(3);

	}

	public void SetPlayerActive(int whichPlayer)
	{
		if (startConfig == null)
		{
			startConfig = StartConfig.GetStartConfig();
		}
		startConfig.ActivePlayer(whichPlayer);
	}
}
