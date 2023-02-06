using UnityEngine;

public class UIAudio : MonoBehaviour
{
	[SerializeField] private UIWwiseEvent uiWwiseEvent = default;

	private float uiSFXTimer;

	private void Update()
	{
		uiSFXTimer -= Time.deltaTime;
	}

	public void TriggerWwiseEvent()
	{
		if (uiSFXTimer <= 0)
		{
			AudioManager.Instance.TriggerWwiseEvent(GetWwiseEventName(uiWwiseEvent), gameObject);
			uiSFXTimer = AudioManager.Instance.sfxCooldown;
		}
	}

	public void TriggerSelectionMoveWwiseEvent()
	{
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayMenuSelectionMove, gameObject);
	}

	private string GetWwiseEventName(UIWwiseEvent pUIWwiseEvent)
	{
		switch (pUIWwiseEvent)
		{
			case UIWwiseEvent.PlayMenuChangeSetting:
				return Constants.WwiseEventPlayMenuChangeSetting;
			case UIWwiseEvent.PlayMenuButtonPress:
				return Constants.WwiseEventPlayMenuButtonPress;
			case UIWwiseEvent.PlayMenuPlay:
				return Constants.WwiseEventPlayMenuPlay;
			case UIWwiseEvent.PlayMenuQuit:
				return Constants.WwiseEventPlayMenuQuit;
			case UIWwiseEvent.PlayMenuDeleteSaveFileButtonPress:
				return Constants.WwiseEventPlayMenuDeleteSaveFileButtonPress;
			case UIWwiseEvent.PlayMenuSelectionMove:
				return Constants.WwiseEventPlayMenuSelectionMove;
			case UIWwiseEvent.PlayMenuButtonBackPress:
				return Constants.WwiseEventPlayMenuButtonBackPress;
			case UIWwiseEvent.PlayPauseMenuRetry:
				return Constants.WwiseEventPlayPauseMenuRetry;
			default:
				return null;
		}
	}
}

public enum UIWwiseEvent
{
	PlayMenuPlay,
	PlayMenuQuit,
	PlayMenuButtonPress,
	PlayMenuButtonBackPress,
	PlayMenuDeleteSaveFileButtonPress,
	PlayMenuSelectionMove,
	PlayMenuChangeSetting,
	PlayPauseMenuRetry
}