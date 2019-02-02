using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
	public class HighscoresPanel : MonoBehaviour
	{
		[SerializeField] private HiScoreUI hiScorePrefab;

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