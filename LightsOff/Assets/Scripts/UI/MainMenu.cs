using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

	private Button playButton;
	private Button quitButton;

	protected override void Awake()
	{
		base.Awake();

		playButton = playButtonObject.GetComponent<Button>();
		quitButton = quitButtonObject.GetComponent<Button>();

		playButton.interactable = false;
	}

	private void Start()
	{
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
	}

	private void OnEnable()
	{
		LevelLoader.Instance.CrossfadeTransitionEnded += ActivateMenu;
	}

	private void OnDisable()
	{
		LevelLoader.Instance.CrossfadeTransitionEnded -= ActivateMenu;
	}

	private void ActivateMenu()
	{
		playButton.interactable = true;
		InputManager.Instance.EnablePlayerInput();
	}

	public void PlayGame()
	{
		playButton.interactable = false;
		GameManager.Instance.LoadGame();
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
		GameManager.Instance.DeleteSaveFile();
	}
}

public enum Menu
{
	Main,
	Controls,
	Settings,
	None
}