using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class MainMenu : Singleton<MainMenu>
{
	[SerializeField] private GameObject mainMenuPanel = default;
	[SerializeField] private GameObject controlsMenuPanel = default;
	[SerializeField] private GameObject settingsMenuPanel = default;

	[SerializeField] private GameObject mainMenuFirstButton = default;
	[SerializeField] private GameObject controlsFirstButton = default;
	[SerializeField] private GameObject controlsClosedButton = default;
	[SerializeField] private GameObject settingsFirstButton = default;
	[SerializeField] private GameObject settingsClosedButton = default;

	[SerializeField] private GameObject playButtonObject = default;
	[SerializeField] private GameObject quitButtonObject = default;

	[SerializeField] private CanvasGroup startMenuCanvasGroup;
	[SerializeField] private CanvasGroup saveFileDeletedCanvasGroup = default;
	[SerializeField] private TMP_Text saveFileDeletedText = default;
	[SerializeField] private GameObject startMenuGlobalVolume;

	[SerializeField] private float saveFileDeletedMessageFadeTime = 0.5f;
	[SerializeField] private float saveFileDeletedMessageWaitTime = 4f;

	private DepthOfField backgroundBlur;

	private Button playButton;
	private Button quitButton;

	private SaveFileDeletedMessageState messageState;
	private float saveFileDeletedMessageFadeCounter;

	protected override void Awake()
	{
		base.Awake();

		playButton = playButtonObject.GetComponent<Button>();
		quitButton = quitButtonObject.GetComponent<Button>();

		startMenuGlobalVolume.GetComponent<Volume>().profile.TryGet(out backgroundBlur);

		playButton.interactable = false;

		messageState = SaveFileDeletedMessageState.Hidden;
	}

	private void Start()
	{
		settingsMenuPanel.SetActive(false);
		controlsMenuPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
	}

	private void Update()
	{
		if (messageState != SaveFileDeletedMessageState.Hidden)
		{
			if (messageState == SaveFileDeletedMessageState.FadingIn)
				saveFileDeletedCanvasGroup.alpha += Time.deltaTime / saveFileDeletedMessageFadeTime;
			else if (messageState == SaveFileDeletedMessageState.FadingOut)
				saveFileDeletedCanvasGroup.alpha -= Time.deltaTime / saveFileDeletedMessageFadeTime;

			saveFileDeletedMessageFadeCounter -= Time.deltaTime;

			if (saveFileDeletedMessageFadeCounter <= 0 && (saveFileDeletedCanvasGroup.alpha >= 1 || saveFileDeletedCanvasGroup.alpha <= 0))
			{
				if (messageState == SaveFileDeletedMessageState.FadingIn)
				{
					messageState = SaveFileDeletedMessageState.Waiting;
					saveFileDeletedMessageFadeCounter = saveFileDeletedMessageWaitTime;
				}
				else if (messageState == SaveFileDeletedMessageState.Waiting)
				{
					messageState = SaveFileDeletedMessageState.FadingOut;
					saveFileDeletedMessageFadeCounter = saveFileDeletedMessageFadeTime;
				}
				else if (messageState == SaveFileDeletedMessageState.FadingOut)
				{
					messageState = SaveFileDeletedMessageState.Hidden;
				}
			}
		}
	}

	private void OnEnable()
	{
		TransitionManager.Instance.CrossfadeTransitionEnded += ActivateMenu;
	}

	private void OnDisable()
	{
		TransitionManager.Instance.CrossfadeTransitionEnded -= ActivateMenu;
	}

	private void ActivateMenu()
	{
		playButton.interactable = true;
		EventSystemManager.Instance.ActivateModule();
	}

	public void PlayGame()
	{
		playButton.interactable = false;
		GameManager.Instance.StartGame();
	}

	public Menu GetCurrentMenu()
	{
		if (mainMenuPanel.activeSelf)
		{
			return Menu.Main;
		}
		else if (controlsMenuPanel.activeSelf)
		{
			return Menu.Controls;
		}
		else if (settingsMenuPanel.activeSelf)
		{
			return Menu.Settings;
		}
		else
		{
			return Menu.None;
		}
	}

	public void OpenSettingsMenu()
	{
		mainMenuPanel.SetActive(false);
		settingsMenuPanel.SetActive(true);

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(settingsFirstButton);
	}

	public void CloseSettingsMenu()
	{
		mainMenuPanel.SetActive(true);
		settingsMenuPanel.SetActive(false);

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(settingsClosedButton);
	}

	public void OpenControlsMenu()
	{
		mainMenuPanel.SetActive(false);
		controlsMenuPanel.SetActive(true);

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(controlsFirstButton);
	}

	public void CloseControlsMenu()
	{
		mainMenuPanel.SetActive(true);
		controlsMenuPanel.SetActive(false);

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(controlsClosedButton);
	}

	public void QuitGame()
	{
		quitButton.interactable = false;
		GameManager.Instance.UnloadToQuit();
	}

	public void DeleteSaveFile()
	{
		GameManager.Instance.DeleteSaveFileFromMenu();
	}

	public void DisplaySaveFileDeletedMessage()
	{
		if (messageState == SaveFileDeletedMessageState.Hidden)
		{
			saveFileDeletedText.text = Constants.UISaveFileDeletedMessage;
			messageState = SaveFileDeletedMessageState.FadingIn;
			saveFileDeletedMessageFadeCounter = saveFileDeletedMessageFadeTime;
		}
	}

	public void DisplayNoSaveFileFoundMessage()
	{
		if (messageState == SaveFileDeletedMessageState.Hidden)
		{
			saveFileDeletedText.text = Constants.UINoSaveFileFoundMessage;
			messageState = SaveFileDeletedMessageState.FadingIn;
			saveFileDeletedMessageFadeCounter = saveFileDeletedMessageFadeTime;
		}
	}

	public void AddToCanvasGroupAlpha(float pDifference)
	{
		startMenuCanvasGroup.alpha += pDifference;
	}

	public void AddToDepthOfFieldFocalLength(float pDifference)
	{
		backgroundBlur.focalLength.value += pDifference;
		backgroundBlur.focalLength.overrideState = true;
	}
}

public enum Menu
{
	Main,
	Controls,
	Settings,
	None
}

public enum SaveFileDeletedMessageState
{
	Hidden,
	FadingIn,
	Waiting,
	FadingOut
}
