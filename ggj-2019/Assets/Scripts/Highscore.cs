using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscore : IComparable<Highscore>
{
	public string playName;
	public int points;
	public Highscore (string name, int points)
	{
		playName = name;
		this.points = points;
	}

	public int CompareTo(Highscore other)
	{
		return other.points.CompareTo(points);
	}
}