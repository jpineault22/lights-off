using UnityEngine;

public class AlternatingDevice : Device
{
    [SerializeField] private float positionA = default;
    [SerializeField] private float positionB = default;
	[SerializeField] private bool movesHorizontally = false;
	[SerializeField] private float movementDuration = 0.225f;
	[SerializeField] private float enemyDistanceBuffer = 1f;
	[SerializeField] private ParticleSystem[] dustParticles;

	private Collider2D deviceCollider;
	private Vector2? target;
	private float targetDistance;
	private float movementTimer;
	private float deviceLeftEdge;
	private float deviceRightEdge;

	protected override void Awake()
	{
		base.Awake();

		deviceCollider = GetComponent<Collider2D>();

		deviceLeftEdge = transform.position.x - spriteRenderer.bounds.extents.x;
		deviceRightEdge = transform.position.x + spriteRenderer.bounds.extents.x;
	}

	protected virtual void Update()
	{
		if (movementTimer > 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, (Vector2) target, targetDistance * Time.deltaTime / movementDuration);
			
			movementTimer -= Time.deltaTime;
		}
		else if (target != null)
		{
			transform.position = (Vector2) target;
			target = null;
			deviceCollider.enabled = true;

			foreach (ParticleSystem particles in dustParticles)
			{
				particles.Stop();
				var module = particles.velocityOverLifetime;
				module.speedModifierMultiplier *= -1;
			}
		}
	}

	private void FixedUpdate()
	{
		if (spawnedEnemy && gameObject.CompareTag(Constants.TagGate))
			CheckIfDeviceBlocked(deviceLeftEdge, deviceRightEdge, enemyDistanceBuffer);
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			spriteRenderer.sprite = spriteOn;

			if (movesHorizontally)
			{
				target = new Vector2(positionA, transform.position.y);
				targetDistance = Mathf.Abs(transform.position.x - positionA);
			}
			else
			{
				target = new Vector2(transform.position.x, positionA); ;
				targetDistance = Mathf.Abs(transform.position.y - positionA);
			}
		}
		else
		{
			spriteRenderer.sprite = spriteOff;

			if (movesHorizontally)
			{
				target = new Vector2(positionB, transform.position.y);
				targetDistance = Mathf.Abs(transform.position.x - positionB);
			}
			else
			{
				target = new Vector2(transform.position.x, positionB);
				targetDistance = Mathf.Abs(transform.position.y - positionB);
			}
		}

		movementTimer = movementDuration;

		foreach (ParticleSystem particles in dustParticles)
			particles.Play();

		if (deviceCollider && deviceCollider.isTrigger)
			deviceCollider.enabled = false;

		if (gameObject.CompareTag(Constants.TagLadder))
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayLadderMoves, gameObject);
		else
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayGateMoves, gameObject);
	}
}
