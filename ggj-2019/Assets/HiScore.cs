using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HiScore : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI txt;
    
	public void AddHiScore(string text)
	{
		txt.text = text;
	}
}
