using UnityEngine;

public class Breaker : InteractibleObject
{
	[SerializeField] private Device[] devices = default;

	private Device breakerDevice = default;

	protected override void Awake()
	{
		base.Awake();

		breakerDevice = GetComponent<Device>();
	}

	public override void Interact()
	{
		breakerDevice.SwitchOnOff();
		
		foreach (Device device in devices)
		{
			device.DisconnectReconnect();
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}
}
