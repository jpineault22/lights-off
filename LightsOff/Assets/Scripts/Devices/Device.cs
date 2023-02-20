using UnityEngine;

public class Device : RotatableObject
{
	[SerializeField] protected bool isConnected = true;
	[SerializeField] protected bool isOn;
    [SerializeField] protected Sprite spriteOn = default;
    [SerializeField] protected Sprite spriteOff = default;
	[SerializeField] protected Sprite spriteInactive = default;

    protected SpriteRenderer spriteRenderer;
	protected SpriteRenderer outlineSpriteRenderer;

	public bool profiling;

	protected override void Awake()
	{
		base.Awake();
		
		spriteRenderer = GetComponent<SpriteRenderer>();

		AssignAudioEmitterToPlayerListener();

		foreach (GameObject obj in GameObjectUtils.GetChildren(gameObject))
		{
			if (obj.CompareTag(Constants.TagOutline))
			{
				outlineSpriteRenderer = obj.GetComponent<SpriteRenderer>();
				outlineSpriteRenderer.color = UIManager.Instance.deviceOutlineColor;
				ShowOutline(false, false, false);
			}
		}

		if (spriteInactive == null)
			spriteInactive = spriteOff;

		ApplyOnOffBehavior();
	}

	protected virtual void OnEnable()
	{
		GameManager.Instance.PlayerSpawned += AssignAudioEmitterToPlayerListener;
	}

	protected virtual void OnDisable()
	{
		GameManager.Instance.PlayerSpawned -= AssignAudioEmitterToPlayerListener;
	}

	public void SwitchOnOff()
	{
		isOn = !isOn;

		ApplyOnOffBehavior();
	}

	public void DisconnectReconnect()
	{
		isConnected = !isConnected;

		ApplyOnOffBehavior();
	}

	public virtual void ApplyOnOffBehavior()
	{
		if (isConnected)
		{
			if (isOn)
				spriteRenderer.sprite = spriteOn;
			else
				spriteRenderer.sprite = spriteOff;
		}
		else
		{
			spriteRenderer.sprite = spriteInactive;
		}
	}

	public virtual void ShowOutline(bool pShow, bool pFromBreaker, bool pSwitchBlocked)
	{
		ChangeOutlineColor(pFromBreaker, pSwitchBlocked);
		
		outlineSpriteRenderer.enabled = pShow;
	}

	public virtual void ChangeOutlineColor(bool pFromBreaker, bool pSwitchBlocked)
	{
		if (pFromBreaker)
		{
			outlineSpriteRenderer.color = UIManager.Instance.breakerOutlineColor;
		}
		else
		{
			if (profiling)
				Debug.Log("switch blocked: " + pSwitchBlocked);

			if (isConnected && !pSwitchBlocked)
				outlineSpriteRenderer.color = UIManager.Instance.deviceOutlineColor;
			else
				outlineSpriteRenderer.color = UIManager.Instance.inactiveOutlineColor;
		}
	}

	public bool IsOnAndConnected()
	{
		return isOn && isConnected;
	}

	public bool IsConnected()
	{
		return isConnected;
	}

	private void AssignAudioEmitterToPlayerListener()
	{
		AudioManager.Instance.AssignEmitterToPlayerListener(gameObject);
	}
}
