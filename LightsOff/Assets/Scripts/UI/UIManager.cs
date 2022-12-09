using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private TMP_Text levelNumberText = default;

	private void Start()
	{
		InputManager.Instance.Paused += ctx => PauseButtonPressed(ctx);
		InputManager.Instance.Backed += ctx => BackButtonPressed(ctx);
	}

	private void PauseButtonPressed(InputAction.CallbackContext ctx)
	{
		if (GameManager.Instance.CurrentGameState == GameState.Paused)
		{
			PauseMenu.Instance.Resume();
		}
		else if (GameManager.Instance.CurrentGameState == GameState.Playing && PlayerController.Instance.CurrentCharacterState != CharacterState.LevelTransition)
		{
			PauseMenu.Instance.Pause();
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(PauseMenu.Instance.pauseFirstButton);
		}
	}

	private void BackButtonPressed(InputAction.CallbackContext ctx)
	{
		switch (GameManager.Instance.CurrentGameState)
		{
			case GameState.Paused:

				PauseMenu.Instance.Resume();
				break;

			case GameState.Menu:

				if (MainMenu.Instance.GetCurrentMenu() == Menu.Controls)
				{
					MainMenu.Instance.CloseControlsMenu();
				}
				else if (MainMenu.Instance.GetCurrentMenu() == Menu.Settings)
				{
					MainMenu.Instance.CloseSettingsMenu();
				}

				break;

			default:
				break;
		}
	}

	public void UpdateLevelNumberText(int pLevelNumber)
	{
		if (pLevelNumber == 0)
		{
			levelNumberText.enabled = false;
		}
		else
		{
			levelNumberText.text = Constants.UILevelNumberText + pLevelNumber;
			levelNumberText.enabled = true;
		}
	}
}
