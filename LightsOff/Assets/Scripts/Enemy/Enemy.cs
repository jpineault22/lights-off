using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyState defaultEnemyState = default;
    [SerializeField] protected Transform groundDetection = default;
    [SerializeField] protected LayerMask groundLayerMask = default;
    [SerializeField] protected float stunnedTime = 2f;
    [SerializeField] protected float groundDetectionRaycastDistance = 1f;           // The distance of the raycast detecting the ground
    [SerializeField] protected float wallDetectionRaycastDistance = 0.25f;          // The distance of the raycast detecting walls
    [SerializeField] protected float groundedRadius = 0.2f;
    [SerializeField] protected float distancePlayerAbove = 1.8f;                    // The distance the player has to be above the enemy for its head collider to be active

    protected Rigidbody2D rb;
    protected CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    
    protected EnemyState currentEnemyState;
    protected EnemyState previousEnemyState;
    protected bool facingRight;
    protected bool isGrounded;
    protected float currentStateTimer = 0f;
    protected float? horizontalPositionToMaintain;

    protected GameObject player;

    protected virtual void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentEnemyState = defaultEnemyState;

        player = PlayerController.Instance.gameObject;

        AssignAudioEmitterToPlayerListener();
    }

    protected virtual void OnEnable()
	{
        GameManager.Instance.PlayerSpawned += AssignAudioEmitterToPlayerListener;
	}

    protected virtual void OnDisable()
	{
        GameManager.Instance.PlayerSpawned -= AssignAudioEmitterToPlayerListener;
    }

	protected virtual void Update()
	{
		if (horizontalPositionToMaintain != null)
		{
            transform.position = new Vector2((float) horizontalPositionToMaintain, transform.position.y);

            if (currentEnemyState != EnemyState.Stunned)
                horizontalPositionToMaintain = null;
		}
	}

	protected virtual void FixedUpdate()
	{
        SetIsGrounded();
        CheckIfFalling();
        
        if (currentEnemyState == EnemyState.Stunned || currentEnemyState == EnemyState.ChasingIdle || currentEnemyState == EnemyState.WakingUp)
		{
            if (currentStateTimer >= 0)
			{
                currentStateTimer -= Time.fixedDeltaTime;
                animator.SetFloat(Constants.AnimatorEnemyCurrentStateTimer, currentStateTimer);
            }
            else
			{
                if (currentEnemyState == EnemyState.WakingUp)
                {
                    currentEnemyState = EnemyState.Chasing;
                    AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayEnemyStartChasing, gameObject);
                }
                else if (previousEnemyState != EnemyState.Stunned)
				{
                    currentEnemyState = previousEnemyState;
                }
                else
				{
                    ResetToDefaultEnemyState();
				}
			}
		}
	}

    protected void Flip()
    {
        if (facingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            facingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            facingRight = true;
        }
    }

    private void SetIsGrounded()
	{
        Vector3 originLeft = new Vector3(circleCollider.bounds.center.x - circleCollider.bounds.extents.x, circleCollider.bounds.center.y, circleCollider.bounds.center.z);
        Vector3 originRight = new Vector3(circleCollider.bounds.center.x + circleCollider.bounds.extents.x, circleCollider.bounds.center.y, circleCollider.bounds.center.z);
        isGrounded = Physics2D.Raycast(originLeft, Vector2.down, circleCollider.bounds.extents.y + groundedRadius, groundLayerMask)
            || Physics2D.Raycast(originRight, Vector2.down, circleCollider.bounds.extents.y + groundedRadius, groundLayerMask);
    }

    private void CheckIfFalling()
	{
        if (!isGrounded && currentEnemyState != EnemyState.Falling)
		{
            previousEnemyState = currentEnemyState;
            currentEnemyState = EnemyState.Falling;
		}
        else if (isGrounded && currentEnemyState == EnemyState.Falling)
		{
            currentEnemyState = previousEnemyState;
            AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayEnemyLand, gameObject);
		}
	}

    // Called by PlayerController when OnTriggerEnter2D is triggered by the Enemy's head collider
    public void GetStunned()
	{
        if (currentEnemyState != EnemyState.Stunned)
		{
            previousEnemyState = currentEnemyState;
            horizontalPositionToMaintain = transform.position.x;
        }

        currentStateTimer = stunnedTime;
        currentEnemyState = EnemyState.Stunned;
        animator.SetTrigger(Constants.AnimatorEnemyBouncedOn);
        animator.SetFloat(Constants.AnimatorEnemyCurrentStateTimer, currentStateTimer);
        AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayEnemyStunned, gameObject);

        if (GameObjectUtils.AnimatorHasParameter(animator, Constants.AnimatorEnemyIsChasing))
            animator.SetBool(Constants.AnimatorEnemyIsChasing, false);
    }

    protected void ResetToDefaultEnemyState()
	{
        currentEnemyState = defaultEnemyState;
    }

    private void AssignAudioEmitterToPlayerListener()
	{
        AudioManager.Instance.AssignEmitterToPlayerListener(gameObject);
    }
}

public enum EnemyState
{
    PassedOut,
    Wandering,
    WakingUp,
    Chasing,
    ChasingIdle,        // When the enemy is chasing the player and is unable to keep chasing them (ex: the player goes up a ladder), they become ChasingIdle
    Stunned,
    Falling
}