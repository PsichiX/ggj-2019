using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscore
{
	public string playName;
	public int points;
	public Highscore (string name, int points)
	{
		playName = name;
		this.points = points;
	}
}
