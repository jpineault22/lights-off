using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput playerInput;
    private InputActionMap gameplayMap;
    private InputActionMap UIMap;                                           // Currently used only for the pause menu
    private InputActionMap creditsEndMap;
    public string CurrentControlScheme { get; private set; }

    public Action<string> ControlSchemeChanged;
    public Action<InputAction.CallbackContext> MovedHorizontally;
    public Action<InputAction.CallbackContext> MovedVertically;
    public Action<InputAction.CallbackContext> StartedJump;
    public Action<InputAction.CallbackContext> EndedJump;
    public Action<InputAction.CallbackContext> StartedInteraction;
    public Action<InputAction.CallbackContext> Paused;
    public Action<InputAction.CallbackContext> Backed;

	protected override void Awake()
	{
		base.Awake();
        DontDestroyOnLoad(gameObject);

		playerInput = GetComponent<PlayerInput>();
        gameplayMap = playerInput.actions.FindActionMap(Constants.InputActionMapGameplay);
        UIMap = playerInput.actions.FindActionMap(Constants.InputActionMapUI);
        creditsEndMap = playerInput.actions.FindActionMap(Constants.InputActionMapCreditsEnd);
        CurrentControlScheme = playerInput.currentControlScheme;

        DisablePlayerInput();
	}

	private void Start()
	{
        ControlSchemeChanged?.Invoke(playerInput.currentControlScheme);
    }

	private void OnEnable()
    {
        gameplayMap.Enable();
        UIMap.Enable();
        creditsEndMap.Disable();
        InputUser.onChange += ChangeControlScheme;
    }

    private void OnDisable()
    {
        InputUser.onChange -= ChangeControlScheme;
        gameplayMap.Disable();
        UIMap.Disable();
        creditsEndMap.Disable();
    }

    private void ChangeControlScheme(InputUser pUser, InputUserChange pChange, InputDevice pDevice)
    {
        if (pChange == InputUserChange.ControlSchemeChanged)
		{
            CurrentControlScheme = playerInput.currentControlScheme;
            ControlSchemeChanged?.Invoke(playerInput.currentControlScheme);
		}
    }

    // These methods are assigned in Unity, in the Events section of the InputManager's PlayerInput component
    // Gameplay Control Scheme

    public void MoveHorizontally(InputAction.CallbackContext ctx)
    {
        MovedHorizontally?.Invoke(ctx);
    }

    public void MoveVertically(InputAction.CallbackContext ctx)
    {
        MovedVertically?.Invoke(ctx);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed) StartedJump?.Invoke(ctx);
        else if (ctx.phase == InputActionPhase.Canceled) EndedJump?.Invoke(ctx);
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed) StartedInteraction?.Invoke(ctx);
    }

    // UI Control Scheme

    public void Pause(InputAction.CallbackContext ctx)
    {
        Paused?.Invoke(ctx);
    }

    public void Back(InputAction.CallbackContext ctx)
    {
        Backed?.Invoke(ctx);
    }

    // Credits End Control Scheme

    public void Exit(InputAction.CallbackContext ctx)
	{
        if (ctx.phase == InputActionPhase.Performed)
		{
            DisablePlayerInputCreditsEnd();
            GameManager.Instance.EndAndReturnToMenu();
        }
	}

    // Enabling/Disabling Input

    public void EnablePlayerInput()
	{
        playerInput.enabled = true;
	}

    public void DisablePlayerInput()
	{
        playerInput.enabled = false;
    }

    public void EnablePlayerInputCreditsEnd()
	{
        gameplayMap.Disable();
        UIMap.Disable();
        creditsEndMap.Enable();
        playerInput.enabled = true;
	}

    public void DisablePlayerInputCreditsEnd()
	{
        creditsEndMap.Disable();
        gameplayMap.Enable();
        UIMap.Enable();
	}
}