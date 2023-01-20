using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	private AudioSource music;

	protected override void Awake()
	{
		base.Awake();

		music = GetComponent<AudioSource>();
	}

	public void PlayMusic()
	{
		music.Play();
	}
}
