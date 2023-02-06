using UnityEngine;

public class EnemyDrunk : Enemy
{
	[SerializeField] private float wanderingSpeed = 3f;

	private bool startedWandering;

	protected override void FixedUpdate()
	{
		if (GameManager.Instance.CurrentGameState == GameState.Playing)
		{
			base.FixedUpdate();

			Wander();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void OnDestroy()
	{
		AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopEnemyWander, gameObject);
	}

	private void Wander()
	{
		if (currentEnemyState == EnemyState.Wandering)
		{
			int direction = facingRight ? 1 : -1;

			RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerGround));
			RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, new Vector2(direction, 0), wallDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerGround));

			if (currentEnemyState != EnemyState.Falling && currentEnemyState != EnemyState.Stunned && (!groundInfo.collider || wallInfo.collider))
			{
				Flip();
			}

			rb.velocity = new Vector2(direction * wanderingSpeed, rb.velocity.y);

			if (!startedWandering)
			{
				startedWandering = true;
				AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayEnemyWander, gameObject);
			}
		}
		else
		{
			startedWandering = false;
		}
	}

	// This mechanic is not used anymore in game. The enemies would have to be redesigned for it to be functional.
	private void PassOut()
	{
		// Instantiate EnemyPassedOut at same position and destroy self
		Spawner.Instance.SpawnNewEnemyPassedOut(transform.position, transform.rotation);
		Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D pCollision)
	{
		if (pCollision.collider.gameObject.CompareTag(Constants.TagEnemy))
		{
			PassOut();
		}
	}
}
