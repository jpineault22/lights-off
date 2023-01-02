using UnityEngine;

// The Moving Platform Type B works only with exactly 2 target points
public class MovingPlatformTypeB : MovingPlatform
{
	[SerializeField] private Sprite spriteOtherDirectionOn = default;
	[SerializeField] private Sprite spriteOtherDirectionOff = default;
	[SerializeField] private Sprite spriteOtherDirectionInactive = default;
	
	[HideInInspector] public bool readyToSwitch;
	private bool otherDirection;
	public bool Moving { get; set; }

	protected override void Awake()
	{
		base.Awake();

		readyToSwitch = true;
	}

	private void FixedUpdate()
	{
		if (isConnected)
		{
			if (Moving)
			{
				Vector2 target = targetPoints[currentTargetIndex].transform.position;

				// If the platform has not reached its target, it moves towards it, otherwise it stops. Set the readyToSwitch variable
				if (!CheckIfReachedTarget(target))
				{
					MoveTowardsTarget(target);
					readyToSwitch = false;
				}
				else
				{
					readyToSwitch = true;
					Moving = false;
					otherDirection = !otherDirection;
					SwitchOnOff();
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
}
