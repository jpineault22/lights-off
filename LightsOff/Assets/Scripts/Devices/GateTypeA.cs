using UnityEngine;

public class GateTypeA : DisappearingDevice
{
    private Animator animator;

	protected override void Awake()
	{
		animator = GetComponent<Animator>();

		base.Awake();
	}

	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.AnimationKeygateOpen) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > animator.GetCurrentAnimatorStateInfo(0).length)
		{
			deviceCollider.enabled = false;
		}
	}

	public override void ApplyOnOffBehavior()
	{
		if (IsOnAndConnected())
		{
			animator.SetBool(Constants.AnimatorGateIsOpen, false);
			deviceCollider.enabled = true;
		}
		else
		{
			animator.SetBool(Constants.AnimatorGateIsOpen, true);

			if (!gameObject.CompareTag(Constants.TagKeygate))
				deviceCollider.enabled = false;
		}
	}
}
