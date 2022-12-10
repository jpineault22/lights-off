using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[SerializeField] private GameObject[] systemPrefabs = default;          // Prefabs to instantiate when launching the game
	[SerializeField] private bool testMode = default;
	[SerializeField] private float playerDoorHeightDifference = 0.5625f;	// The player's position at the start of a level will be lower than the start door's position by this amount

	public GameState CurrentGameState { get; private set; }
	[HideInInspector] public GameObject player;

	private List<GameObject> instantiatedSystemPrefabs;

	private Light[] levelLights;
	private Door door;
	private Vector2 startDoorPosition;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		LevelLoader.Instance.TransitionHalfDone += SetPlayerPosition;

		instantiatedSystemPrefabs = new List<GameObject>();
		InstantiateSystemPrefabs();

		LoadMenu();
	}

	#region Game saving and loading, pause

	public void SaveGame()
	{
		if (testMode)
		{
			Debug.Log("Cannot save while in test mode.");
		}
		else
		{
			int currentLevelNumber = LevelLoader.Instance.CurrentLevelNumber;
			Debug.Log("Saving in " + Constants.NamePrefixSceneLevel + currentLevelNumber);

			PlayerData data = new PlayerData(currentLevelNumber);

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
		CurrentGameState = GameState.Playing;

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
		CurrentGameState = GameState.Playing;
	}

	private void LoadMenu()
	{
		CurrentGameState = GameState.Menu;
		LevelLoader.Instance.LoadScene(Constants.NameSceneStartMenu);
	}

	public void QuitToMenu()
	{
		CurrentGameState = GameState.Menu;
		LevelLoader.Instance.QuitToMenu();
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
		Spawner.Instance.DestroyPlayer(player);
		player = null;
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

	protected override void OnDestroy()
	{
		for (int i = 0; i < instantiatedSystemPrefabs.Count; i++)
		{
			Destroy(instantiatedSystemPrefabs[i]);
		}

		instantiatedSystemPrefabs.Clear();

		base.OnDestroy();
	}
}

public enum GameState
{
	Menu,
	Playing,
	Paused,
	Reloading,
	Cutscene		// Not implemented
}