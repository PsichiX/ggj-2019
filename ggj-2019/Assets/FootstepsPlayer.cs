using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
	[SerializeField] private AudioSource aus;
	[SerializeField] private AudioClip[] footstepSounds;
	public void PlayFootstepSound()
	{
		aus.PlayOneShot(footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)]);
	}
}
