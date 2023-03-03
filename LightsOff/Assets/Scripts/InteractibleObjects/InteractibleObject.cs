using UnityEngine;
using TMPro;
using UnityEngine.UI;

// This class defines objects that have a trigger collider displaying a short message when in range, and with which the player can interact (such as switches, breakers and exit doors)
public abstract class InteractibleObject : MonoBehaviour
{
	[SerializeField] protected Image interactMessageBackground = default;
	[SerializeField] protected TMP_Text interactMessageText = default;
	[SerializeField] private Image interactMessageImage = default;
	[SerializeField] private InteractMessage interactMessage = default;
	[SerializeField] protected float cooldownTime = 0.5f;

	public bool InRange { get; protected set; }
	protected float cooldownCounter = 0;

	protected virtual void Awake()
	{
		AssignAudioEmitterToPlayerListener();

		if (interactMessage == InteractMessage.Press) interactMessageText.text = Constants.UIInteractMessagePress;
		else if (interactMessage == InteractMessage.Enter) interactMessageText.text = Constants.UIInteractMessageEnter;
		else if (interactMessage == InteractMessage.Sleep) interactMessageText.text = Constants.UIInteractMessageSleep;

		EnableDisableInteractMessage(false);
		interactMessageText.color = UIManager.Instance.accessibleColor;
	}

	private void Start()
	{
		InputManager.Instance.ControlSchemeChanged += ctx => UpdateInteractMessage(ctx);

		UpdateInteractMessage(InputManager.Instance.CurrentControlScheme);
	}

	protected virtual void OnEnable()
	{
		GameManager.Instance.PlayerSpawned += AssignAudioEmitterToPlayerListener;
	}

	protected virtual void OnDisable()
	{
		GameManager.Instance.PlayerSpawned -= AssignAudioEmitterToPlayerListener;
	}

	protected virtual void Update()
	{
		if (interactMessageText.enabled)
		{
			UpdateInteractMessageColor(PlayerController.Instance.CanAccessInteractibleObject());
		}
	}

	protected virtual void FixedUpdate()
	{
		if (cooldownCounter > 0)
		{
			cooldownCounter -= Time.fixedDeltaTime;
		}
	}

	private void UpdateInteractMessage(string pControlScheme)
	{

		if (interactMessageImage != null)
		{
			if (pControlScheme == Constants.InputControlSchemeKeyboardMouse) interactMessageImage.sprite = UIManager.Instance.interactMessageImageKeyboard;
			else if (pControlScheme == Constants.InputControlSchemeGamepad) interactMessageImage.sprite = UIManager.Instance.interactMessageImageController;
		}
	}

	private void UpdateInteractMessageColor(bool pAccessible)
	{
		interactMessageText.color = pAccessible ? UIManager.Instance.accessibleColor : UIManager.Instance.inaccessibleColor;
	}

	protected void EnableDisableInteractMessage(bool pValue)
	{
		interactMessageBackground.enabled = pValue;
		interactMessageText.enabled = pValue;
		if (interactMessageImage != null)
			interactMessageImage.enabled = pValue;
		InRange = pValue;
	}

	protected virtual void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(true);
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(false);
		}
	}

	public abstract void Interact();

	private void AssignAudioEmitterToPlayerListener()
	{
		AudioManager.Instance.AssignEmitterToPlayerListener(gameObject);
	}
}

public enum InteractMessage
{
	Press,
	Enter,
	Sleep
}