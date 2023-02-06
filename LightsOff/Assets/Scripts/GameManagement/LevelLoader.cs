using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    public int CurrentLevelNumber { get; private set; }
    public GameObject CurrentFunctionalLevel { get; private set; }
    public MenuLevelState CurrentMenuLevelState { get; set; }

    public event Action MenuReloaded;
    public event Action LastSceneUnloaded;

    private List<AsyncOperation> currentSceneLoadOperations;
    private string previousSceneName = string.Empty;
    private string levelNamePrefix;

	protected override void Awake()
	{
        base.Awake();

        currentSceneLoadOperations = new List<AsyncOperation>();
        CurrentMenuLevelState = MenuLevelState.None;
	}

    public void LoadMenu(int pSavedLevelNumber, bool pIsTestMode)
	{
        levelNamePrefix = pIsTestMode ? Constants.NamePrefixSceneTest : Constants.NamePrefixSceneLevel;
        //TransitionManager.Instance.SetCrossfadeCanvasAlpha(1f);
        LoadMenuAndAddOperation();
        LoadLevel(pSavedLevelNumber);
	}

    public void LoadMenuAndAddOperation()
	{
        currentSceneLoadOperations.Add(LoadScene(Constants.NameSceneStartMenu));
    }

    public void FadeOutMenu()
	{
        CurrentMenuLevelState = MenuLevelState.FadingOutMenu;
        TransitionManager.Instance.SetTransitionCounter(TransitionManager.Instance.menuFadeOutTime);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventSetTransitionSFXVolumeToMax);
    }

    public void UnloadMenu()
	{
        CurrentMenuLevelState = MenuLevelState.None;
        previousSceneName = Constants.NameSceneStartMenu;
        UnloadScene(Constants.NameSceneStartMenu);
    }

    public void UnloadCurrentLevel()
	{
        UnloadScene(levelNamePrefix + CurrentLevelNumber);
    }

    public void LoadNextLevel()
	{
        previousSceneName = levelNamePrefix + CurrentLevelNumber;
        int nextLevelNumber = CurrentLevelNumber + 1;
        GameManager.Instance.SaveGame(nextLevelNumber);
        LoadLevel(nextLevelNumber);
	}

    public void LoadLevel(int pTargetLevelNumber)
    {
        CurrentLevelNumber = pTargetLevelNumber;

        try
		{
            AsyncOperation operation = LoadScene(levelNamePrefix + pTargetLevelNumber);
            operation.allowSceneActivation = GameManager.Instance.CurrentGameState == GameState.PresentationScreen ||
                                            GameManager.Instance.CurrentGameState == GameState.Menu ||
                                            GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile;
            currentSceneLoadOperations.Add(operation);
		}
        catch (NullReferenceException e)
		{
            Debug.LogException(e);
            Debug.LogWarning("[LevelLoader] Unable to load level. Quitting game...");
            GameManager.Instance.QuitGame();
		}

        if (GameManager.Instance.CurrentGameState != GameState.Menu && GameManager.Instance.CurrentGameState != GameState.PresentationScreen)
		{
            TransitionManager.Instance.TriggerCrossfadeStart();
            
            if (GameManager.Instance.CurrentGameState != GameState.DeletingSaveFile)
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeOutForLevelTransition);
        }
    }

    public void ReloadLevel()
	{
        TransitionManager.Instance.TriggerCrossfadeStart();
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeOutForLevelTransition);
    }

    public void QuitToMenu()
	{
        CurrentMenuLevelState = MenuLevelState.LoadMenuAndCrossfadeStart;
        TransitionManager.Instance.TriggerCrossfadeStart();

        if (!GameManager.Instance.EndingGame)
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeOutForQuitTransition);
    }

    public void StartQuitTransition()
	{
        TransitionManager.Instance.TriggerCrossfadeStart();
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeOutForQuitTransition);
    }

    public void RefreshMenuLevelAfterFileDeletion()
	{
        TransitionManager.Instance.TriggerCrossfadeStartFileDeletion();
	}

    public AsyncOperation LoadScene(string pSceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(pSceneName, LoadSceneMode.Additive);

        if (asyncOperation == null)
        {
            Debug.LogError("[LevelLoader] Unable to load scene " + pSceneName);
            return null;
        }

        asyncOperation.completed += OnLoadOperationComplete;

        return asyncOperation;
    }

    public void UnloadScene(string pSceneName)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(pSceneName);

        if (asyncOperation == null)
        {
            Debug.LogError("[LevelLoader] Unable to unload scene " + pSceneName);
            return;
        }

        asyncOperation.completed += OnUnloadOperationComplete;
    }

    private void OnLoadOperationComplete(AsyncOperation pAsyncOperation)
    {
        Debug.Log("Load Complete");
        UIManager.Instance.UpdateLevelNumberText(CurrentLevelNumber);

        Scene currentLevel = SceneManager.GetSceneByName(levelNamePrefix + CurrentLevelNumber);
        Spawner.Instance.SetRootGameObjects(currentLevel.GetRootGameObjects());
        Spawner.Instance.FindFunctionalLevel();
        CinemachineManagerV2.Instance.ChangeCameraPositionAndSize(Spawner.Instance.GetLevelCameraPosition(), Spawner.Instance.GetLevelCameraSize());

        if (GameManager.Instance.CurrentGameState == GameState.PresentationScreen || GameManager.Instance.CurrentGameState == GameState.Menu || GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile)
            CinemachineManagerV2.Instance.ZoomOutInstantly();

        if (GameManager.Instance.CurrentGameState == GameState.PresentationScreen)
		{
            CreditsManager.Instance.DisableLastScreen();
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Menu)
        {
            if (CurrentMenuLevelState == MenuLevelState.LoadMenuAndCrossfadeStart)
			{
                return;
            }
            else if (CurrentMenuLevelState == MenuLevelState.ReloadLevel)
			{
                CurrentMenuLevelState = MenuLevelState.CrossfadeEnd;
                
                MenuReloaded?.Invoke();
                PauseMenu.Instance.Resume();
            }

            CreditsManager.Instance.DisableLastScreen();

            if (TransitionManager.Instance.CrossfadeState == CrossfadeState.NotFading)
			{
                TransitionManager.Instance.TriggerCrossfadeEnd();
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeInToMenu);
            }
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Reloading)
		{
            SetLevel();
            TransitionManager.Instance.TriggerCrossfadeEnd();
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeInForLevelTransition);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile)
		{
            TransitionManager.Instance.TriggerCrossfadeEndFileDeletion();
        }
        else if (previousSceneName != string.Empty)
        {
            currentSceneLoadOperations.Clear();
            UnloadScene(previousSceneName);
            previousSceneName = string.Empty;
        }

        if (GameManager.Instance.CurrentGameState != GameState.Playing && CheckIfAllOperationsDone())
            currentSceneLoadOperations.Clear();
    }

    private void OnUnloadOperationComplete(AsyncOperation pAsyncOperation)
    {
        Debug.Log("Unload Complete.");

        if (GameManager.Instance.CurrentGameState == GameState.Reloading)
		{
            // When the current level has finished unloading, reset pause menu, load same level and end Crossfade animation when this is done
            PauseMenu.Instance.Resume();
            currentSceneLoadOperations.Add(LoadScene(levelNamePrefix + CurrentLevelNumber));
            return;
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Menu)
		{
            if (GameManager.Instance.EndingGame)
			{
                GameManager.Instance.EndingGame = false;
                CurrentLevelNumber = Constants.StartingLevelNumber;
            }
            
            CurrentMenuLevelState = MenuLevelState.ReloadLevel;
            GameManager.Instance.DestroyPlayer();
            LoadLevel(CurrentLevelNumber);
            return;
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Quitting)
		{
            if (GameObjectUtils.GetLoadedScenesNoBoot().Count == 0)
                LastSceneUnloaded?.Invoke();

            return;
		}
        else if (GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile)
		{
            LoadLevel(Constants.StartingLevelNumber);
            return;
        }

        SetLevel();

        if (GameManager.Instance.CurrentGameState == GameState.StartingGame)
		{
            CurrentMenuLevelState = MenuLevelState.StartGameMenuUnloaded;
            TransitionManager.Instance.SetTransitionCounter(TransitionManager.Instance.defaultFadeTime);
            return;
        }

        TransitionManager.Instance.TriggerCrossfadeEnd();
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventFadeInForLevelTransition);
    }

    private void SetLevel()
	{
        Spawner.Instance.SpawnEnemies();
        GameManager.Instance.SetLevel(Spawner.Instance.FindStartDoor(), Spawner.Instance.FindWindow());
    }

    public bool CheckIfAllOperationsDone()
	{
        bool allOperationsDone = true;

        foreach (AsyncOperation operation in currentSceneLoadOperations)
        {
            if (!operation.isDone)
                allOperationsDone = false;
        }

        return allOperationsDone;
    }

    public void ActivateAllLoadedScenes()
	{
        foreach (AsyncOperation operation in currentSceneLoadOperations)
        {
            // When the scene has been loaded, this line allows the AsyncOperation to be completed and OnLoadOperationComplete to be called
            operation.allowSceneActivation = true;
        }
    }
}

// This enum defines the state of the transition between the menu and the levels. It is used for the Quit to Menu transition, as well as the Start Game transition.
public enum MenuLevelState
{
    None,
    LoadMenuAndCrossfadeStart,
    UnloadLevel,
    ReloadLevel,
    CrossfadeEnd,
    FadingOutMenu,
    StartGameMenuUnloaded
}