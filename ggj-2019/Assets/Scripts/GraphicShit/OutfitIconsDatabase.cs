using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Icons database")]
public class OutfitIconsDatabase : ScriptableObject
{
	public List<Sprite> outfitSprites = new List<Sprite>();
}
