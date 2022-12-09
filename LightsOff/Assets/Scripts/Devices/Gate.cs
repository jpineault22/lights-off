using UnityEngine;

public class Gate : Device
{
	private BoxCollider2D boxCollider;

	protected override void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		base.Awake();
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			spriteRenderer.enabled = true;
			boxCollider.enabled = true;
		}
		else
		{
			spriteRenderer.enabled = false;
			boxCollider.enabled = false;
		}
	}
}
