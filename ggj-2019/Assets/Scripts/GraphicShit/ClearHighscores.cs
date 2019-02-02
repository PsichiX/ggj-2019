using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
	public class ClearHighscores : MonoBehaviour
	{
		public void Clear()
		{
			StartConfig.GetStartConfig().ClearHighscores();
		}
	} 
}
