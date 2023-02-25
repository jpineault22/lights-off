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
			IncrementTargetIndex();
			StopPlatform();
			return true;
		}

		return false;
	}

	protected void IncrementTargetIndex()
	{
		currentTargetIndex++;

		if (currentTargetIndex >= targetPoints.Length)
		{
			currentTargetIndex = 0;
		}
	}

	private void SetPlayerAsChild(Collision2D pCollision)
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing && pCollision.gameObject.CompareTag(Constants.TagPlayer) && PlayerController.Instance.IsGrounded())
		{
			pCollision.gameObject.transform.SetParent(transform);
		}
	}

	private void OnCollisionEnter2D(Collision2D pCollision)
	{
		SetPlayerAsChild(pCollision);
	}

	private void OnCollisionStay2D(Collision2D pCollision)
	{
		SetPlayerAsChild(pCollision);
	}

	private void OnCollisionExit2D(Collision2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			pCollision.gameObject.transform.SetParent(null);
			DontDestroyOnLoad(pCollision.gameObject);
		}
	}
}
