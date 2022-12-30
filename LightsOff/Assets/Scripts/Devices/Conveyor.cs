using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Device
{
	[SerializeField] private bool directionRight = default;
	[SerializeField] private float force = 500f;

	private List<SpriteRenderer> conveyorBlocks = default;

	protected override void Awake()
	{
		conveyorBlocks = new List<SpriteRenderer>();
		
		foreach (Transform child in transform)
		{
			SpriteRenderer block = child.GetComponent<SpriteRenderer>();
			block.flipX = !directionRight;
			conveyorBlocks.Add(block);
		}

		ApplyOnOffBehavior();
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			foreach (SpriteRenderer block in conveyorBlocks)
			{
				block.sprite = spriteOn;
			}
		}
		else
		{
			foreach (SpriteRenderer block in conveyorBlocks)
			{
				block.sprite = spriteOff;
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<Rigidbody2D>() == null)
			return;

		if (IsOnAndConnected())
		{
			int direction = directionRight ? 1 : -1;
			Vector2 forceVector = new Vector2(force * direction, 0f);

			if (!collision.gameObject.CompareTag(Constants.TagPlayer))
			{
				collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			}

			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(forceVector);
		}
	}

	public bool IsDirectionRight()
	{
		return directionRight;
	}
}
