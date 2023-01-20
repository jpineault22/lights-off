using UnityEngine;
using TMPro;

public class PresentationScreen : Singleton<PresentationScreen>
{
    [SerializeField] private CanvasGroup presentationScreenCanvasGroup = default;
    [SerializeField] private TMP_Text presentationScreenText = default;

    public PresentationScreenState CurrentPresentationScreenState { get; private set; }

	protected override void Awake()
	{
		base.Awake();

        CurrentPresentationScreenState = PresentationScreenState.Booting;
	}

	public void SwitchToNextState()
	{
        if (CurrentPresentationScreenState == PresentationScreenState.Booting) CurrentPresentationScreenState = PresentationScreenState.InitialBlack;
        else if (CurrentPresentationScreenState == PresentationScreenState.InitialBlack) CurrentPresentationScreenState = PresentationScreenState.TextFadeIn;
        else if (CurrentPresentationScreenState == PresentationScreenState.TextFadeIn) CurrentPresentationScreenState = PresentationScreenState.TextWait;
        else if (CurrentPresentationScreenState == PresentationScreenState.TextWait) CurrentPresentationScreenState = PresentationScreenState.TextFadeOut;
        else if (CurrentPresentationScreenState == PresentationScreenState.TextFadeOut) CurrentPresentationScreenState = PresentationScreenState.FinalBlack;
        else if (CurrentPresentationScreenState == PresentationScreenState.FinalBlack) CurrentPresentationScreenState = PresentationScreenState.ScreenFadeOut;
        else if (CurrentPresentationScreenState == PresentationScreenState.ScreenFadeOut) CurrentPresentationScreenState = PresentationScreenState.ScreenGone;
	}

    public void AddToTextAlpha(float pDifference)
	{
        presentationScreenText.alpha += pDifference;
    }

    public void AddToPanelAlpha(float pDifference)
	{
        presentationScreenCanvasGroup.alpha += pDifference;
	}

    public void SetPanelInteractableAndBlocksRaycasts(bool pInteractableAndBlocksRaycasts)
	{
        presentationScreenCanvasGroup.interactable = presentationScreenCanvasGroup.blocksRaycasts = pInteractableAndBlocksRaycasts;
	}
}

public enum PresentationScreenState
{
    Booting,
    InitialBlack,
    TextFadeIn,
    TextWait,
    TextFadeOut,
    FinalBlack,
    ScreenFadeOut,
    ScreenGone
}