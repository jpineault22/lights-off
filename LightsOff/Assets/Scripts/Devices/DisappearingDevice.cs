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
		}
		else
		{
			spriteRenderer.sprite = spriteOff;
			deviceCollider.enabled = false;
		}
	}
}
