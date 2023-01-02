using UnityEngine;
using System.Collections.Generic;

public class Switch : InteractibleObject
{
	[SerializeField] protected Device[] devices = default;

	protected Device switchDevice;											// The Device script component of a switch. It should not be null. when connected to a breaker, it can activate/deactivate it.
	protected List<MovingPlatformTypeB> blockingDevices;					// Adapt if you add more types of blocking devices
	protected bool blocked;

	protected override void Awake()
	{
		base.Awake();

		switchDevice = GetComponent<Device>();
		blockingDevices = new List<MovingPlatformTypeB>();
		blocked = false;

		foreach (Device device in devices)
		{
			if (device is MovingPlatformTypeB)
			{
				blockingDevices.Add((MovingPlatformTypeB) device);
			}
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		
		foreach (MovingPlatformTypeB blockingDevice in blockingDevices)
		{
			if (!blockingDevice.readyToSwitch)
			{
				blocked = true;
				break;
			}
			else
			{
				blocked = false;
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (!blocked && switchDevice.IsConnected() && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			interactMessageText.enabled = true;
			InRange = true;
		}
	}

	public override void Interact()
	{
		if (blocked || !switchDevice.IsConnected())
		{
			return;
		}

		switchDevice.SwitchOnOff();
		
		foreach (Device device in devices)
		{
			device.SwitchOnOff();

			if (device is MovingPlatformTypeB)
			{
				MovingPlatformTypeB platform = device as MovingPlatformTypeB;
				platform.Moving = true;
			}
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}
}
