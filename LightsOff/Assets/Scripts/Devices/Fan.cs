using UnityEngine;

public class Fan : Device
{
	[SerializeField] private float forceMagnitude = 500f;
	[SerializeField] private float forceMagnitudeUpwards = 125f;
	
	private GameObject functionalFan;
	private SpriteRenderer functionalFanSpriteRenderer;
	private AreaEffector2D functionalFanAreaEffector;
	private BoxCollider2D functionalFanCollider;
	private Animator animator;
	private Animator outlineAnimator;

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
		functionalFanSpriteRenderer = functionalFan.GetComponent<SpriteRenderer>();
		functionalFanAreaEffector = functionalFan.GetComponent<AreaEffector2D>();
		functionalFanCollider = functionalFan.GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();

		//functionalFanAreaEffector.forceMagnitude = transform.localEulerAngles.z == 90 ? forceMagnitudeUpwards : forceMagnitude;
		
		base.Awake();

		functionalFanAreaEffector.forceMagnitude = objectRotation == ObjectRotation.West ? forceMagnitudeUpwards : forceMagnitude;
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			animator.enabled = true;
			outlineAnimator.enabled = true;

			functionalFanSpriteRenderer.enabled = true;
			//functionalFanAreaEffector.forceMagnitude = forceMagnitude;
			functionalFanCollider.enabled = true;
		}
		else
		{
			animator.enabled = false;
			outlineAnimator.enabled = false;

			functionalFanSpriteRenderer.enabled = false;
			//functionalFanAreaEffector.forceMagnitude = 0;
			functionalFanCollider.enabled = false;
		}
	}
}
