using UnityEngine;

public class Fan : Device
{
	[SerializeField] private float forceMagnitude = 500f;
	[SerializeField] private float forceMagnitudeUpwards = 125f;
	
	public ObjectOrientation Orientation { get; private set; }
	private GameObject functionalFan;
	private ParticleSystem functionalFanParticleSystem;
	private AreaEffector2D functionalFanAreaEffector;
	private BoxCollider2D functionalFanCollider;
	private Animator animator;
	private Animator outlineAnimator;

	private void OnValidate()
	{
		switch(transform.localEulerAngles.z)
		{
			case 90:
				Orientation = ObjectOrientation.North;
				break;
			case 180:
				Orientation = ObjectOrientation.West;
				break;
			case 270:
				Orientation = ObjectOrientation.South;
				break;
			case 0:
				Orientation = ObjectOrientation.East;
				break;
			default:
				Orientation = ObjectOrientation.North;
				break;
		}
	}

	protected override void Awake()
	{
		foreach (GameObject obj in GameObjectUtils.GetChildren(gameObject))
		{
			if (obj.CompareTag(Constants.TagFunctionalFan))
				functionalFan = obj;
			else if (obj.CompareTag(Constants.TagOutline))
				outlineAnimator = obj.GetComponent<Animator>();
		}

		//functionalFan = GameObjectUtils.GetChildren(gameObject)[0];
		functionalFanParticleSystem = functionalFan.GetComponent<ParticleSystem>();
		functionalFanAreaEffector = functionalFan.GetComponent<AreaEffector2D>();
		functionalFanCollider = functionalFan.GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();

		//functionalFanAreaEffector.forceMagnitude = transform.localEulerAngles.z == 90 ? forceMagnitudeUpwards : forceMagnitude;
		
		base.Awake();

		functionalFanAreaEffector.forceMagnitude = objectRotation == ObjectOrientation.West ? forceMagnitudeUpwards : forceMagnitude;
	}

	private void OnDestroy()
	{
		if (IsOnAndConnected())
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopFan, gameObject);
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			animator.enabled = true;
			outlineAnimator.enabled = true;

			functionalFanParticleSystem.Play();
			//functionalFanAreaEffector.forceMagnitude = forceMagnitude;
			functionalFanCollider.enabled = true;

			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayFan, gameObject);
		}
		else
		{
			animator.enabled = false;
			outlineAnimator.enabled = false;

			functionalFanParticleSystem.Stop();
			//functionalFanAreaEffector.forceMagnitude = 0;
			functionalFanCollider.enabled = false;

			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopFan, gameObject);
		}
	}
}
