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
    public event Action LevelTransitionEnded;

    private AsyncOperation currentLevelLoadOperation;
    private string previousSceneName = string.Empty;
    private bool crossfadeStartEnded;
    private bool reloadingLevel;
    private string levelNamePrefix;

	protected override void Awake()
	{
        base.Awake();

        CurrentLevelNumber = 0;
	}

	private void Start()
	{
        DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
        if (crossfadeStartEnded && currentLevelLoadOperation.isDone)
        {
            crossfadeStartEnded = false;
            TransitionHalfDone?.Invoke();
        }
    }

    public void LoadLevelFromMenu(int pTargetLevelNumber, bool pTestMode, string pCurrentSceneName)
    {
        levelNamePrefix = pTestMode ? Constants.NamePrefixSceneTest : Constants.NamePrefixSceneLevel;
        previousSceneName = pCurrentSceneName;
        LoadLevel(pTargetLevelNumber);
    }

    public void LoadNextLevel()
	{
        LoadLevel(CurrentLevelNumber + 1);
	}

    public void LoadLevel(int pTargetLevelNumber)
    {
        if (!previousSceneName.Equals(Constants.NameSceneStartMenu))
		{
            previousSceneName = levelNamePrefix + CurrentLevelNumber;
		}

        CurrentLevelNumber = pTargetLevelNumber;
        GameManager.Instance.SaveGame();
        currentLevelLoadOperation = LoadScene(levelNamePrefix + pTargetLevelNumber);

        try
		{
            currentLevelLoadOperation.allowSceneActivation = false;
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

    public void ReloadLevel()
	{
        reloadingLevel = true;
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

        if (GameManager.Instance.CurrentGameState == GameState.Playing || GameManager.Instance.CurrentGameState == GameState.Reloading)
        {
            Scene currentLevel = SceneManager.GetSceneByName(levelNamePrefix + CurrentLevelNumber);
            GameObject[] gameObjects = currentLevel.GetRootGameObjects();

            CurrentFunctionalLevel = FindFunctionalLevel(gameObjects);
            GameManager.Instance.SetLevel(Spawner.Instance.FindStartDoor(CurrentFunctionalLevel));

            if (reloadingLevel)
			{
                StartCoroutine(CrossfadeEndTransition());
                reloadingLevel = false;
			}
        }
    }

    private void OnUnloadOperationComplete(AsyncOperation pAsyncOperation)
    {
        Debug.Log("Unload Complete.");

        if (reloadingLevel)
		{
            // When the current level has finished unloading, load same level and end Crossfade animation when this is done
            currentLevelLoadOperation = LoadScene(levelNamePrefix + CurrentLevelNumber);
        }
    }

    IEnumerator CrossfadeStartTransition()
    {
        crossfadeAnimator.SetTrigger(Constants.AnimatorCrossfadeStart);

        yield return new WaitForSeconds(fadeTime);

        crossfadeStartEnded = true;

        if (reloadingLevel)
		{
            GameManager.Instance.ResetCharacterAfterDeath();
            UnloadScene(levelNamePrefix + CurrentLevelNumber);
        }
        else
		{
            currentLevelLoadOperation.allowSceneActivation = true;

            if (previousSceneName != string.Empty)
            {
                UnloadScene(previousSceneName);
                previousSceneName = string.Empty;
            }

            StartCoroutine(CrossfadeEndTransition());
        }
    }

    IEnumerator CrossfadeEndTransition()
    {
        crossfadeAnimator.SetTrigger(Constants.AnimatorCrossfadeEnd);

        yield return new WaitForSeconds(fadeTime);

        LevelTransitionEnded?.Invoke();
        GameManager.Instance.SetGameState(GameState.Playing);
        GameManager.Instance.ResetCharacterState();
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
