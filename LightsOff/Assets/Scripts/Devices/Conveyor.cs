﻿using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Device
{
	[SerializeField] private bool directionRight = default;
	[SerializeField] private float force = 500f;
	[SerializeField] private Sprite[] conveyorSprites = default;
	[SerializeField] private float spriteChangeTime = 0.1f;

	private List<SpriteRenderer> conveyorBlocks;
	private List<SpriteRenderer> conveyorBlockOutlines;
	private float spriteChangeTimer;
	private int currentSprite;

	protected override void Awake()
	{
		conveyorBlocks = new List<SpriteRenderer>();
		conveyorBlockOutlines = new List<SpriteRenderer>();
		
		foreach (GameObject child in GameObjectUtils.GetChildren(gameObject))
		{
			SpriteRenderer block = child.GetComponent<SpriteRenderer>();
			block.flipX = directionRight;
			conveyorBlocks.Add(block);

			foreach (GameObject obj in GameObjectUtils.GetChildren(child))
			{
				if (obj.CompareTag(Constants.TagOutline))
				{
					SpriteRenderer outline = obj.GetComponent<SpriteRenderer>();
					outline.color = UIManager.Instance.deviceOutlineColor;
					conveyorBlockOutlines.Add(outline);
				}
			}
		}

		ShowOutline(false, false);

		if (IsOnAndConnected())
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayConveyor, gameObject);
	}

	private void OnDestroy()
	{
		if (IsOnAndConnected())
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopConveyor, gameObject);
	}

	private void Update()
	{
		if (IsOnAndConnected())
		{
			if (spriteChangeTimer > 0)
			{
				spriteChangeTimer -= Time.deltaTime;
			}
			else
			{
				spriteChangeTimer = spriteChangeTime;

				if (directionRight)
				{
					currentSprite++;

					if (currentSprite > conveyorSprites.Length - 1)
						currentSprite = 0;
				}
				else
				{
					currentSprite--;

					if (currentSprite < 0)
						currentSprite = conveyorSprites.Length - 1;
				}

				foreach (SpriteRenderer block in conveyorBlocks)
				{
					block.sprite = conveyorSprites[currentSprite];
				}
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
			else if (PlayerController.Instance.CurrentCharacterState == CharacterState.Dying ||PlayerController.Instance.CurrentCharacterState == CharacterState.LevelTransition ||
				PlayerController.Instance.CurrentCharacterState == CharacterState.Climbing)
			{
				return;
			}

			collision.gameObject.GetComponent<Rigidbody2D>().AddForce(forceVector);
		}
	}

	public bool IsDirectionRight()
	{
		return directionRight;
	}

	// The physics part is handled in OnCollisionStay2D, whereas the animation is handled in Update
	// Only the audio is handled here, but this would need to be changed to make it cleaner and more consistent
	public override void ApplyOnOffBehavior()
	{
		string wwiseEventName = IsOnAndConnected() ? Constants.WwiseEventPlayConveyor : Constants.WwiseEventStopConveyor;
		AudioManager.Instance.TriggerWwiseEvent(wwiseEventName, gameObject);

		return;
	}

	public override void ShowOutline(bool pShow, bool pFromBreaker)
	{
		ChangeOutlineColor(pFromBreaker);

		foreach (SpriteRenderer blockOutline in conveyorBlockOutlines)
		{
			blockOutline.enabled = pShow;
		}
	}

	public override void ChangeOutlineColor(bool pFromBreaker)
	{
		if (pFromBreaker)
		{
			ChangeConveyorBlocksColor(UIManager.Instance.breakerOutlineColor);
		}
		else
		{
			if (isConnected)
				ChangeConveyorBlocksColor(UIManager.Instance.deviceOutlineColor);
			else
				ChangeConveyorBlocksColor(UIManager.Instance.inactiveOutlineColor);
		}
	}

	private void ChangeConveyorBlocksColor(Color pColor)
	{
		foreach (SpriteRenderer blockOutline in conveyorBlockOutlines)
		{
			blockOutline.color = pColor;
		}
	}
}
