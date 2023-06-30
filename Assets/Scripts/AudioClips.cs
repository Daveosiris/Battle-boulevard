using System;
using UnityEngine;

[Serializable]
public class AudioClips
{
	public string name;

	public float volume = 1f;

	public bool loop;

	public AudioClip[] clip;
}
