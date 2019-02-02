using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GaryMoveOut
{
	public class StartConfig : MonoBehaviour
	{
		public Dictionary<int, Outfit> playersChoosenOutfits = new Dictionary<int, Outfit>();
		public OutfitIconsDatabase outfitIcons;
		private string hiScoresPath;
		private const string defaultGameName = "UnnamedPlayer";
		private const string startConfigName = "startConfig";
		private const string outfitIconsDatabasePath = "Databases/OutfitIconsDatabase";
		private const string emptyHighScoreList = "No highscores!";
		private const string colonString = " : ";
		private const char hashChar = '#';
		private const char colonChar = ':';

		public bool[] players;
		private int playerMaxNumber = 3;
		private static StartConfig _instance;
		private string gameName;
		private List<Highscore> allHighScores = new List<Highscore>();

		public enum Outfit
		{
			Default,
			Shark
		}

		void Awake()
		{
			hiScoresPath = Application.dataPath + "/StreamingAssets/hiscore.dat";
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
			CheckHighscoresFileAndCreate();
		}

		public static StartConfig GetStartConfig()
		{
			if (_instance == null)
			{
				var ob = new GameObject(startConfigName);
				ob.AddComponent<StartConfig>();
				return ob.GetComponent<StartConfig>();
			}
			return _instance;
		}

		private void InitPlayers()
		{
			players = new bool[playerMaxNumber];
			for (int i = 0; i < playerMaxNumber; i++)
			{
				players[i] = false;
			}
		}

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

		public Sprite ChooseOutfit(int player, int outfit)
		{
			if (outfitIcons == null)
			{
				outfitIcons = Resources.Load(outfitIconsDatabasePath) as OutfitIconsDatabase;
			}
			if (playersChoosenOutfits.ContainsKey(player))
			{
				playersChoosenOutfits[player] = (Outfit)outfit;
			}
			else
			{
				playersChoosenOutfits.Add(player, (Outfit)outfit);
			}
			return outfitIcons.outfitSprites[(int)outfit];
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

		public void ChangeGameName(TMPro.TMP_InputField field)
		{
			gameName = field.text;
		}

		public void SaveHiScore(int score)
		{
			if (gameName == null || gameName == string.Empty)
			{
				gameName = defaultGameName;
			}
			string previousScores = ReadHighscores();
			File.WriteAllText(hiScoresPath, previousScores + hashChar + gameName + colonChar + score.ToString());
		}

		private string ReadHighscores()
		{
			return File.ReadAllText(hiScoresPath);
			
		}

		private void CheckHighscores()
		{
			string[] hiscoresPlays = ReadHighscores().Split(new char[] { hashChar, colonChar }, StringSplitOptions.RemoveEmptyEntries);
			allHighScores.Clear();
			for (int i = 0; i < hiscoresPlays.Length; i += 2)
			{
				allHighScores.Add(new Highscore(hiscoresPlays[i], int.Parse(hiscoresPlays[i + 1])));
			}
		}

		public void FillHiScores()
		{
			CheckHighscores();
			HighscoresPanel hScores = FindObjectOfType<HighscoresPanel>();
			if (hScores == null)
			{
				return;
			}
			if (allHighScores.Count == 0)
			{
				hScores.Fill(emptyHighScoreList);
				return;
			}
			allHighScores.OrderBy(h => h.points).ToList();
			foreach (var score in allHighScores)
			{
				hScores.Fill(score.playName + colonString + score.points);
			}
		}

		private void CheckHighscoresFileAndCreate()
		{
			if (File.Exists(hiScoresPath) == false)
			{
				File.Create(hiScoresPath);
			}
		}
	}
}