using UnityEngine;

public class EnemyPassedOut : Enemy
{
	[SerializeField] private float chasingSpeed = 5f;
	[SerializeField] private float playerDetectionRaycastDistance = 2f;         // The distance of the "line of sight" raycast detecting the player
	[SerializeField] private float exitChasingDistance = 20f;                   // The distance the player needs to be from the enemy for it to stop chasing them
	[SerializeField] private float verticalExitChasingDistance = 4f;            // The vertical distance the player needs to be from the enemy for it to stop chasing them
	[SerializeField] private float chasingIdleTime = 2f;						// The time the enemy will be ChasingIdle
	[SerializeField] private float chasingFlipTimeMin = 1f;                     // The minimum time between each flip when chasing
	[SerializeField] private float ladderMaxWaitingDistance = 2f;               // The maximum distance from the ladder at which the enemy will stop and become ChasingIdle
	[SerializeField] private float ladderMinWaitingDistance = 1.8f;				// The minimum distance from the ladder at which the enemy will stop and become ChasingIdle
	[SerializeField] private float climbingVerticalExitChasingDistance = 2f;	// The minimum distance the player has to be from the enemy while climbing for it to stop chasing them

	private float chasingFlipTimer;
	private float? playerClimbingHorizontalPosition;                            // The player's horizontal position when they start climbing
	private bool crossingLadder;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// Check if player has just started climbing
		if (PlayerController.Instance.CurrentCharacterState == CharacterState.Climbing && playerClimbingHorizontalPosition == null)
		{
			playerClimbingHorizontalPosition = player.transform.position.x;
		}

		// Update chasing flip timer
		if (chasingFlipTimer > 0)
		{
			chasingFlipTimer -= Time.fixedDeltaTime;
		}

		// Process action for the corresponding enemy state
		if (currentEnemyState == EnemyState.PassedOut)
		{
			DetectPlayer();
		}
		else if (currentEnemyState == EnemyState.Chasing)
		{
			ChasePlayer();
		}
		else if (currentEnemyState == EnemyState.ChasingIdle && currentStateTimer <= 0)
		{
			playerClimbingHorizontalPosition = null;
			ResetToDefaultEnemyState();
		}
	}

	private void DetectPlayer()
	{
		int direction = facingRight ? 1 : -1;

		RaycastHit2D playerInfoFront = Physics2D.Raycast(groundDetection.position, new Vector2(direction, 0), playerDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerPlayer));
		RaycastHit2D playerInfoBack = Physics2D.Raycast(groundDetection.position, new Vector2(-direction, 0), spriteRenderer.bounds.size.x + playerDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerPlayer));

		if (playerInfoFront.collider || playerInfoBack.collider)
		{
			currentEnemyState = EnemyState.Chasing;
		}
	}

	private void ChasePlayer()
	{
		float playerVerticalDistance = player.transform.position.y - transform.position.y;
		bool playerWithinVerticalRange = Mathf.Abs(playerVerticalDistance) < verticalExitChasingDistance;
		int chasingDirection = CheckChasingDirection(playerWithinVerticalRange);

		RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, new Vector2(chasingDirection, 0), wallDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerGround));
		RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDetectionRaycastDistance, LayerMask.GetMask(Constants.LayerGround));

		bool hasCrossedLadder = false;

		// Using -0.5f as a buffer below since the player and enemy standing on the same ground will not have the same position.y, the enemy being 1.5x higher than the player
		bool playerLevelOrAbove = playerVerticalDistance >= -0.5f;
		bool playerClimbingOutOfReach = playerClimbingHorizontalPosition != null && Mathf.Abs(playerVerticalDistance) >= climbingVerticalExitChasingDistance &&
										Mathf.Abs(playerClimbingHorizontalPosition.Value - transform.position.x) <= ladderMaxWaitingDistance;

		// If the player climbs out of reach when the enemy has gone past the waiting range beside the ladder (between ladderMaxWaitingDistance and ladderMinWaitingDistance),
		// the enemy will start crossing the ladder and become ChasingIdle once it leaves the range on the opposite side of the ladder
		if (!crossingLadder && playerClimbingOutOfReach && Mathf.Abs(playerClimbingHorizontalPosition.Value - transform.position.x) < ladderMinWaitingDistance)
		{
			crossingLadder = true;
			return;
		}
		else if (crossingLadder && Mathf.Abs(playerClimbingHorizontalPosition.Value - transform.position.x) >= ladderMaxWaitingDistance)
		{
			crossingLadder = false;
			hasCrossedLadder = true;
		}

		// Switch the enemy's state to ChasingIdle IF the enemy is not currently crossing the ladder AND:
		// The enemy reaches a wall
		// OR the enemy reaches a pit and the player is at the same height or above the enemy
		// OR the enemy has finished crossing the ladder
		// OR the player is far enough from the enemy
		// OR the player is far enough above the enemy
		// OR the player has climbed up or down out of reach from the enemy
		if (!crossingLadder && (wallInfo.collider || (!groundInfo.collider && playerLevelOrAbove) || hasCrossedLadder || Vector2.Distance(transform.position, player.transform.position) > exitChasingDistance ||
			player.transform.position.y - transform.position.y > verticalExitChasingDistance || playerClimbingOutOfReach))
		{
			BecomeChasingIdle(chasingIdleTime);
		}
		else
		{
			rb.velocity = new Vector2(chasingDirection * chasingSpeed, rb.velocity.y);
		}
	}

	private int CheckChasingDirection(bool pPlayerWithinVerticalRange)
	{
		int chasingDirection = facingRight ? 1 : -1;

		if (!crossingLadder && chasingFlipTimer <= 0)
		{
			int newChasingDirection = transform.position.x <= player.transform.position.x ? 1 : -1;

			if (pPlayerWithinVerticalRange && ((newChasingDirection == 1 && !facingRight) || (newChasingDirection == -1 && facingRight)))
			{
				chasingDirection = newChasingDirection;
				Flip();
				chasingFlipTimer = chasingFlipTimeMin;
			}
		}

		return chasingDirection;
	}

	private void BecomeChasingIdle(float pChasingIdleTime)
	{
		currentEnemyState = EnemyState.ChasingIdle;
		currentStateTimer = pChasingIdleTime;
		rb.velocity = Vector2.zero;
	}
}
