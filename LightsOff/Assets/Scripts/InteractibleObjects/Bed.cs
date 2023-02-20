using UnityEngine;

public class Bed : InteractibleObject
{
	private Animator animator;

	protected override void Awake()
	{
		base.Awake();

		animator = GetComponent<Animator>();
	}

	protected override void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer) && GameManager.Instance.CheckIfAllLightsOff())
		{
			EnableDisableInteractMessage(true);
		}
	}

	protected override void OnTriggerExit2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
		{
			EnableDisableInteractMessage(false);
		}
	}

	public override void Interact()
	{
		if (GameManager.Instance.CheckIfAllLightsOff())
		{
			InputManager.Instance.DisableGameplayMap();
			UIManager.Instance.EnableInGameUI(false);
			animator.enabled = true;
			StartCoroutine(TransitionManager.Instance.TriggerEndingCinematic(this));
		}
	}

	public void WakeUp()
	{
		animator.SetTrigger(Constants.AnimatorBedWakingUp);
	}
}
