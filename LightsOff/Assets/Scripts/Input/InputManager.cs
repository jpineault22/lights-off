using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput playerInput;
    private InputActionMap gameplayMap;
    private InputActionMap UIMap;                                           // Currently used only for the pause menu
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

		playerInput = GetComponent<PlayerInput>();
        gameplayMap = playerInput.actions.FindActionMap(Constants.InputActionMapGameplay);
        UIMap = playerInput.actions.FindActionMap(Constants.InputActionMapUI);
        CurrentControlScheme = playerInput.currentControlScheme;
	}

	private void Start()
	{
        ControlSchemeChanged?.Invoke(playerInput.currentControlScheme);
    }

	private void OnEnable()
    {
        gameplayMap.Enable();
        UIMap.Enable();
        InputUser.onChange += ChangeControlScheme;
    }

    private void OnDisable()
    {
        InputUser.onChange -= ChangeControlScheme;
        gameplayMap.Disable();
        UIMap.Disable();
    }

    private void ChangeControlScheme(InputUser pUser, InputUserChange pChange, InputDevice pDevice)
    {
        if (pChange == InputUserChange.ControlSchemeChanged)
		{
            CurrentControlScheme = playerInput.currentControlScheme;
            ControlSchemeChanged?.Invoke(playerInput.currentControlScheme);
		}
    }

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
}