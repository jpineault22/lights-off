using UnityEngine;

public class OneWayPlatform : RotatableObject
{
	[SerializeField] private float platformWidthHalf = 0.25f;
	[SerializeField] private DisappearingDevice disappearingDevice = default;

	private EdgeCollider2D edgeCollider;

	protected override void Awake()
	{
		base.Awake();
		
		edgeCollider = GetComponent<EdgeCollider2D>();
	}

	private void Update()
	{
		if (PlayerController.IsInitialized && (disappearingDevice == default || disappearingDevice.IsOnAndConnected()))
		{
			if (objectRotation == ObjectOrientation.West)
			{
				if (PlayerController.Instance.transform.position.x + PlayerController.Instance.boxCollider.bounds.extents.x > transform.position.x - platformWidthHalf)
				{
					edgeCollider.isTrigger = true;
				}
				else if (PlayerController.Instance.transform.position.x + PlayerController.Instance.boxCollider.bounds.extents.x < transform.position.x - platformWidthHalf)
				{
					edgeCollider.isTrigger = false;
				}
			}
			else if (objectRotation == ObjectOrientation.East)
			{
				if (PlayerController.Instance.transform.position.x - PlayerController.Instance.boxCollider.bounds.extents.x < transform.position.x + platformWidthHalf)
				{
					edgeCollider.isTrigger = true;
				}
				else if (PlayerController.Instance.transform.position.x - PlayerController.Instance.boxCollider.bounds.extents.x > transform.position.x + platformWidthHalf)
				{
					edgeCollider.isTrigger = false;
				}
			}
			else if (objectRotation == ObjectOrientation.South)
			{
				if (PlayerController.Instance.transform.position.y + PlayerController.Instance.boxCollider.bounds.extents.y > transform.position.y - platformWidthHalf)
				{
					edgeCollider.isTrigger = true;
				}
				else if (PlayerController.Instance.transform.position.y + PlayerController.Instance.boxCollider.bounds.extents.y < transform.position.y - platformWidthHalf)
				{
					edgeCollider.isTrigger = false;
				}
			}
			else
			{
				// Activate/deactivate collider depending on Player's height or if the Player is climbing
				if (PlayerController.Instance.CurrentCharacterState == CharacterState.Climbing ||
					PlayerController.Instance.transform.position.y - PlayerController.Instance.boxCollider.bounds.extents.y < transform.position.y + platformWidthHalf)
				{
					edgeCollider.isTrigger = true;
				}
				else if (PlayerController.Instance.transform.position.y - PlayerController.Instance.boxCollider.bounds.extents.y > transform.position.y + platformWidthHalf)
				{
					edgeCollider.isTrigger = false;
				}
			}
		}
	}
}
