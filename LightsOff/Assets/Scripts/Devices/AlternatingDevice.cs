using UnityEngine;

public class AlternatingDevice : Device
{
    [SerializeField] private float positionA = default;
    [SerializeField] private float positionB = default;
	[SerializeField] private bool movesHorizontally = false;
	[SerializeField] private float movementDuration = 0.225f;

	private Collider2D deviceCollider;
	private Vector2? target;
	private float targetDistance;
	private float movementTimer;

	protected override void Awake()
	{
		base.Awake();

		deviceCollider = GetComponent<Collider2D>();
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
			deviceCollider.enabled = true;
		}
	}

	public override void ApplyOnOffBehavior()
	{
		// Add animations later on
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

		if (deviceCollider && deviceCollider.isTrigger)
			deviceCollider.enabled = false;

		if (gameObject.CompareTag(Constants.TagLadder))
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayLadderMoves, gameObject);
		else
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayGateMoves, gameObject);
	}
}
