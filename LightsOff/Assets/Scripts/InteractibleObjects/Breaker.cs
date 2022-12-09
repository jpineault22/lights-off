using UnityEngine;

public class Breaker : InteractibleObject
{
	[SerializeField] private Device[] devices = default;

	public override void Interact()
	{
		foreach (Device device in devices)
		{
			device.DisconnectReconnect();
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}
}
