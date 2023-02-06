using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] private float defaultMasterVolume = 100f;
	[SerializeField] private float defaultMusicVolume = 80f;
	[SerializeField] private float defaultSFXVolume = 80f;
	[SerializeField] private float defaultUIVolume = 80f;
	[SerializeField] public float sfxCooldown = 0.05f;

	public float MasterVolume { get; private set; }
	public float MusicVolume { get; private set; }
	public float SFXVolume { get; private set; }
	public float UIVolume { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		MasterVolume = defaultMasterVolume;
		MusicVolume = defaultMusicVolume;
		SFXVolume = defaultSFXVolume;
		UIVolume = defaultUIVolume;
	}

	public void TriggerWwiseEvent(string pEventName, GameObject pGameObject)
	{
		AkSoundEngine.PostEvent(pEventName, pGameObject);
	}

	// This overload of the method should always be used when the Wwise Event Action scope is global (like transitions)
	public void TriggerWwiseEvent(string pEventName)
	{
		TriggerWwiseEvent(pEventName, gameObject);
	}

	private void SetWwiseRTPC(string pRTPCName, float pValue)
	{
		AkSoundEngine.SetRTPCValue(pRTPCName, pValue, AkSoundEngine.AK_INVALID_GAME_OBJECT);
	}

	public void AssignEmitterToPlayerListener(GameObject pGameObject)
	{
		if (PlayerController.IsInitialized)
		{
			ulong[] listenersArray = new ulong[1];
			listenersArray[0] = PlayerController.Instance.PlayerAkAudioListener.GetAkGameObjectID();
			AkSoundEngine.SetListeners(pGameObject, listenersArray, 1);
		}
	}

	public void UpdateMasterVolume(float pVolume)
	{
		MasterVolume = pVolume;
		SetWwiseRTPC(Constants.WwiseRTPCMasterVolume, pVolume);
	}

	public void UpdateMusicVolume(float pVolume)
	{
		MusicVolume = pVolume;
		SetWwiseRTPC(Constants.WwiseRTPCMusicVolume, pVolume);
	}

	public void UpdateSFXVolume(float pVolume)
	{
		SFXVolume = pVolume;
		SetWwiseRTPC(Constants.WwiseRTPCSFXVolume, pVolume);
	}

	public void UpdateUIVolume(float pVolume)
	{
		UIVolume = pVolume;
		SetWwiseRTPC(Constants.WwiseRTPCUIVolume, pVolume);
	}
}
