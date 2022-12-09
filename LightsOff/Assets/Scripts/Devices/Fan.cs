using UnityEngine;

public class Fan : Device
{
	[SerializeField] private float forceMagnitude = 500f;
	[SerializeField] private float forceMagnitudeUpwards = 125f;
	
	private GameObject functionalFan;
	private SpriteRenderer functionalFanSpriteRenderer;
	private AreaEffector2D functionalFanAreaEffector;
	private BoxCollider2D functionalFanCollider;

	protected override void Awake()
	{
		functionalFan = GameObjectUtils.GetChildren(gameObject)[0];
		functionalFanSpriteRenderer = functionalFan.GetComponent<SpriteRenderer>();
		functionalFanAreaEffector = functionalFan.GetComponent<AreaEffector2D>();
		functionalFanCollider = functionalFan.GetComponent<BoxCollider2D>();

		//functionalFanAreaEffector.forceMagnitude = transform.localEulerAngles.z == 90 ? forceMagnitudeUpwards : forceMagnitude;
		
		base.Awake();

		functionalFanAreaEffector.forceMagnitude = objectRotation == ObjectRotation.West ? forceMagnitudeUpwards : forceMagnitude;
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			// Switch for an animation eventually
			spriteRenderer.sprite = spriteOn;

			functionalFanSpriteRenderer.enabled = true;
			//functionalFanAreaEffector.forceMagnitude = forceMagnitude;
			functionalFanCollider.enabled = true;
		}
		else
		{
			spriteRenderer.sprite = spriteOff;

			functionalFanSpriteRenderer.enabled = false;
			//functionalFanAreaEffector.forceMagnitude = 0;
			functionalFanCollider.enabled = false;
		}
	}
}
