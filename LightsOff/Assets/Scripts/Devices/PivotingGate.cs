using UnityEngine;

public class PivotingGate : CycleDevice
{
	[SerializeField] private float rotatingSpeed = 350f;
	[SerializeField] private float failToSwitchTime = 0.03f;
	[SerializeField] private NearbyFunctionalFan[] nearbyFunctionalFans = default;
	
	private bool rotating = false;
	private bool attemptingSwitchToNext;
	private bool firstPartOfAttempt;
	private float failToSwitchTimer;

	private void FixedUpdate()
	{
		if (rotating)
		{
			ProcessSwitchAttempt();
			ProcessRotation();
		}
	}

	private void ProcessRotation()
	{
		Quaternion targetRotation = Quaternion.Euler(0, 0, 90 * (nbStates - currentState));                                         // States/rotation: 0/0, 1/270, 2/180, 3/90

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatingSpeed * Time.fixedDeltaTime);

		if (transform.rotation == targetRotation)
		{
			rotating = false;
		}
	}

	private void ProcessSwitchAttempt()
	{
		if (failToSwitchTimer > 0)
		{
			failToSwitchTimer -= Time.fixedDeltaTime;
		}
		else if (firstPartOfAttempt)
		{
			firstPartOfAttempt = false;

			if (attemptingSwitchToNext)
				DecrementState();
			else
				IncrementState();
		}
	}

	public override void ApplyBehavior()
	{
		if (isConnected)
		{
			rotating = true;
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPivotingGateRotates, gameObject);
		}
	}

	public override void FailToSwitch(bool pNextState)
	{
		rotating = firstPartOfAttempt = true;
		attemptingSwitchToNext = pNextState;
		failToSwitchTimer = failToSwitchTime;
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPivotingGateBlocks, gameObject);
	}

	private void OnTriggerStay2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagFunctionalFan) && !rotating)
		{
			foreach (NearbyFunctionalFan fan in nearbyFunctionalFans)
			{
				if (fan.functionalFan == pCollision.gameObject)
				{
					if (fan.directionLeft)
						SwitchToPreviousState(true);
					else
						SwitchToNextState(true);
					
					break;
				}
			}
		}
	}

	public bool IsRotating()
	{
		return rotating;
	}
}

[System.Serializable]
public struct NearbyFunctionalFan
{
	public GameObject functionalFan;
	public bool directionLeft;
}