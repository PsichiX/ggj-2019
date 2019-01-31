using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
	public class StartConfig : MonoBehaviour
	{
		public Dictionary<int, Outfit> playersChoosenOutfits = new Dictionary<int, Outfit>();
		public List<Sprite> outfitSprites = new List<Sprite>();
		public enum Outfit
		{
			Default,
			Shark
		}
		public Sprite ChooseOutfit(int player, int outfit)
		{
			if (playersChoosenOutfits.ContainsKey(player))
			{
				playersChoosenOutfits[player] = (Outfit)outfit;
			}
			else
			{
				playersChoosenOutfits.Add(player, (Outfit)outfit);
			}
			return outfitSprites[(int)outfit];
		}

		public void SetOutfit(int player, PlayerController pc)
		{
			int outFitChosen = 0;
			if (playersChoosenOutfits.ContainsKey(player))
			{
				outFitChosen = (int)playersChoosenOutfits[player];
			}
			pc.SetOutfit(outFitChosen);
		}

		private static StartConfig _instance;
		public static StartConfig GetStartConfig()
		{
			if (_instance == null)
			{
				var ob = new GameObject("startConfig");
				ob.AddComponent<StartConfig>();
				return ob.GetComponent<StartConfig>();
			}
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
			for (int i = 0; i < playerMaxNumber; i++)
			{
				players[i] = false;
			}
		}

		private int lastPlayer;
		public void ActivePlayer(int playerNumber)
		{
			if (playerNumber > 0 && playerNumber <= playerMaxNumber)
			{
				players[playerNumber - 1] = !players[playerNumber - 1];
			}
		}

		public void DeactivePlayer(int playerNumber)
		{
			if (playerNumber > 0 && playerNumber <= playerMaxNumber)
			{
				players[playerNumber - 1] = false;
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
				if (players[i])
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

}