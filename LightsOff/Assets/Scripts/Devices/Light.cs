using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Light : Device
{
	private Light2D haloLight;
	private Light2D directionalLight;

	protected override void Awake()
	{
		foreach (GameObject obj in GameObjectUtils.GetChildren(gameObject))
		{
			if (obj.CompareTag(Constants.TagHaloLight))
				haloLight = obj.GetComponent<Light2D>();
			else if (obj.CompareTag(Constants.TagDirectionalLight))
				directionalLight = obj.GetComponent<Light2D>();
		}
		
		base.Awake();
	}

	public override void ApplyOnOffBehavior()
	{
		if (isConnected)
		{
			if (isOn)
			{
				spriteRenderer.sprite = spriteOn;
				SwitchLightsOnOff(true);
			}
			else
			{
				spriteRenderer.sprite = spriteOff;
				SwitchLightsOnOff(false);
			}
		}
		else
		{
			spriteRenderer.sprite = spriteInactive;
			SwitchLightsOnOff(false);
		}
	}

	private void SwitchLightsOnOff(bool pOn)
	{
		haloLight.enabled = pOn;
		directionalLight.enabled = pOn;
	}
}
