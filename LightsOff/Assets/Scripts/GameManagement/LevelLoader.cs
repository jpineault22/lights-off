using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    [SerializeField] private Animator crossfadeAnimator = default;
    [SerializeField] private float fadeTime = 1;                          // Time for fade out/in, total transition animation time is twice that amount

    public int CurrentLevelNumber { get; private set; }
    public GameObject CurrentFunctionalLevel { get; private set; }

    public event Action TransitionHalfDone;                                 // This event is invoked if the CrossfadeStart animation has ended AND the next level has been loaded
    public event Action CrossfadeTransitionEnded;
    public event Action MenuReloaded;
    public event Action LastSceneUnloaded;

    private AsyncOperation currentSceneLoadOperation;
    private string previousSceneName = string.Empty;
    private bool firstMenuLoad;
    private bool crossfadeStartEnded;
    private string levelNamePrefix;

	protected override void Awake()
	{
        base.Awake();

        DontDestroyOnLoad(gameObject);

        firstMenuLoad = true;
        CurrentLevelNumber = 0;
	}

	private void Update()
	{
        if (crossfadeStartEnded && currentSceneLoadOperation.isDone)
        {
            crossfadeStartEnded = false;
            TransitionHalfDone?.Invoke();

            if (GameManager.Instance.CurrentGameState == GameState.Menu)
            {
                PauseMenu.Instance.Resume();
                UnloadLevelToMenu();
            }
            else if (previousSceneName != string.Empty)
            {
                UnloadScene(previousSceneName);
                previousSceneName = string.Empty;
            }
        }
    }

    public void LoadLevelFromMenu(int pTargetLevelNumber, bool pTestMode)
    {
        levelNamePrefix = pTestMode ? Constants.NamePrefixSceneTest : Constants.NamePrefixSceneLevel;
        previousSceneName = Constants.NameSceneStartMenu;
        LoadLevel(pTargetLevelNumber);
    }

    public void LoadNextLevel()
	{
        int nextLevelNumber = CurrentLevelNumber + 1;
        GameManager.Instance.SaveGame(nextLevelNumber);
        LoadLevel(nextLevelNumber);
	}

    public void LoadLevel(int pTargetLevelNumber)
    {
        if (!previousSceneName.Equals(Constants.NameSceneStartMenu))
		{
            previousSceneName = levelNamePrefix + CurrentLevelNumber;
		}

        CurrentLevelNumber = pTargetLevelNumber;
        currentSceneLoadOperation = LoadScene(levelNamePrefix + pTargetLevelNumber);

        try
		{
            currentSceneLoadOperation.allowSceneActivation = false;
		}
        catch (NullReferenceException e)
		{
            Debug.LogException(e);
            Debug.LogWarning("[LevelLoader] Unable to find next level. Quitting game...");
            GameManager.Instance.QuitGame();
		}

        // Start Crossfade animation
        StartCoroutine(CrossfadeStartTransition());
    }

    public void ReloadLevel()
	{
        StartCoroutine(CrossfadeStartTransition());
	}

    public void QuitToMenu()
	{
        currentSceneLoadOperation = LoadScene(Constants.NameSceneStartMenu);
        StartCoroutine(CrossfadeStartTransition());

        try
		{
            currentSceneLoadOperation.allowSceneActivation = false;
        }
        catch (NullReferenceException e)
		{
            Debug.LogException(e);
            Debug.LogWarning("[LevelLoader] Unable to load menu. Quitting game...");
            GameManager.Instance.QuitGame();
        }
    }

    public void UnloadLevelToMenu()
    {
        if (CurrentLevelNumber != 0)
        {
            UnloadScene(levelNamePrefix + CurrentLevelNumber);
            CurrentLevelNumber = 0;
        }
        else
        {
            Debug.LogError("[LevelLoader] Current level not set, cannot unload scene.");
        }
    }

    public void StartQuitTransition()
	{
        StartCoroutine(CrossfadeStartTransition());
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

        if (GameManager.Instance.CurrentGameState == GameState.Reloading)
        {
            SetLevel();
            StartCoroutine(CrossfadeEndTransition());
        }
        else if (firstMenuLoad)
		{
            firstMenuLoad = false;
            StartCoroutine(CrossfadeEndTransition());
        }
    }

    private void OnUnloadOperationComplete(AsyncOperation pAsyncOperation)
    {
        Debug.Log("Unload Complete.");

        if (GameManager.Instance.CurrentGameState == GameState.Reloading)
		{
            // When the current level has finished unloading, reset pause menu, load same level and end Crossfade animation when this is done
            PauseMenu.Instance.Resume();
            currentSceneLoadOperation = LoadScene(levelNamePrefix + CurrentLevelNumber);
            return;
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Menu)
		{
            MenuReloaded?.Invoke();
            GameManager.Instance.DestroyPlayer();
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Quitting)
		{
            LastSceneUnloaded?.Invoke();
            return;
		}
        else
		{
            SetLevel();
		}

        StartCoroutine(CrossfadeEndTransition());
    }

    IEnumerator CrossfadeStartTransition()
    {
        crossfadeAnimator.SetTrigger(Constants.AnimatorCrossfadeStart);

        yield return new WaitForSeconds(fadeTime);

        crossfadeStartEnded = true;

        if (GameManager.Instance.CurrentGameState == GameState.Reloading)
		{
            Spawner.Instance.DestroyObjectsForReload();
            GameManager.Instance.ResetCharacterAfterReload();
            UnloadScene(levelNamePrefix + CurrentLevelNumber);
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Quitting)
		{
            string sceneName = CurrentLevelNumber == 0 ? Constants.NameSceneStartMenu : levelNamePrefix + CurrentLevelNumber;
            UnloadScene(sceneName);
        }
        else
		{
            // When the scene has been loaded, this line allows the AsyncOperation to be completed and OnLoadOperationComplete to be called
            currentSceneLoadOperation.allowSceneActivation = true;
        }
    }

    IEnumerator CrossfadeEndTransition()
    {
        crossfadeAnimator.SetTrigger(Constants.AnimatorCrossfadeEnd);

        yield return new WaitForSeconds(fadeTime);

        CrossfadeTransitionEnded?.Invoke();

        if (GameManager.Instance.CurrentGameState == GameState.Menu)
		{
            EventSystemManager.Instance.ActivateModule();
		}
        else if (GameManager.Instance.CurrentGameState == GameState.LoadingGame)
		{
            InputManager.Instance.EnablePlayerInput();
            GameManager.Instance.SetGameState(GameState.Playing);
        }

        if (GameManager.Instance.CurrentGameState == GameState.Reloading) GameManager.Instance.SetGameState(GameState.Playing);
        if (GameManager.Instance.CurrentGameState == GameState.Playing) GameManager.Instance.ResetCharacterState();
    }

    private void SetLevel()
	{
        Scene currentLevel = SceneManager.GetSceneByName(levelNamePrefix + CurrentLevelNumber);
        GameObject[] gameObjects = currentLevel.GetRootGameObjects();

        CurrentFunctionalLevel = FindFunctionalLevel(gameObjects);
        GameManager.Instance.SetLevel(Spawner.Instance.FindStartDoor(CurrentFunctionalLevel));
    }

    private GameObject FindFunctionalLevel(GameObject[] pGameObjects)
    {
        foreach (GameObject obj in pGameObjects)
        {
            if (obj.CompareTag(Constants.TagFunctionalLevel))
            {
                return obj;
            }
        }

        return null;
    }
}
