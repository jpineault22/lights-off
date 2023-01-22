using UnityEngine;

public class MovingPlatform : Device
{
	protected Rigidbody2D rb;

	[SerializeField] protected GameObject[] targetPoints = default;
	[SerializeField] protected float speed = 5f;

    protected int currentTargetIndex;

	protected override void Awake()
	{
		base.Awake();
		
		rb = GetComponent<Rigidbody2D>();

		currentTargetIndex = 0;
	}

	protected void MoveTowardsTarget(Vector2 pTarget)
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing)
			transform.position = Vector3.MoveTowards(transform.position, pTarget, speed * Time.fixedDeltaTime);
	}

	protected void StopPlatform()
	{
		rb.velocity = Vector2.zero;
	}

	protected bool CheckIfReachedTarget(Vector2 pTarget)
	{
		if (Mathf.Abs(pTarget.x - transform.position.x) <= 0.01f && Mathf.Abs(pTarget.y - transform.position.y) <= 0.01f)
		{
			currentTargetIndex++;

			if (currentTargetIndex >= targetPoints.Length)
			{
				currentTargetIndex = 0;
			}

			StopPlatform();

			return true;
		}

		return false;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing && collision.gameObject.CompareTag(Constants.TagPlayer) && PlayerController.Instance.IsGrounded())
		{
			collision.gameObject.transform.SetParent(transform);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(Constants.TagPlayer))
		{
			collision.gameObject.transform.SetParent(null);
			DontDestroyOnLoad(collision.gameObject);
		}
	}
}
