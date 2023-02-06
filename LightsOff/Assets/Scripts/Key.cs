using UnityEngine;

public class Key : MonoBehaviour
{
	[SerializeField] private bool carriedUpByFan;			// This variable is used to check whether the key's collider has to be set to trigger when carried upwards by a fan, in order to avoid colliding with One-way Platforms.
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (carriedUpByFan)
			boxCollider.isTrigger = rb.velocity.y > 0;
	}

	private void OnCollisionEnter2D(Collision2D pCollision)
	{
		if (pCollision.collider.gameObject.layer == LayerMask.NameToLayer(Constants.LayerGround))
		{
			AudioManager.Instance.TriggerWwiseEvent(Constants.WwiseEventPlayKeyLands, gameObject);
		}
	}
}
