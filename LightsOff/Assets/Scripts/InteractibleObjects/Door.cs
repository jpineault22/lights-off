using UnityEngine;

public class Door : InteractibleObject
{
	private Animator animator;

	private bool isOpen;

	protected override void Awake()
	{
		base.Awake();

		animator = GetComponent<Animator>();
	}

	public void OpenDoor()
	{
		isOpen = true;
		animator.SetBool(Constants.AnimatorDoorIsOpen, true);
	}

	public void CloseDoor()
	{
		isOpen = false;
		animator.SetBool(Constants.AnimatorDoorIsOpen, false);
		EnableDisableInteractMessage(false);
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (isOpen && pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(true);
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
