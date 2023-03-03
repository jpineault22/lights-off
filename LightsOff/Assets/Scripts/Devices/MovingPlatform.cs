using UnityEngine;

public class MovingPlatform : Device
{
	protected Rigidbody2D rb;

	[SerializeField] protected GameObject[] targetPoints = default;
	[SerializeField] protected float speed = 5f;
	[SerializeField] protected BoxCollider2D colliderForEnemies = default;

    protected int currentTargetIndex;

	protected override void Awake()
	{
		base.Awake();
		
		rb = GetComponent<Rigidbody2D>();

		currentTargetIndex = 0;
	}

	protected virtual void FixedUpdate()
	{
		if (spawnedEnemy)
		{
			colliderForEnemies.isTrigger = spawnedEnemy.transform.position.y - spawnedEnemyBoundsExtents.y + 0.2f < transform.position.y + spriteRenderer.bounds.extents.y;
		}
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

	private void SetPlayerAsChild(GameObject pCollision)
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing && pCollision.CompareTag(Constants.TagPlayer) && PlayerController.Instance.IsGrounded())
		{
			pCollision.transform.SetParent(transform);
		}
	}

	private void RemovePlayerFromChildren(GameObject pCollision)
	{
		if (pCollision.CompareTag(Constants.TagPlayer))
		{
			pCollision.transform.SetParent(null);
			DontDestroyOnLoad(pCollision);
		}
	}

	private void OnCollisionEnter2D(Collision2D pCollision)
	{
		SetPlayerAsChild(pCollision.gameObject);
	}

	private void OnCollisionStay2D(Collision2D pCollision)
	{
		SetPlayerAsChild(pCollision.gameObject);
	}

	private void OnCollisionExit2D(Collision2D pCollision)
	{
		RemovePlayerFromChildren(pCollision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D pCollider)
	{
		RemovePlayerFromChildren(pCollider.gameObject);
	}
}
