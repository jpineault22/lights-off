using UnityEngine;

public class Door : InteractibleObject
{
	[SerializeField] private Sprite doorClosed = default;
	[SerializeField] private Sprite doorOpen = default;

	private SpriteRenderer spriteRenderer;

	private bool isOpen;

	protected override void Awake()
	{
		base.Awake();
		
		spriteRenderer = GetComponent<SpriteRenderer>();

		spriteRenderer.sprite = doorClosed;
	}

	public void OpenDoor()
	{
		isOpen = true;
		spriteRenderer.sprite = doorOpen;
	}

	public void CloseDoor()
	{
		isOpen = false;
		spriteRenderer.sprite = doorClosed;
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (isOpen && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			interactMessageText.enabled = true;
			InRange = true;
		}
	}

	public override void Interact()
	{
		if (isOpen && PlayerController.Instance.CurrentCharacterState != CharacterState.LevelTransition)
		{
			GameManager.Instance.LoadNextLevel();
		}
	}
}
