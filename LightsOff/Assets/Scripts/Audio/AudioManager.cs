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

	public void PlayPlayerWalk(GameObject pGameObject)
	{
		//AkSoundEngine.PostEvent(Constants.WwiseEventPlayPlayerWalk, pGameObject);
	}

	public void StopPlayerWalk(GameObject pGameObject)
	{
		//AkSoundEngine.PostEvent(Constants.WwiseEventStopPlayerWalk, pGameObject);
	}

	public void PlayPlayerJump(GameObject pGameObject)
	{
		AkSoundEngine.PostEvent(Constants.WwiseEventPlayPlayerJump, pGameObject);
	}
}
