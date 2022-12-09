using UnityEngine;

public class CycleSwitch : InteractibleObject
{
	[SerializeField] private CycleDevice[] devices = default;
	[SerializeField] private bool cycleSwitchLeft = false;			// Right being clockwise and left being counter-clockwise
	
	private Device switchDevice;

	protected override void Awake()
	{
		base.Awake();

		switchDevice = GetComponent<Device>();
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if ((switchDevice == null || switchDevice.IsConnected()) && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			interactMessageText.enabled = true;
			InRange = true;
		}
	}

	public override void Interact()
	{
		if (cooldownCounter > 0 || (switchDevice != null && !switchDevice.IsConnected()))
		{
			return;
		}

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
