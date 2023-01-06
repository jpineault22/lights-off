using UnityEngine;

public abstract class CycleDevice : RotatableObject
{
    [SerializeField] protected bool isConnected = true;
	[SerializeField] protected int numberOfStates = 4;
	[SerializeField] protected Device blockingDevice = default;		// If the CycleDevice has a blocking device, when the latter is on, it prevents the object from switching to the next or previous state at a certain threshold.
																	// Ex.: A Gate Type-B moves in a position where it blocks a pivoting gate and prevents it from rotating.
	public bool hasLimits;
	public int stateLowerLimit;
	public int stateUpperLimit;
	public int blockingDeviceUpperThreshold;		// A blocking device that is on will add an additional lower and upper limit to the CycleDevice, this field being the lower limit, above the threshold,
													// and the upper limit being the state just under it. If the CycleDevice already has limits, it will split the accessible states in 2 ranges.
													// Ex.: A pivoting gate having limits (lower: 0, upper: 3) and an upper threshold of 2 will have new limits (lower: 2, upper: 3 OR lower: 0, upper: 1), depending on its current state.
	protected int nbStates;
	protected int currentState = 0;			// The current state of the cycle device, ranging between 0 and (nbStates - 1).

	// Fields used to check in OnValidate if the values of the corresponding fields have been changed in the inspector, in order to validate and correct those values.
	private int numberOfStatesChangeCheck = -1000;
	private int stateLowerLimitChangeCheck = -1000;
	private int stateUpperLimitChangeCheck = -1000;

	protected SpriteRenderer outlineSpriteRenderer;

	private void OnValidate()
	{
		if (numberOfStatesChangeCheck == -1000) numberOfStatesChangeCheck = numberOfStates;
		if (stateLowerLimitChangeCheck == -1000) stateLowerLimitChangeCheck = stateLowerLimit;
		if (stateUpperLimitChangeCheck == -1000) stateUpperLimitChangeCheck = stateUpperLimit;

		if (numberOfStates != numberOfStatesChangeCheck)
		{
			stateLowerLimit = stateLowerLimitChangeCheck = 0;
			stateUpperLimit = stateUpperLimitChangeCheck = numberOfStates - 1;
			
			numberOfStatesChangeCheck = numberOfStates;
		}

		if (stateLowerLimit != stateLowerLimitChangeCheck)
		{
			if (stateLowerLimit < 0)
			{
				stateLowerLimit = 0;
			}
			else if (stateLowerLimit > numberOfStates - 1)
			{
				stateLowerLimit = numberOfStates - 1;
			}

			stateLowerLimitChangeCheck = stateLowerLimit;
		}

		if (stateUpperLimit != stateUpperLimitChangeCheck)
		{
			if (stateUpperLimit < 0)
			{
				stateUpperLimit = 0;
			}
			else if (stateUpperLimit > numberOfStates - 1)
			{
				stateUpperLimit = numberOfStates - 1;
			}

			stateUpperLimitChangeCheck = stateUpperLimit;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		nbStates = numberOfStates;

		if (nbStates == 4)
		{
			switch (objectRotation)
			{
				case ObjectRotation.North:
					currentState = 0;
					break;
				case ObjectRotation.East:
					currentState = 1;
					break;
				case ObjectRotation.South:
					currentState = 2;
					break;
				case ObjectRotation.West:
					currentState = 3;
					break;
				default:
					currentState = 0;
					break;
			}
		}

		foreach (GameObject obj in GameObjectUtils.GetChildren(gameObject))
		{
			if (obj.CompareTag(Constants.TagOutline))
			{
				outlineSpriteRenderer = obj.GetComponent<SpriteRenderer>();
				outlineSpriteRenderer.color = UIManager.Instance.deviceOutlineColor;
				ShowOutline(false);
			}
		}
	}

	public void SwitchToNextState()
	{
		if ((!hasLimits || currentState != stateUpperLimit) && (!BlockingDeviceIsOn() || currentState != blockingDeviceUpperThreshold - 1))
		{
			currentState++;

			if (currentState >= nbStates)
			{
				currentState = 0;
			}

			ApplyBehavior();
		}
	}

	public void SwitchToPreviousState()
	{
		if ((!hasLimits || currentState != stateLowerLimit) && (!BlockingDeviceIsOn() || currentState != blockingDeviceUpperThreshold))
		{
			currentState--;

			if (currentState < 0)
			{
				currentState = nbStates - 1;
			}

			ApplyBehavior();
		}
	}

	private bool BlockingDeviceIsOn()
	{
		return blockingDevice != null && blockingDevice.IsOnAndConnected();
	}

    public void DisconnectReconnect()
	{
		isConnected = !isConnected;

		ApplyBehavior();
	}

	public abstract void ApplyBehavior();

	public virtual void ShowOutline(bool pShow)
	{
		outlineSpriteRenderer.enabled = pShow;
	}

	public bool IsConnected()
	{
		return isConnected;
	}
}