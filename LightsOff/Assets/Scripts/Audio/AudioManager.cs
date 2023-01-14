using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	protected override void Awake()
	{
		base.Awake();

		AudioSource music = GetComponent<AudioSource>();

		music.Play();
	}
}
