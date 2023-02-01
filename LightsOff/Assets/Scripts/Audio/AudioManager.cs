using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private float defaultMainVolume = 100f;
	
	public float MainVolume { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		MainVolume = defaultMainVolume;
	}

	public void UpdateMainVolume(float pVolume)
	{
		MainVolume = pVolume;
		AkSoundEngine.SetRTPCValue(Constants.WwiseRTPCMasterVolume, pVolume, AkSoundEngine.AK_INVALID_GAME_OBJECT);
	}

	#region Music methods

	public void StartGameMusic()
	{
		AkSoundEngine.PostEvent(Constants.WwiseEventMusicStart, gameObject);
	}

	public void TransitionInGameMusic()
	{
		AkSoundEngine.PostEvent(Constants.WwiseEventMusicPlayGame, gameObject);
	}

	public void TransitionBackToMenuMusic()
	{
		AkSoundEngine.PostEvent(Constants.WwiseEventMusicBackToMenu, gameObject);
	}

	public void EndGameMusic()
	{
		AkSoundEngine.PostEvent(Constants.WwiseEventMusicEnd, gameObject);
	}

	#endregion

	#region Player SFX methods

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
		//AkSoundEngine.PostEvent(Constants.WwiseEventPlayPlayerJump, pGameObject);
	}

	#endregion
}
