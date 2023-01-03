using UnityEngine;

public class TimerSwitch : Switch
{
	private SpriteRenderer spriteRenderer;
	
	[SerializeField] private float timerTime = 5f;
	[SerializeField] private Sprite spriteActive = default;
	[SerializeField] private Sprite[] spritesTriggered = default;

	private float timer = 0;
	private bool triggered = false;

	protected override void Awake()
	{
		base.Awake();

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (!switchDevice.IsConnected())
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
			triggered = true;
			spriteRenderer.sprite = spritesTriggered[Mathf.CeilToInt(timer * spritesTriggered.Length / timerTime) - 1];
			timer -= Time.deltaTime;
		}
		else if (triggered)
		{
			triggered = false;
			SwitchDevices();
			spriteRenderer.sprite = spriteActive;
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
		spriteRenderer.sprite = spritesTriggered[0];
	}

	private void SwitchDevices()
	{
		foreach (Device device in devices)
		{
			device.SwitchOnOff();

			if (device is MovingPlatformTypeB)
			{
				MovingPlatformTypeB platform = device as MovingPlatformTypeB;
				platform.Moving = true;
			}
		}

		if (PlayerController.Instance.CurrentCharacterState != CharacterState.LevelTransition)
			GameManager.Instance.CheckIfAllLightsOff();
	}
}
