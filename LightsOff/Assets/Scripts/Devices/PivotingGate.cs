using UnityEngine;

public class PivotingGate : CycleDevice
{
	[SerializeField] private float rotatingSpeed = 350f;
	[SerializeField] private NearbyFunctionalFan[] nearbyFunctionalFans = default;
	
	private bool rotating = false;

	private void FixedUpdate()
	{
		if (rotating)
		{
			Quaternion targetRotation = Quaternion.Euler(0, 0, 90 * (nbStates - currentState));											// States/rotation: 0/0, 1/270, 2/180, 3/90

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatingSpeed * Time.fixedDeltaTime);

			if (transform.rotation == targetRotation)
			{
				rotating = false;
			}
		}
	}

	public override void ApplyBehavior()
	{
		rotating = true;
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
					{
						SwitchToPreviousState();
					}
					else
					{
						SwitchToNextState();
					}
					
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