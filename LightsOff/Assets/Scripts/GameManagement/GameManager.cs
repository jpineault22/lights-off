﻿using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private bool isTestMode = default;

	public GameState CurrentGameState { get; set; }
	[HideInInspector] public GameObject player;

	private Light[] levelLights;
	private Door door;

	public bool EndingGame { get; set; }

	public event Action PlayerSpawned;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		LevelLoader.Instance.LastSceneUnloaded += DestroyToQuit;

		DisplayPresentationScreen();
	}

	#region Game saving and loading, pause

	public void SaveGame(int pCurrentLevelNumber)
	{
		if (isTestMode)
		{
			Debug.Log("Cannot save while in test mode.");
		}
		else
		{
			Debug.Log("Saving in " + Constants.NamePrefixSceneLevel + pCurrentLevelNumber);

			PlayerData data = new PlayerData(pCurrentLevelNumber);

			SaveSystem.SavePlayerData(data);
		}
		
	}

	public int GetSavedLevelNumber()
	{
		PlayerData data = SaveSystem.LoadPlayerData();
		int savedLevelNumber = Constants.StartingLevelNumber;

		if (data != null && !isTestMode)
		{
			savedLevelNumber = data.CurrentLevelNumber;
		}

		return savedLevelNumber;
	}

	public void DeleteSaveFileFromMenu()
	{
		DeleteSaveFile();

		if (LevelLoader.Instance.CurrentLevelNumber == Constants.StartingLevelNumber)
		{
			MainMenu.Instance.DisplayNoSaveFileFoundMessage();
			return;
		}

		CurrentGameState = GameState.DeletingSaveFile;
		EventSystemManager.Instance.DeactivateModule();
		LevelLoader.Instance.RefreshMenuLevelAfterFileDeletion();
		MainMenu.Instance.DisplaySaveFileDeletedMessage();
	}

	public void DeleteSaveFile()
	{
		SaveSystem.DeleteSaveFile();
	}

	private void DisplayPresentationScreen()
	{
		CurrentGameState = GameState.PresentationScreen;
		PresentationScreen.Instance.SwitchToNextState();
		TransitionManager.Instance.SetTransitionCounter(TransitionManager.Instance.presentationScreenFadeTime);
	}

	public void LoadMenu()
	{
		EventSystemManager.Instance.DeactivateModule();
		LevelLoader.Instance.LoadMenu(GetSavedLevelNumber(), isTestMode);
	}

	public void QuitToMenu()
	{
		Time.timeScale = 1f;
		CurrentGameState = GameState.Menu;
		PlayerController.Instance.ResetCharacterForQuitToMenu();
		InputManager.Instance.DisableGameplayMap();
		EventSystemManager.Instance.DeactivateModule();
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventMusicBackToMenu);
		LevelLoader.Instance.QuitToMenu();
	}

	public void EndAndReturnToMenu()
	{
		CurrentGameState = GameState.Menu;
		EndingGame = true;
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeOutCreditsAmbiance);
		SaveSystem.DeleteSaveFile();
		QuitToMenu();
	}

	public void StartGame()
	{
		CurrentGameState = GameState.StartingGame;
		InstantiatePlayer();
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventMusicPlayGame);
		LevelLoader.Instance.FadeOutMenu();
	}

	public void LoadNextLevel()
	{
		PlayerController.Instance.ResetCharacterForLevelTransition(door.gameObject.transform.position.x);
		LevelLoader.Instance.LoadNextLevel();
	}

	public void ReloadLevel()
	{
		Time.timeScale = 1f;
		CurrentGameState = GameState.Reloading;
		PlayerController.Instance.ResetCharacterForReload();

		if (player.scene != gameObject.scene || player.transform.parent != null)
		{
			player.transform.SetParent(null);
			DontDestroyOnLoad(player);
		}
		
		LevelLoader.Instance.ReloadLevel();
	}

	public void ResetCharacterAfterReload()
	{
		PlayerController.Instance.ResetCharacterAfterReload();
	}

	public void ResetCharacterState()
	{
		PlayerController.Instance.ResetState();
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
		CurrentGameState = GameState.Paused;
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1f;

		if (CurrentGameState == GameState.Paused)
			CurrentGameState = GameState.Playing;
	}

	// When quitting the game, first the currently loaded scene is unloaded, then some mutually dependent game objects/scripts need to be destroyed, and finally we quit the application
	public void UnloadToQuit()
	{
		Time.timeScale = 1f;
		CurrentGameState = GameState.Quitting;
		LevelLoader.Instance.StartQuitTransition();
	}

	// The objects/scripts that need to be destroyed use references to other objects/scripts, or are subscribed to events from other objects/scripts. Here is the order of destruction:
	// 1. PlayerController
	// 2. UIManager
	// 3. InputManager
	// 4. MainMenu
	// 5. CinemachineManager
	// 6. (Unsubscribe GameManager from LevelLoader's events)
	// 7. LevelLoader
	// 8. (Quit game)
	private void DestroyToQuit()
	{
		DestroyPlayer();
		Destroy(UIManager.Instance.gameObject);
		Destroy(InputManager.Instance.gameObject);

		if (MainMenu.IsInitialized)
			Destroy(MainMenu.Instance.gameObject);

		if (CinemachineManager.IsInitialized)
			Destroy(CinemachineManager.Instance.gameObject);

		LevelLoader.Instance.LastSceneUnloaded -= DestroyToQuit;

		Destroy(LevelLoader.Instance.gameObject);

		QuitGame();
	}

	public void QuitGame()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

	#endregion

	#region Instantiate/Destroy methods

	private void InstantiatePlayer()
	{
		player = Spawner.Instance.SpawnPlayer();
		PlayerSpawned?.Invoke();
	}

	public void DestroyPlayer()
	{
		if (player != null)
		{
			Destroy(player);
			player = null;
		}
	}

	#endregion

	#region State setting

	public void SetLevel(GameObject pStartDoor, GameObject pWindow)
	{
		levelLights = FindObjectsOfType<Light>();
		door = FindObjectOfType<Door>();
		Spawner.Instance.SetStartDoorPosition(pStartDoor.transform.position);
		TransitionManager.Instance.SetWindow(pWindow);
		TransitionManager.Instance.SetWindowAnimator();

		CheckIfAllLightsOff();
		PlayerController.Instance.SetCharacterAnimationToEnterLevel();
		SetPlayerPosition();
	}

	public bool CheckIfAllLightsOff()
	{
		int lightsRemaining = 0;
		
		foreach (Light light in levelLights)
		{
			if (light.IsOnAndConnected())
			{
				if (door != null)
					door.CloseDoor();

				lightsRemaining++;
			}
		}

		if (door != null && lightsRemaining == 0)
			door.OpenDoor();

		UIManager.Instance.UpdateLightNumberText(lightsRemaining);

		return lightsRemaining == 0;
	}

	private void SetPlayerPosition()
	{
		if (player != null)
			player.transform.position = Spawner.Instance.GetPositionForPlayerSpawn();
	}

	#endregion
}

public enum GameState
{
	PresentationScreen,
	Menu,
	StartingGame,
	Playing,
	Paused,
	Reloading,
	Cutscene,
	Quitting,
	DeletingSaveFile
}