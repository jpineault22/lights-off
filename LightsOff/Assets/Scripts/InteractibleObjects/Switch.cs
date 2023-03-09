using UnityEngine;

public class Switch : InteractibleObject
{
	[SerializeField] protected Device[] devices = default;

	protected Device switchDevice;                              // The Device script component of a switch. It should not be null. when connected to a breaker, it can activate/deactivate it.

	protected bool switchBlocked;

	protected override void Awake()
	{
		base.Awake();

		switchDevice = GetComponent<Device>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		foreach (Device device in devices)
		{
			device.DeviceBlocked += CheckIfDevicesBlocked;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		foreach (Device device in devices)
		{
			device.DeviceBlocked -= CheckIfDevicesBlocked;
		}
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (switchDevice.IsConnected() && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(true);
			ShowDevicesOutline(true);
		}
	}

	protected override void OnTriggerExit2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(false);
			ShowDevicesOutline(false);
		}
	}

	private void ShowDevicesOutline(bool pShow)
	{
		foreach (Device device in devices)
		{
			device.ShowOutline(pShow, false, switchBlocked);
		}
	}

	public override void Interact()
	{
		if (!switchDevice.IsConnected() || switchBlocked)
			return;

		switchDevice.SwitchOnOff();

		string wwiseEventName = switchDevice.IsOnAndConnected() ? Constants.WwiseEventPlaySwitchOn : Constants.WwiseEventPlaySwitchOff;
		AudioManager.Instance.TriggerWwiseEvent(wwiseEventName, gameObject);
		
		foreach (Device device in devices)
		{
			device.SwitchOnOff();

			if (device is MovingPlatformTypeB)
			{
				MovingPlatformTypeB platform = device as MovingPlatformTypeB;
				platform.StartMoving();
			}
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}

	private void CheckIfDevicesBlocked(GameObject pObject, bool pDeviceBlocked)
	{
		foreach (Device obj in devices)
		{
			if (obj.gameObject == pObject)
			{
				if (pDeviceBlocked && !switchBlocked)
				{
					switchBlocked = true;

					foreach (Device device in devices)
						device.ChangeOutlineColor(false, true);
				}
				else if (!pDeviceBlocked && switchBlocked)
				{
					switchBlocked = false;

					foreach (Device device in devices)
						device.ChangeOutlineColor(false, false);
				}

				return;
			}
		}
	}
}
