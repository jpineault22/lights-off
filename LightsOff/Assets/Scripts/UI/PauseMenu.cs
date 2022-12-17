using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu>
{
	[SerializeField] private GameObject pauseMenuPanel = default;
	[SerializeField] private GameObject retryButtonObject = default;
	[SerializeField] private GameObject quitToMenuButtonObject = default;
	[SerializeField] private GameObject quitGameButtonObject = default;

	public GameObject pauseFirstButton = default;

	private Button retryButton;
	private Button quitToMenuButton;
	private Button quitGameButton;

	protected override void Awake()
	{
		base.Awake();

		retryButton = retryButtonObject.GetComponent<Button>();
		quitToMenuButton = quitToMenuButtonObject.GetComponent<Button>();
		quitGameButton = quitGameButtonObject.GetComponent<Button>();
	}

	public void Pause()
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing)
		{
			pauseMenuPanel.SetActive(true);
			GameManager.Instance.PauseGame();
		}
	}

	public void Resume()
	{
		pauseMenuPanel.SetActive(false);
		retryButton.interactable = true;
		quitToMenuButton.interactable = true;
		GameManager.Instance.UnpauseGame();
	}

	public void Retry()
	{
		retryButton.interactable = false;
		GameManager.Instance.ReloadLevel();
	}

	public void LoadMenu()
	{
		quitToMenuButton.interactable = false;
		retryButton.interactable = false;
		GameManager.Instance.QuitToMenu();
	}

	public void QuitGame()
	{
		quitGameButton.interactable = false;
		GameManager.Instance.UnloadToQuit();
	}
}
