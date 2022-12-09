using UnityEngine;

// The Moving Platform Type B works only with exactly 2 target points
public class MovingPlatformTypeB : MovingPlatform
{
	public bool readyToSwitch;

	protected override void Awake()
	{
		base.Awake();

		readyToSwitch = true;
	}

	private void FixedUpdate()
	{
		if (isConnected)
		{
			// The variable isOn is used here as an alternating state between the two possible positions for the Type B platform
			if ((isOn && currentTargetIndex == 1) || (!isOn && currentTargetIndex == 0))
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
				}
			}
		}
	}
}
