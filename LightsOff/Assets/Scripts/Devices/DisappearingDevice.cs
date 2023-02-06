using UnityEngine;

public class DisappearingDevice : Device
{
	protected Collider2D deviceCollider;

	protected override void Awake()
	{
		deviceCollider = GetComponent<Collider2D>();

		base.Awake();
	}

	public override void ApplyOnOffBehavior()
	{
		// Replace sprite renderer enabling by animation
		if (IsOnAndConnected())
		{
			spriteRenderer.sprite = spriteOn;
			deviceCollider.enabled = true;

			if (gameObject.CompareTag(Constants.TagOneWayPlatformTypeB))
				AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayOWBOn, gameObject);
		}
		else
		{
			spriteRenderer.sprite = spriteOff;
			deviceCollider.enabled = false;

			if (gameObject.CompareTag(Constants.TagOneWayPlatformTypeB))
				AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayOWBOff, gameObject);
		}
	}
}
