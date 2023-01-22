﻿using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private CanvasGroup inGameUICanvasGroup = default;
	[SerializeField] private TMP_Text levelNumberText = default;
	[SerializeField] private TMP_Text keyNumberText = default;
	[SerializeField] private float fadeTime = 0.5f;

	public Sprite interactMessageImageKeyboard = default;
	public Sprite interactMessageImageController = default;
	
	public Color deviceOutlineColor = Color.white;

	private bool uiIsFadingIn;
	private float fadeCounter;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		InputManager.Instance.Paused += ctx => PauseButtonPressed(ctx);
		InputManager.Instance.Backed += ctx => BackButtonPressed(ctx);
	}

	private void Update()
	{
		if (uiIsFadingIn)
		{
			inGameUICanvasGroup.alpha += Time.deltaTime / fadeTime;

			fadeCounter -= Time.deltaTime;

			if (fadeCounter <= 0 && (inGameUICanvasGroup.alpha <= 0 || inGameUICanvasGroup.alpha >= 1))
				uiIsFadingIn = false;
		}
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
		levelNumberText.text = Constants.UILevelNumberText + pLevelNumber;
	}

	public void UpdateKeyNumberText(int pKeyNumber)
	{
		keyNumberText.text = Constants.UIKeyNumberText + pKeyNumber;
	}

	public void EnableInGameUI(bool pEnable)
	{
		if (pEnable)
		{
			uiIsFadingIn = true;
			fadeCounter = fadeTime;
		}
		else
		{
			inGameUICanvasGroup.alpha = 0;
		}
	}
}
