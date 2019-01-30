using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashText : MonoBehaviour
{
	public float interval;
	public float randomRange;
	private TextMeshProUGUI text;

    void Start()
    {
		text = GetComponent<TextMeshProUGUI>();
		FlashIn();
    }

	private void FlashIn()
	{
		text.DOFade(0, Random.Range(interval - randomRange, interval + randomRange)).OnComplete(FlashOut);
	}

	private void FlashOut()
	{
		text.DOFade(1, Random.Range(interval - randomRange, interval + randomRange)).OnComplete(FlashIn);
	}
}
