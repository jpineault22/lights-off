using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [SerializeField] private CanvasGroup crossfadeCanvasGroup = default;
    [SerializeField] private Canvas crossfadeCanvas = default;
    [SerializeField] private float endingCinematicWaitTime = 10f;
    [SerializeField] private int crossfadeCanvasDefaultSortOrder = 3;
    [SerializeField] private int crossfadeCanvasFileDeletionSortOrder = 1;
    [SerializeField] private float defaultBackgroundBlurFocalLength = 300f;

    private float currentTransitionTime;
    public float defaultFadeTime = 1;                            // Time for fade out/in, total transition animation time is twice that amount
    public float fileDeletionFadeTime = 0.5f;
    public float menuFadeOutTime = 0.5f;
    public float presentationScreenFadeTime = 2f;
    public float presentationScreenWaitTime = 3f;

    public event Action CrossfadeTransitionEnded;

    private GameObject window;
    private Animator windowAnimator;
    private Bed bed;

    public CrossfadeState CrossfadeState { get; private set; }
    private float transitionCounter;
    private bool crossfadeStartEnded;
    private bool readyForCredits;

    protected override void Awake()
	{
		base.Awake();

        CrossfadeState = CrossfadeState.NotFading;
        currentTransitionTime = defaultFadeTime;
    }

	private void Update()
	{
        ProcessPresentationScreen();
        ProcessCrossfade();
        ProcessMenuTransition();
        ProcessEndCinematic();
    }

    // Transition processing methods

    public void TriggerCrossfadeStart()
    {
        TriggerCrossfadeStart(defaultFadeTime);
    }

    public void TriggerCrossfadeEnd()
    {
        TriggerCrossfadeEnd(defaultFadeTime);
    }

    public void TriggerCrossfadeStart(float pFadeTime)
	{
        CrossfadeState = CrossfadeState.Start;
        SetCrossfadeInteractableAndBlocksRaycasts(true);
        transitionCounter = currentTransitionTime = pFadeTime;
    }

    public void TriggerCrossfadeEnd(float pFadeTime)
	{
        CrossfadeState = CrossfadeState.End;
        transitionCounter = currentTransitionTime = pFadeTime;
    }

    public void TriggerCrossfadeStartFileDeletion()
	{
        crossfadeCanvas.sortingOrder = crossfadeCanvasFileDeletionSortOrder;
        TriggerCrossfadeStart(fileDeletionFadeTime);
	}

    public void TriggerCrossfadeEndFileDeletion()
	{
        TriggerCrossfadeEnd(fileDeletionFadeTime);
	}

    public IEnumerator TriggerEndingCinematic(Bed pBed)
    {
        GameManager.Instance.CurrentGameState = GameState.Cutscene;
        GameManager.Instance.DestroyPlayer();
        bed = pBed;

        yield return new WaitForSeconds(endingCinematicWaitTime);

        if (window == null)
        {
            Debug.LogError("[TransitionManager] Game object 'Window' not found. Quitting game...");
            GameManager.Instance.QuitGame();
        }

        readyForCredits = true;
        windowAnimator.enabled = true;
    }

    private void ProcessCrossfade()
	{
        if (CrossfadeState != CrossfadeState.NotFading && GameManager.Instance.CurrentGameState != GameState.PresentationScreen)
        {
            if (transitionCounter <= 0 && (crossfadeCanvasGroup.alpha <= 0 || crossfadeCanvasGroup.alpha >= 1))
            {
                if (CrossfadeState == CrossfadeState.Start)
                {
                    if (GameManager.Instance.CurrentGameState == GameState.Reloading)
                    {
                        Spawner.Instance.DestroyObjectsForReload();
                        GameManager.Instance.ResetCharacterAfterReload();
                        LevelLoader.Instance.UnloadCurrentLevel();
                    }
                    else if (GameManager.Instance.CurrentGameState == GameState.Quitting)
                    {
                        foreach (Scene scene in GameObjectUtils.GetLoadedScenesNoBoot())
                        {
                            LevelLoader.Instance.UnloadScene(scene.name);
                        }
                    }
                    else if (GameManager.Instance.CurrentGameState == GameState.Menu)
                    {
                        crossfadeStartEnded = true;
                        UIManager.Instance.UpdateKeyNumberText(0);
                        UIManager.Instance.EnableInGameUI(false);
                        LevelLoader.Instance.LoadMenuAndAddOperation();
                    }
                    else if (GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile)
					{
                        LevelLoader.Instance.UnloadCurrentLevel();
					}
                    else
                    {
                        LevelLoader.Instance.ActivateAllLoadedScenes();
                    }
                }
                else
                {
                    CrossfadeTransitionEnded?.Invoke();

                    if (GameManager.Instance.CurrentGameState == GameState.Menu)
                    {
                        LevelLoader.Instance.CurrentMenuLevelState = MenuLevelState.None;
                        EventSystemManager.Instance.ActivateModule();
                    }
                    else if (GameManager.Instance.CurrentGameState == GameState.Reloading)
                    {
                        GameManager.Instance.CurrentGameState = GameState.Playing;
                    }
                    else if (GameManager.Instance.CurrentGameState == GameState.DeletingSaveFile)
					{
                        GameManager.Instance.CurrentGameState = GameState.Menu;
                        crossfadeCanvas.sortingOrder = crossfadeCanvasDefaultSortOrder;
                        EventSystemManager.Instance.ActivateModule();
                    }

                    if (GameManager.Instance.CurrentGameState == GameState.Playing)
                        GameManager.Instance.ResetCharacterState();

                    SetCrossfadeInteractableAndBlocksRaycasts(false);
                }

                CrossfadeState = CrossfadeState.NotFading;
            }

            // Process animation manually
            float alphaDifference = Time.deltaTime / currentTransitionTime;
            int fadeDirection = CrossfadeState == CrossfadeState.Start ? 1 : -1;
            crossfadeCanvasGroup.alpha += alphaDifference * fadeDirection;

            transitionCounter -= Time.deltaTime;
        }
    }

    private void ProcessMenuTransition()
	{
        if (GameManager.Instance.CurrentGameState == GameState.LoadingGame)
        {
            if (transitionCounter <= 0)
            {
                if (LevelLoader.Instance.CurrentMenuLevelState == MenuLevelState.FadingOutMenu)
                {
                    UIManager.Instance.EnableInGameUI(true);
                    LevelLoader.Instance.UnloadMenu();
                }
                else if (LevelLoader.Instance.CurrentMenuLevelState == MenuLevelState.StartGameMenuUnloaded)
                {
                    LevelLoader.Instance.CurrentMenuLevelState = MenuLevelState.None;
                    InputManager.Instance.EnablePlayerInput();
                    GameManager.Instance.CurrentGameState = GameState.Playing;
                    GameManager.Instance.ResetCharacterState();
                }
            }

            if (LevelLoader.Instance.CurrentMenuLevelState == MenuLevelState.FadingOutMenu)
            {
                float differenceProportion = Time.deltaTime / currentTransitionTime;
                MainMenu.Instance.AddToCanvasGroupAlpha(-differenceProportion);
                MainMenu.Instance.AddToDepthOfFieldFocalLength(-differenceProportion * defaultBackgroundBlurFocalLength);
                CinemachineManagerV2.Instance.ZoomIn(differenceProportion);
            }

            transitionCounter -= Time.deltaTime;
        }
        else if (GameManager.Instance.CurrentGameState == GameState.Menu && crossfadeStartEnded && LevelLoader.Instance.CheckIfAllOperationsDone())
        {
            crossfadeStartEnded = false;
            LevelLoader.Instance.CurrentMenuLevelState = MenuLevelState.UnloadLevel;
            LevelLoader.Instance.UnloadCurrentLevel();
        }
    }

    private void ProcessPresentationScreen()
	{
        if (GameManager.Instance.CurrentGameState == GameState.PresentationScreen)
		{
            if (transitionCounter <= 0)
			{
                PresentationScreen.Instance.SwitchToNextState();

                if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.TextFadeIn)
                {
                    SetTransitionCounter(presentationScreenFadeTime);
                    GameManager.Instance.LoadMenu();
                }
                else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.TextWait) SetTransitionCounter(presentationScreenWaitTime);
                else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.TextFadeOut) SetTransitionCounter(presentationScreenFadeTime);
                else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.FinalBlack) SetTransitionCounter(presentationScreenFadeTime);
                else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.ScreenFadeOut)
                {
                    SetTransitionCounter(presentationScreenFadeTime);
                    AudioManager.Instance.PlayMusic();
                }
                else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.ScreenGone)
                {
                    GameManager.Instance.CurrentGameState = GameState.Menu;
                    EventSystemManager.Instance.ActivateModule();
                    PresentationScreen.Instance.SetPanelInteractableAndBlocksRaycasts(false);
                    CrossfadeTransitionEnded?.Invoke();
                }
            }
            
            if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.TextFadeIn)
                PresentationScreen.Instance.AddToTextAlpha(Time.deltaTime / currentTransitionTime);
            else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.TextFadeOut)
                PresentationScreen.Instance.AddToTextAlpha(-Time.deltaTime / currentTransitionTime);
            else if (PresentationScreen.Instance.CurrentPresentationScreenState == PresentationScreenState.ScreenFadeOut)
                PresentationScreen.Instance.AddToPanelAlpha(-Time.deltaTime / currentTransitionTime);

            transitionCounter -= Time.deltaTime;
		}
	}

    private void ProcessEndCinematic()
	{
        if (windowAnimator != null && windowAnimator.enabled && windowAnimator.GetCurrentAnimatorStateInfo(0).IsName(Constants.AnimationWindowSunrise))
        {
            if (readyForCredits && windowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                readyForCredits = false;
                StartCoroutine(CreditsManager.Instance.StartCredits());
            }
            else if (windowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 8.5f / 13)
            {
                bed.WakeUp();
            }
        }
    }

    // State setting methods

    private void SetCrossfadeInteractableAndBlocksRaycasts(bool pInteractableAndBlocksRaycasts)
    {
        crossfadeCanvasGroup.interactable = crossfadeCanvasGroup.blocksRaycasts = pInteractableAndBlocksRaycasts;
    }

    public void SetCrossfadeCanvasAlpha(float pAlpha)
	{
        crossfadeCanvasGroup.alpha = pAlpha;
	}

    public void SetTransitionCounter(float pTransitionTime)
	{
        transitionCounter = currentTransitionTime = pTransitionTime;
	}

    public void SetWindow(GameObject pWindow)
	{
        window = pWindow;
	}

    public void SetWindowAnimator()
	{
        if (window != null)
            windowAnimator = window.GetComponent<Animator>();
	}
}

public enum CrossfadeState
{
    NotFading,
    Start,
    End
}
