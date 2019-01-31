using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GaryMoveOut
{
	public class OutfitChooser : MonoBehaviour
	{
		public int playerId;
		private StartConfig startConfig;
		[SerializeField] private Image outfitImage;
		private int currentOutfit = 0;
		private int maxAvailableOutfits;

		private void Start()
		{
			startConfig = StartConfig.GetStartConfig();
			maxAvailableOutfits = Enum.GetValues(typeof(StartConfig.Outfit)).Length -1;
		}

		public void NextOutfit(bool previous = false)
		{
			int dif = previous? -1 : 1;
			currentOutfit += dif;
			if (currentOutfit < 0)
			{
				currentOutfit = 0;
			}
			else if (currentOutfit > maxAvailableOutfits)
			{
				currentOutfit = maxAvailableOutfits;
			}
			outfitImage.sprite = startConfig.ChooseOutfit(playerId, currentOutfit);
		}
	} 
}
