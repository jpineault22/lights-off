using UnityEngine;

public class CycleSwitch : InteractibleObject
{
	[SerializeField] private CycleDevice[] devices = default;
	[SerializeField] private bool cycleSwitchLeft = false;          // Right being clockwise and left being counter-clockwise
	[SerializeField] private float pressedTime = 0.15f;
	
	private Device switchDevice;									// For a cycle switch, ON is the short time during which the switch is pressed, while OFF is the regular "active" switch

	private float pressedCounter;

	protected override void Awake()
	{
		base.Awake();

		switchDevice = GetComponent<Device>();

		if (cooldownTime < pressedTime)
			cooldownTime = pressedTime;
	}

	private void Update()
	{
		if (switchDevice.IsOnAndConnected())
		{
			if (pressedCounter > 0)
				pressedCounter -= Time.deltaTime;
			else
				switchDevice.SwitchOnOff();
		}
		else
		{
			pressedCounter = 0f;
		}
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (switchDevice.IsConnected() && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			interactMessageText.enabled = true;
			InRange = true;
		}
	}

	public override void Interact()
	{
		if (cooldownCounter > 0 || !switchDevice.IsConnected())
		{
			return;
		}

		switchDevice.SwitchOnOff();
		pressedCounter = pressedTime;

		foreach (CycleDevice device in devices)
		{
			if (cycleSwitchLeft)
			{
				device.SwitchToPreviousState();
			}
			else
			{
				device.SwitchToNextState();
			}
		}

		cooldownCounter = cooldownTime;
	}
}
