using UnityEngine;

public class Key : MonoBehaviour
{
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		boxCollider.isTrigger = rb.velocity.y > 0;
	}
}
