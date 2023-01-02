using UnityEngine;

public class Device : RotatableObject
{
	[SerializeField] protected bool isConnected = true;
	[SerializeField] protected bool isOn;
    [SerializeField] protected Sprite spriteOn = default;
    [SerializeField] protected Sprite spriteOff = default;
	[SerializeField] protected Sprite spriteInactive = default;

    protected SpriteRenderer spriteRenderer;

	protected override void Awake()
	{
		base.Awake();
		
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (spriteInactive == null)
			spriteInactive = spriteOff;

		ApplyOnOffBehavior();
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

	public bool IsOnAndConnected()
	{
		return isOn && isConnected;
	}

	public bool IsConnected()
	{
		return isConnected;
	}
}
