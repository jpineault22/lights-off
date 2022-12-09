using UnityEngine;

public class MovingPlatformTypeA : MovingPlatform
{
	[SerializeField] protected float idleTime = 2f;

	private float idleCounter;

	protected override void Awake()
	{
		base.Awake();

		idleCounter = idleTime;
	}

	private void FixedUpdate()
	{
		if (isOn && isConnected)
		{
			Vector2 target = targetPoints[currentTargetIndex].transform.position;

			// If the platform reaches its target, set the Idle Counter, increment the Target Index and reset the velocity
			if (CheckIfReachedTarget(target))
			{
				idleCounter = idleTime;
			}

			// If the platform is idle, update the counter, otherwise move the platform towards the target
			if (idleCounter > 0)
			{
				idleCounter -= Time.fixedDeltaTime;
			}
			else
			{
				MoveTowardsTarget(target);
			}
		}
		else
		{
			StopPlatform();
		}
	}
}
