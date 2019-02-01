using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
	public class Highscores : MonoBehaviour
	{
		[SerializeField] private HiScore hiScorePrefab;

		private void Start()
		{
			StartConfig.GetStartConfig().FillHiScores();
		}

		public void Fill(string play)
		{
			var comp = Instantiate(hiScorePrefab, transform);
			comp.AddHiScore(play);
		}
	}

}