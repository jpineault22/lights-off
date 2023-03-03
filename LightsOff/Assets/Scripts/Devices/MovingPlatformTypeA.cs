using UnityEngine;

public class MovingPlatformTypeA : MovingPlatform
{
	[SerializeField] protected float idleTime = 2f;

	private float idleCounter;
	private bool sfxStarted;

	protected override void Awake()
	{
		base.Awake();

		idleCounter = idleTime;
	}

	private void OnDestroy()
	{
		if (IsOnAndConnected())
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopMovingPlatformTypeA, gameObject);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if (isOn && isConnected)
		{
			Vector2 target = targetPoints[currentTargetIndex].transform.position;

			// If the platform reaches its target, set the Idle Counter, increment the Target Index and reset the velocity
			if (CheckIfReachedTarget(target))
			{
				idleCounter = idleTime;
				AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopMovingPlatformTypeA, gameObject);
				sfxStarted = false;
			}

			// If the platform is idle, update the counter, otherwise move the platform towards the target
			if (idleCounter > 0)
			{
				idleCounter -= Time.fixedDeltaTime;
			}
			else
			{
				if (!sfxStarted && GameManager.Instance.CurrentGameState == GameState.Playing)
				{
					AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayMovingPlatformTypeA, gameObject);
					sfxStarted = true;
				}
				
				MoveTowardsTarget(target);
			}
		}
		else
		{
			StopPlatform();

			if (sfxStarted)
			{
				AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopMovingPlatformTypeA, gameObject);
				sfxStarted = false;
			}
		}
	}
}
