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

		string wwiseEventName = breakerDevice.IsOnAndConnected() ? Constants.WwiseEventPlayBreakerOn : Constants.WwiseEventPlayBreakerOff;
		AudioManager.Instance.TriggerWwiseEvent(wwiseEventName, gameObject);
		
		foreach (Device device in devices)
		{
			device.DisconnectReconnect();
		}

		GameManager.Instance.CheckIfAllLightsOff();
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(true);

			foreach (Device device in devices)
			{
				device.ShowOutline(true, true);
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(false);

			foreach (Device device in devices)
			{
				device.ShowOutline(false, true);
			}
		}
	}
}
