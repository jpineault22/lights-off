using UnityEngine;

public class Breaker : InteractibleObject
{
	[SerializeField] private Device[] devices = default;
	//[SerializeField] private Color outlineColor = Color.white;	// TO IMPLEMENT

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

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(true);

			foreach (Device device in devices)
			{
				device.ShowOutline(true);
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
				device.ShowOutline(false);
			}
		}
	}
}
