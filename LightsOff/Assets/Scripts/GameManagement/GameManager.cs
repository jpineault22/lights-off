using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private GameObject[] systemPrefabs = default;          // Prefabs to instantiate when launching the game
	[SerializeField] private bool testMode = default;
	[SerializeField] private float playerDoorHeightDifference = 0.56f;		// The player's position at the start of a level will be lower than the start door's position by this amount

	public GameState CurrentGameState { get; private set; }
	[HideInInspector] public GameObject player;

	private List<GameObject> instantiatedSystemPrefabs;

	private Light[] levelLights;
	private Door door;
	private Vector2 startDoorPosition;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		instantiatedSystemPrefabs = new List<GameObject>();
		InstantiateSystemPrefabs();

		LoadMenu();
	}

	protected override void OnDestroy()
	{
		for (int i = 0; i < instantiatedSystemPrefabs.Count; i++)
		{
			Destroy(instantiatedSystemPrefabs[i]);
		}

		instantiatedSystemPrefabs.Clear();

		base.OnDestroy();
	}

	private void OnEnable()
	{
		LevelLoader.Instance.TransitionHalfDone += SetPlayerPosition;
		LevelLoader.Instance.LastSceneUnloaded += DestroyToQuit;
	}

	#region Game saving and loading, pause

	public void SaveGame(int pCurrentLevelNumber)
	{
		if (testMode)
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

	public void LoadGame()
	{
		PlayerData data = SaveSystem.LoadPlayerData();
		int currentLevelNumber = Constants.StartingLevelNumber;

		if (data != null && !testMode)
		{
			currentLevelNumber = data.CurrentLevelNumber;
		}

		StartGame(currentLevelNumber);
	}

	public void DeleteSaveFile()
	{
		SaveSystem.DeleteSaveFile();
	}

	public void StartGame(int pCurrentLevelNumber)
	{
		InputManager.Instance.DisablePlayerInput();
		CurrentGameState = GameState.LoadingGame;

		InstantiatePlayer();
		LevelLoader.Instance.LoadLevelFromMenu(pCurrentLevelNumber, testMode, Constants.NameSceneStartMenu);
	}

	public void LoadNextLevel()
	{
		PlayerController.Instance.ResetCharacterForLevelTransition();
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

	private void LoadMenu()
	{
		CurrentGameState = GameState.Menu;
		EventSystemManager.Instance.DeactivateModule();
		LevelLoader.Instance.LoadScene(Constants.NameSceneStartMenu);
	}

	public void QuitToMenu()
	{
		Time.timeScale = 1f;
		CurrentGameState = GameState.Menu;
		EventSystemManager.Instance.DeactivateModule();
		LevelLoader.Instance.QuitToMenu();
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

		LevelLoader.Instance.TransitionHalfDone -= SetPlayerPosition;
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

	private void SetPlayerPosition()
	{
		if (player != null)
			player.transform.position = new Vector2(startDoorPosition.x, startDoorPosition.y - playerDoorHeightDifference);
	}

	#endregion

	#region Instantiate/Destroy methods

	private void InstantiateSystemPrefabs()
	{
		for (int i = 0; i < systemPrefabs.Length; i++)
		{
			GameObject prefabInstance = Instantiate(systemPrefabs[i]);
			instantiatedSystemPrefabs.Add(prefabInstance);
		}
	}

	private void InstantiatePlayer()
	{
		player = Spawner.Instance.InstantiatePlayer();
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

	public void SetGameState(GameState pGameState)
	{
		CurrentGameState = pGameState;
	}
	
	public void SetLevel(GameObject pStartDoor)
	{
		levelLights = FindObjectsOfType<Light>();
		door = FindObjectOfType<Door>();
		startDoorPosition = pStartDoor.transform.position;

		CheckIfAllLightsOff();
	}

	public void CheckIfAllLightsOff()
	{
		if (PlayerController.Instance.CurrentCharacterState == CharacterState.LevelTransition) return;
		
		foreach (Light light in levelLights)
		{
			if (light.IsOnAndConnected())
			{
				door.CloseDoor();
				return;
			}
		}

		door.OpenDoor();
	}

	#endregion
}

public enum GameState
{
	Menu,
	LoadingGame,
	Playing,
	Paused,
	Reloading,
	Cutscene,		// Not implemented
	Quitting
}