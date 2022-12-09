using UnityEngine;

public class TimerSwitch : Switch
{
	private SpriteRenderer spriteRenderer;
	
	[SerializeField] private float timerTime = 5f;
	[SerializeField] private Sprite spriteActive = default;
	[SerializeField] private Sprite spriteTriggered = default;

	private float timer = 0;
	private bool triggered = false;

	protected override void Awake()
	{
		base.Awake();

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (switchDevice != null && !switchDevice.IsConnected())
		{
			if (triggered)
			{
				triggered = false;
				timer = 0f;
				SwitchDevices();
			}
		}
		else if (timer > 0)
		{
			timer -= Time.fixedDeltaTime;
			triggered = true;
		}
		else if (triggered)
		{
			triggered = false;
			SwitchDevices();
			UpdateSprite();
		}
	}

	// Consider using a variable (switch on/off?) to avoid repeating the exact same Interact method from Switch
	public override void Interact()
	{
		if (triggered || blocked || (switchDevice != null && !switchDevice.IsConnected()))
		{
			return;
		}

		SwitchDevices();

		timer = timerTime;
		triggered = true;
		UpdateSprite();
	}

	private void SwitchDevices()
	{
		foreach (Device device in devices)
		{
			device.SwitchOnOff();
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}

	private void UpdateSprite()
	{
		if (triggered)
		{
			spriteRenderer.sprite = spriteTriggered;
		}
		else
		{
			spriteRenderer.sprite = spriteActive;
		}
	}
}
