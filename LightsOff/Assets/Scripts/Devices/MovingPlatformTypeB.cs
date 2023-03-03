using UnityEngine;

// The Moving Platform Type B works only with exactly 2 target points
public class MovingPlatformTypeB : MovingPlatform
{
	[SerializeField] private Sprite spriteOtherDirectionOn = default;
	[SerializeField] private Sprite spriteOtherDirectionOff = default;
	[SerializeField] private Sprite spriteOtherDirectionInactive = default;
	
	private bool otherDirection;
	public bool Moving { get; private set; }

	private void OnDestroy()
	{
		if (IsOnAndConnected())
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopMovingPlatformTypeB, gameObject);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		
		if (isConnected)
		{
			if (Moving)
			{
				Vector2 target = targetPoints[currentTargetIndex].transform.position;

				// If the platform has not reached its target, it moves towards it, otherwise it stops. Set the readyToSwitch variable
				if (!CheckIfReachedTarget(target))
				{
					MoveTowardsTarget(target);
				}
				else
				{
					Moving = false;
					otherDirection = !otherDirection;
					SwitchOnOff();
					AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopMovingPlatformTypeB, gameObject);
				}
			}
		}
	}

	public override void ApplyOnOffBehavior()
	{
		if (isConnected)
		{
			if (isOn)
				spriteRenderer.sprite = otherDirection ? spriteOtherDirectionOn : spriteOn;
			else
				spriteRenderer.sprite = otherDirection ? spriteOtherDirectionOff : spriteOff;
		}
		else
		{
			spriteRenderer.sprite = otherDirection ? spriteOtherDirectionInactive : spriteInactive;
		}
	}

	public void StartMoving()
	{
		if (!Moving)
		{
			Moving = true;
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayMovingPlatformTypeB, gameObject);
		}
		else
		{
			IncrementTargetIndex();
			otherDirection = !otherDirection;
			SwitchOnOff();
		}
	}
}
