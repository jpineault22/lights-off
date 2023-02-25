using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public CharacterState CurrentCharacterState { get; private set; }

    // Components
    [HideInInspector] public BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public AkAudioListener PlayerAkAudioListener { get; private set; }

    // Game objects assignable in inspector
    [Header("Game objects/Layer masks")]
    [SerializeField] private Transform ceilingCheckLeft = default;
    [SerializeField] private Transform ceilingCheckRight = default;
    [SerializeField] private LayerMask groundLayerMask = default;

    // References to the interactible game object and ladders in range
    private GameObject interactibleGameObject;
    private List<PivotingGate> pivotingGateList;
    private List<GameObject> ladderList;                                        // List of ladders whose trigger collider the player is currently touching. It should technically not have more than two elements,
    private GameObject functionalFan;                                           // those being ladders directly one above the other.

    // Modifiable in inspector
    [Header("Movement settings")]
    [SerializeField] private float moveInputThreshold = 0.2f;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float climbingSpeed = 5f;
    [SerializeField] private float climbCooldownTime = 0.25f;
    [SerializeField] private float gravityScale = 10f;
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float jumpTimeFanArea = 0.02f;
    [SerializeField] private float coyoteTime = 0.05f;                          // The amount of time the player can jump after starting to walk off a platform
    [SerializeField] private float pivotingGateTime = 0.2f;                     // The amount of time the player cannot jump after having left a collision with a rotating pivoting gate
    [SerializeField] private float bunkRadius = 0.1f;
    [SerializeField] private float groundedRadius = 0.2f;
    [SerializeField] private float minVerticalVelocity = -35f;                  // Minimum vertical velocity that the player can reach when falling
    [SerializeField] private float incompleteJumpFallVelocityDivider = 2f;      // When cancelling jump, divide current vertical velocity by this number
    [SerializeField] private float bounceMinFallingVerticalDistance = 4f;       // The minimum vertical distance the player has to be from the enemy when starting to fall in order to trigger a bounce and not die
    [SerializeField] private float bounceBuffer = 1f;                           // When bouncing, the player reaches their last falling start height + this buffer (except when it's a second bounce)
    [SerializeField] private float minBounceForce = 10f;
    [SerializeField] private float conveyorSpeedMultiplier = 3f;
    [SerializeField] private float fanSpeedMultiplier = 2f;

    // State variables
    private float horizontalMoveInput;
    private float verticalMoveInput;
    private bool facingRight;

    private bool isGrounded;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;

    private float pivotingGateTimeCounter;
    private bool inFanArea;

    private float climbCooldownCounter;
    private bool readyToLeaveClimbingState;                                     // This variable is set to true once the player has started climbing and has moved passed the corresponding ladder's end if applicable.It is reset when climbing ends.
    private bool reachedLadderTop;
    private float highestLadderEnd;
    private float lowestLadderEnd;

    private float lastFallingStartHeight;                                       // Stores the height at which the player last started falling. Used by ProcessBounce() to reach just above that same height, and by the collision detection, to determine if the player is high enough to trigger a bounce.
    private bool justBounced;

    private float momentumBonus;
    private Vector2 velocityBeforePhysicsUpdate;

    private int numberOfKeys;


    #region MonoBehaviour methods

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        // Getting components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PlayerAkAudioListener = GetComponent<AkAudioListener>();

        pivotingGateList = new List<PivotingGate>();
        ladderList = new List<GameObject>();

        facingRight = true;
        numberOfKeys = 0;
        momentumBonus = 1f;
    }

	protected override void OnDestroy()
	{
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerClimb, gameObject);

        base.OnDestroy();
	}

	private void OnEnable()
	{
        // Setting up controls methods
        InputManager.Instance.MovedHorizontally += ctx => UpdateHorizontalMoveInput(ctx);
        InputManager.Instance.MovedVertically += ctx => UpdateVerticalMoveInput(ctx);
        InputManager.Instance.StartedJump += ctx => StartJump(ctx);
        InputManager.Instance.EndedJump += ctx => EndJump(ctx);
        InputManager.Instance.StartedInteraction += ctx => StartInteraction(ctx);
    }

	private void OnDisable()
	{
        InputManager.Instance.MovedHorizontally -= ctx => UpdateHorizontalMoveInput(ctx);
        InputManager.Instance.MovedVertically -= ctx => UpdateVerticalMoveInput(ctx);
        InputManager.Instance.StartedJump -= ctx => StartJump(ctx);
        InputManager.Instance.EndedJump -= ctx => EndJump(ctx);
        InputManager.Instance.StartedInteraction -= ctx => StartInteraction(ctx);
    }

	private void Update()
    {
        if (CanPerformGameplayAction())
		{
            CheckFlipX();
            CheckIfNearbyPivotingGateRotating();

            if (coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
            if (pivotingGateTimeCounter > 0)
			{
                pivotingGateTimeCounter -= Time.deltaTime;
			}

            if (CurrentCharacterState != CharacterState.Falling && CurrentCharacterState != CharacterState.Bouncing)
            {
                justBounced = false;
            }

            if (CurrentCharacterState == CharacterState.Climbing)
            {
                animator.SetBool(Constants.AnimatorCharacterIsJumping, false);
                animator.SetBool(Constants.AnimatorCharacterIsClimbing, true);
            }
        }
        else if (GameManager.Instance.CurrentGameState != GameState.Reloading && animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.AnimationCharacterDeath) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > animator.GetCurrentAnimatorStateInfo(0).length)
		{
            GameManager.Instance.ReloadLevel();
        }
    }

    private void FixedUpdate()
    {
        if (CanPerformGameplayAction())
		{
            velocityBeforePhysicsUpdate = rb.velocity;

            SetIsGrounded();
            CheckIfFalling();
            ManageMomentumBonus();
            Move();

            if (CurrentCharacterState == CharacterState.Jumping)
            {
                ProcessJump();
            }
            else if (CurrentCharacterState == CharacterState.Bouncing)
            {
                ProcessBounce();
            }

            // Minimum cap for Y velocity when falling
            if (rb.velocity.y < minVerticalVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, minVerticalVelocity);
            }
        }
    }

    #endregion

    #region Input system methods

    private void UpdateHorizontalMoveInput(InputAction.CallbackContext ctx)
	{
        if (this != null) horizontalMoveInput = CanPerformGameplayAction() ? ctx.ReadValue<float>() : 0;
    }

    private void UpdateVerticalMoveInput(InputAction.CallbackContext ctx)
	{
        if (this != null) verticalMoveInput = CanPerformGameplayAction() ? ctx.ReadValue<float>() : 0;
    }

    private void StartJump(InputAction.CallbackContext ctx)
    {
        if (this != null && CanPerformGameplayAction() && (isGrounded || coyoteTimeCounter > 0 || CurrentCharacterState == CharacterState.Climbing) && pivotingGateTimeCounter <= 0)
        {
            if (CurrentCharacterState == CharacterState.Climbing)
                LeaveClimbingState();
            else if (inFanArea)
                SetFanMomentum();

            CurrentCharacterState = CharacterState.Jumping;
            animator.SetBool(Constants.AnimatorCharacterIsJumping, true);
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerClimb, gameObject);
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerJump, gameObject);
            jumpTimeCounter = inFanArea ? jumpTimeFanArea : jumpTime;
        }
    }

    private void EndJump(InputAction.CallbackContext ctx)
    {
        if (this != null && CanPerformGameplayAction() && CurrentCharacterState == CharacterState.Jumping)
        {
            SetStateToFalling();
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / incompleteJumpFallVelocityDivider);
        }
    }

    private void StartInteraction(InputAction.CallbackContext ctx)
    {
        if (this != null && CanPerformGameplayAction() && interactibleGameObject != null && !CurrentCharacterState.Equals(CharacterState.Jumping) && !CurrentCharacterState.Equals(CharacterState.Falling))
        {
            InteractibleObject interactibleObject = interactibleGameObject.GetComponent<InteractibleObject>();
            interactibleObject.Interact();
        }
    }

    #endregion

    #region Action processing methods, called from FixedUpdate

    private void Move()
	{
		bool climbingInValidDirection = false;          // Variable used to evaluate validity of the direction input when starting to climb, in order to allow it
		float playerFeetGroundedBuffer = transform.position.y - boxCollider.bounds.extents.y - groundedRadius;
		reachedLadderTop = playerFeetGroundedBuffer > highestLadderEnd - groundedRadius - spriteRenderer.bounds.extents.y && !boxCollider.IsTouchingLayers(groundLayerMask);

        climbingInValidDirection = SetUpLadders(climbingInValidDirection, playerFeetGroundedBuffer);
        
		if (ladderList.Count > 0 && climbCooldownCounter <= 0 && (CurrentCharacterState == CharacterState.Climbing || (Mathf.Abs(verticalMoveInput) > Mathf.Abs(horizontalMoveInput) && climbingInValidDirection)))
		{
            ProcessClimbing(playerFeetGroundedBuffer);
		}
		else
		{
			// Set state to falling if player was climbing
			if (CurrentCharacterState == CharacterState.Climbing)
			{
				LeaveClimbingState();
				SetStateToFalling();
			}

			if (climbCooldownCounter > 0)
			{
				climbCooldownCounter -= Time.fixedDeltaTime;
			}

			ProcessHorizontalMovement();
		}
	}

	private bool SetUpLadders(bool climbingInValidDirection, float playerFeetGroundedBuffer)
	{
		if (ladderList.Count > 0 && CurrentCharacterState != CharacterState.Climbing)
		{
            // Determine if player is climbing in a valid direction
            if (Mathf.Abs(verticalMoveInput) >= moveInputThreshold &&
                ((verticalMoveInput > 0 && playerFeetGroundedBuffer < highestLadderEnd - groundedRadius) || (verticalMoveInput < 0 && playerFeetGroundedBuffer > lowestLadderEnd + groundedRadius)))
            {
                if (!reachedLadderTop)
                {
                    climbingInValidDirection = true;
                }
            }
        }

		return climbingInValidDirection;
	}

	private void ProcessHorizontalMovement()
	{
        if (Mathf.Abs(horizontalMoveInput) >= moveInputThreshold || momentumBonus > 1f)
        {
            int direction = facingRight ? 1 : -1;
            rb.velocity = new Vector2(direction * speed * momentumBonus, rb.velocity.y);

            if (isGrounded && CurrentCharacterState != CharacterState.Jumping && CurrentCharacterState != CharacterState.Bouncing)
            {
                if (CurrentCharacterState != CharacterState.Walking)
                    AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerWalk, gameObject);

                CurrentCharacterState = CharacterState.Walking;
            }
        }
        else
        {
            StopHorizontalMovement();
        }

        animator.SetFloat(Constants.AnimatorCharacterSpeed, Mathf.Abs(horizontalMoveInput));
    }

    private void ProcessClimbing(float pPlayerFeetGroundedBuffer)
    {
        if (CurrentCharacterState != CharacterState.Climbing)
        {
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerClimb, gameObject);
        }

        CurrentCharacterState = CharacterState.Climbing;
        animator.SetBool(Constants.AnimatorCharacterIsJumping, false);
        animator.SetBool(Constants.AnimatorCharacterIsClimbing, true);
        rb.gravityScale = 0;

        // Check if reached ladder's bottom
        if (pPlayerFeetGroundedBuffer > lowestLadderEnd + groundedRadius)
        {
            readyToLeaveClimbingState = true;
        }
        else if (readyToLeaveClimbingState)
        {
            LeaveClimbingState();
            SetStateToFalling();
        }

        if (Mathf.Abs(verticalMoveInput) >= moveInputThreshold && (!reachedLadderTop || verticalMoveInput < 0))
        {
            if (!animator.enabled)
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerClimb, gameObject);

            int direction = verticalMoveInput > 0 ? 1 : -1;
            rb.velocity = new Vector2(0, direction * climbingSpeed);
            animator.enabled = true;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.AnimationCharacterClimb))
            {
                animator.enabled = false;
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerClimb, gameObject);
            }
        }

        // Snap player to horizontal middle of ladder
        transform.position = new Vector2(ladderList[0].transform.position.x, transform.position.y);
    }

    private void ProcessJump()
    {
        if (CheckIfCeilingBunk())
        {
            return;
        }
        else if (jumpTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            SetStateToFalling();
        }
    }

    private void ProcessBounce()
	{
        if (CheckIfCeilingBunk())
		{
            return;
		}
        else if (transform.position.y < lastFallingStartHeight + bounceBuffer)
		{
            if (rb.velocity.y < minBounceForce)
			{
                rb.velocity = new Vector2(rb.velocity.x, minBounceForce);
			}
		}
        else
		{
            //rb.velocity = new Vector2(rb.velocity.x, 0);

            if (rb.velocity.y > minBounceForce)
			{
                rb.velocity = new Vector2(rb.velocity.x, minBounceForce);
			}

            SetStateToFalling();
		}
	}

    #endregion

    #region Collisions

    private void OnCollisionEnter2D(Collision2D pCollision)
    {
        if (pCollision.gameObject.CompareTag(Constants.TagKey))
        {
            CollectKey(pCollision.gameObject);
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagKeygate) && numberOfKeys > 0)
        {
            numberOfKeys--;
            UIManager.Instance.UpdateKeyNumberText(numberOfKeys);
            pCollision.gameObject.GetComponent<GateTypeA>().SwitchOnOff();
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagPivotingGate))
        {
            pivotingGateList.Add(pCollision.gameObject.GetComponent<PivotingGate>());
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagEnemy))
		{
            Die();
		}
    }

	private void OnCollisionExit2D(Collision2D pCollision)
	{
        if (pCollision.gameObject.CompareTag(Constants.TagPivotingGate))
        {
            pivotingGateList.Remove(pCollision.gameObject.GetComponent<PivotingGate>());
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagConveyor) && transform.position.y > pCollision.gameObject.transform.position.y)
        {
            CheckIfFalling();

            if (CurrentCharacterState == CharacterState.Jumping || CurrentCharacterState == CharacterState.Falling)
			{
                Conveyor conveyor = pCollision.gameObject.GetComponent<Conveyor>();
                bool sameDirection = (facingRight && conveyor.IsDirectionRight()) || (!facingRight && !conveyor.IsDirectionRight());

                if (conveyor.IsOnAndConnected() && sameDirection)
				{
                    float momentumDivider = Mathf.Abs(horizontalMoveInput) > moveInputThreshold ? 1f : 2f;
                    momentumBonus = conveyorSpeedMultiplier / momentumDivider;
                }
			}
        }
    }

	private void OnTriggerEnter2D(Collider2D pCollision)
    {
        if (pCollision.gameObject.layer == LayerMask.NameToLayer(Constants.LayerInteractibleObject))
        {
            interactibleGameObject = pCollision.gameObject;
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagLadder))
        {
            ladderList.Add(pCollision.gameObject);
            UpdateLadderEnds();
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagKey))
		{
            CollectKey(pCollision.gameObject);
		}
        else if (pCollision.gameObject.CompareTag(Constants.TagFunctionalFan))
		{
            inFanArea = true;
            functionalFan = pCollision.gameObject;
		}
        else if (pCollision.gameObject.CompareTag(Constants.TagEnemy))
        {
            if (CurrentCharacterState == CharacterState.Falling && lastFallingStartHeight - pCollision.gameObject.transform.position.y >= bounceMinFallingVerticalDistance)
            {
                pCollision.gameObject.GetComponent<Enemy>().GetStunned();
                CurrentCharacterState = CharacterState.Bouncing;
                animator.SetTrigger(Constants.AnimatorCharacterIsJumping);
                rb.velocity = new Vector2(velocityBeforePhysicsUpdate.x, -velocityBeforePhysicsUpdate.y);
                justBounced = true;
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerBounce, gameObject);
            }
        }
    }

	private void OnTriggerExit2D(Collider2D pCollision)
    {
        if (pCollision.gameObject.layer == LayerMask.NameToLayer(Constants.LayerInteractibleObject))
        {
            interactibleGameObject = null;
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagLadder))
        {
            ladderList.Remove(pCollision.gameObject);
            UpdateLadderEnds();
        }
        else if (pCollision.gameObject.CompareTag(Constants.TagFunctionalFan))
        {
            inFanArea = false;
            functionalFan = null;
        }
    }

    #endregion

    #region State update methods

    private void SetIsGrounded()
    {
        bool previouslyNotGrounded = !isGrounded;
        
        Vector3 originLeft = new Vector3(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);
        Vector3 originRight = new Vector3(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);
        RaycastHit2D raycastLeft = Physics2D.Raycast(originLeft, Vector2.down, boxCollider.bounds.extents.y + groundedRadius, groundLayerMask);
        RaycastHit2D raycastRight = Physics2D.Raycast(originRight, Vector2.down, boxCollider.bounds.extents.y + groundedRadius, groundLayerMask);

        isGrounded = (raycastLeft && !raycastLeft.collider.isTrigger) || (raycastRight && !raycastRight.collider.isTrigger);

        if (previouslyNotGrounded && isGrounded)
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerLand, gameObject);
    }

    private void CheckFlipX()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Playing && CurrentCharacterState != CharacterState.Climbing && Mathf.Abs(horizontalMoveInput) >= moveInputThreshold)
        {
            if (horizontalMoveInput > 0)
            {
                if (!facingRight && momentumBonus > 1f) momentumBonus = 1f;

                spriteRenderer.flipX = false;
                facingRight = true;
            }
            else if (horizontalMoveInput < 0)
            {
                if (facingRight && momentumBonus > 1f) momentumBonus = 1f;

                spriteRenderer.flipX = true;
                facingRight = false;
            }
        }
    }

    private void CheckIfFalling()
    {
        if (!isGrounded && (CurrentCharacterState == CharacterState.Idle || CurrentCharacterState == CharacterState.Walking))
        {
            coyoteTimeCounter = coyoteTime;
            SetStateToFalling();
        }
        else if (CurrentCharacterState != CharacterState.Falling)
        {
            animator.SetBool(Constants.AnimatorCharacterIsFalling, false);
        }
    }

    private bool CheckIfCeilingBunk()
    {
        Collider2D ceilingLeft = Physics2D.OverlapCircle(ceilingCheckLeft.position, bunkRadius, groundLayerMask);
        Collider2D ceilingRight = Physics2D.OverlapCircle(ceilingCheckRight.position, bunkRadius, groundLayerMask);

        if ((ceilingLeft && !ceilingLeft.isTrigger) || (ceilingRight && !ceilingRight.isTrigger))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            SetStateToFalling();
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerBunk, gameObject);
            return true;
        }

        return false;
    }

    private void CheckIfNearbyPivotingGateRotating()
	{
        foreach (PivotingGate pivotingGate in pivotingGateList)
		{
            if (pivotingGate.IsRotating())
			{
                pivotingGateTimeCounter = pivotingGateTime;
			}
		}
	}

    private bool CanPerformGameplayAction()
	{
        return GameManager.Instance.CurrentGameState == GameState.Playing && CurrentCharacterState != CharacterState.Dying && CurrentCharacterState != CharacterState.LevelTransition;
	}

    public bool CanAccessInteractibleObject()
	{
        return CanPerformGameplayAction() && CurrentCharacterState != CharacterState.Jumping && CurrentCharacterState != CharacterState.Falling &&
            CurrentCharacterState != CharacterState.Bouncing && CurrentCharacterState != CharacterState.Climbing;

    }

    private void UpdateLadderEnds()
	{
        highestLadderEnd = -1000f;
        lowestLadderEnd = 1000f;

        foreach (GameObject ladder in ladderList)
        {
            foreach (GameObject ladderEnd in GameObjectUtils.GetChildren(ladder))
            {
                if (ladderEnd.transform.position.y > highestLadderEnd)
                {
                    highestLadderEnd = ladderEnd.transform.position.y;
                }
                else if (ladder.transform.position.y < lowestLadderEnd)
                {
                    lowestLadderEnd = ladderEnd.transform.position.y;
                }
            }
        }
    }

    private void SetFanMomentum()
	{
        Fan fan = functionalFan.GetComponentInParent<Fan>();
        bool sameDirection = (facingRight && fan.Orientation == ObjectOrientation.East) || (!facingRight && fan.Orientation == ObjectOrientation.West);

        if (sameDirection)
        {
            float momentumDivider = Mathf.Abs(horizontalMoveInput) > moveInputThreshold ? 1f : 2f;
            momentumBonus = fanSpeedMultiplier / momentumDivider;
        }
    }

    private void ManageMomentumBonus()
	{
        if (momentumBonus > 1f)
		{
            if (CurrentCharacterState == CharacterState.Idle || CurrentCharacterState == CharacterState.Walking || CurrentCharacterState == CharacterState.Climbing || CurrentCharacterState == CharacterState.Dying)
			{
                momentumBonus = 1f;
			}
		}
	}

    private void StopHorizontalMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (isGrounded && CurrentCharacterState != CharacterState.Jumping)
        {
            if (CurrentCharacterState == CharacterState.Walking)
                AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);

            CurrentCharacterState = CharacterState.Idle;
        }
    }

    private void SetStateToFalling()
	{
        if (CurrentCharacterState == CharacterState.Walking)
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);

        CurrentCharacterState = CharacterState.Falling;
        animator.SetBool(Constants.AnimatorCharacterIsJumping, false);
        animator.SetBool(Constants.AnimatorCharacterIsFalling, true);
        lastFallingStartHeight = justBounced ? transform.position.y - bounceBuffer : transform.position.y;
    }

    private void LeaveClimbingState()
	{
        animator.enabled = true;
        animator.SetBool(Constants.AnimatorCharacterIsClimbing, false);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerClimb, gameObject);
        rb.gravityScale = gravityScale;
        climbCooldownCounter = climbCooldownTime;
        readyToLeaveClimbingState = false;
    }

    // This method is called when the player dies (touches enemy). It is soon followed by ResetCharacterForReload, called by GameManager once the death animation has finished,
    // and then by ResetCharacterAfterDeath, called by GameManager once the first part of the crossfade transition has ended.
    private void Die()
    {
        // Reset character states and set state and animation to Dying. Once the animation ends, the level will be reloaded.
        CurrentCharacterState = CharacterState.Dying;
        animator.enabled = true;
        animator.SetBool(Constants.AnimatorCharacterIsDying, true);
        animator.SetBool(Constants.AnimatorCharacterIsFalling, false);
        animator.SetBool(Constants.AnimatorCharacterIsJumping, false);
        animator.SetBool(Constants.AnimatorCharacterIsClimbing, false);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        boxCollider.enabled = false;
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerDeath, gameObject);
    }

    // Called by GameManager at Level Transition, Death, or Level Retry
    public void ResetCharacterForLevelTransition(float pDoorHorizontalPosition)
	{
        transform.position = new Vector2(pDoorHorizontalPosition, transform.position.y);
        ResetCharacterForReload();
        animator.SetBool(Constants.AnimatorCharacterIsExitingLevel, true);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventStopPlayerWalk, gameObject);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerExitLevel, gameObject);
    }

    public void SetCharacterAnimationToEnterLevel()
	{
        spriteRenderer.enabled = true;
        animator.SetBool(Constants.AnimatorCharacterIsExitingLevel, false);
        animator.SetBool(Constants.AnimatorCharacterIsEnteringLevel, true);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayPlayerEnterLevel, gameObject);
    }

    public void ResetCharacterForReload()
	{
        CurrentCharacterState = CharacterState.LevelTransition;
        animator.SetFloat(Constants.AnimatorCharacterSpeed, 0);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        boxCollider.enabled = false;
        ladderList.Clear();
        numberOfKeys = 0;
	}

    public void ResetCharacterAfterReload()
	{
        animator.SetBool(Constants.AnimatorCharacterIsDying, false);
        animator.SetBool(Constants.AnimatorCharacterIsJumping, false);
        animator.SetBool(Constants.AnimatorCharacterIsFalling, false);
        animator.SetBool(Constants.AnimatorCharacterIsClimbing, false);
        animator.enabled = true;
        spriteRenderer.flipX = false;
        facingRight = true;
        boxCollider.enabled = true;
        rb.gravityScale = gravityScale;
    }

    public void ResetCharacterForQuitToMenu()
	{
        ResetState();
        rb.gravityScale = 0;
	}

    public void ResetState()
	{
        CurrentCharacterState = CharacterState.Idle;
        animator.SetFloat(Constants.AnimatorCharacterSpeed, 0);
        animator.SetBool(Constants.AnimatorCharacterIsExitingLevel, false);
        animator.SetBool(Constants.AnimatorCharacterIsEnteringLevel, false);
        boxCollider.enabled = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityScale;
    }

    private void CollectKey(GameObject pKey)
	{
        numberOfKeys++;
        UIManager.Instance.UpdateKeyNumberText(numberOfKeys);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayKeyCollect, gameObject);
        Destroy(pKey);
    }

    #endregion

    public bool IsGrounded()
	{
        return isGrounded;
	}
}

public enum CharacterState
{
    Idle,
    Walking,
    Jumping,
    Falling,
    Climbing,
    Bouncing,
    Dying,
    LevelTransition
}