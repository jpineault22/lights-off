using UnityEngine;

public class Device : RotatableObject
{
	[SerializeField] protected bool isConnected = true;
	[SerializeField] protected bool isOn;
    [SerializeField] protected Sprite spriteOn = default;
    [SerializeField] protected Sprite spriteOff = default;

    protected SpriteRenderer spriteRenderer;

	protected override void Awake()
	{
		base.Awake();
		
		spriteRenderer = GetComponent<SpriteRenderer>();

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
		if (IsOnAndConnected())
		{
			spriteRenderer.sprite = spriteOn;
		}
		else
		{
			spriteRenderer.sprite = spriteOff;
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
