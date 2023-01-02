using UnityEngine;

public class AlternatingDevice : Device
{
    [SerializeField] private float positionA = default;
    [SerializeField] private float positionB = default;
	[SerializeField] private bool movesHorizontally = false;

	public override void ApplyOnOffBehavior()
	{
		// Add animations later on
		if (IsOnAndConnected())
		{
			spriteRenderer.sprite = spriteOn;
			transform.position = movesHorizontally
				? new Vector2(positionA, transform.position.y)
				: new Vector2(transform.position.x, positionA);
		}
		else
		{
			spriteRenderer.sprite = spriteOff;
			transform.position = movesHorizontally
				? new Vector2(positionB, transform.position.y)
				: new Vector2(transform.position.x, positionB);
		}
	}
}
