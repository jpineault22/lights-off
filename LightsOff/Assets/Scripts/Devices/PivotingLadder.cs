using UnityEngine;

public class PivotingLadder : MonoBehaviour
{
	[SerializeField] private bool startingSideways;

	private BoxCollider2D boxCollider;
	private Collider2D parentCollider;
	private GameObject ladderTop;
	private GameObject ladderBottom;

	private float defaultValidRotation;
	private float otherValidRotation;
	private float ladderTopHeight;
	private float ladderBottomHeight;
	private bool collidingWithPlayer;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		parentCollider = transform.parent.GetComponent<Collider2D>();

		foreach (GameObject child in GameObjectUtils.GetChildren(gameObject))
		{
			if (child.CompareTag(Constants.TagLadderTop))
			{
				ladderTop = child;
				ladderTopHeight = child.transform.localPosition.y;
			}
			else if (child.CompareTag(Constants.TagLadderBottom))
			{
				ladderBottom = child;
				ladderBottomHeight = child.transform.localPosition.y;
			}
		}
		
		float parentRotation = transform.parent.transform.localEulerAngles.z;

		switch (parentRotation)
		{
			case 0:
				defaultValidRotation = 0;
				otherValidRotation = 180;
				break;
			case 90:
				defaultValidRotation = 90;
				otherValidRotation = 270;
				break;
			case 180:
				defaultValidRotation = 180;
				otherValidRotation = 0;
				break;
			case 270:
				defaultValidRotation = 270;
				otherValidRotation = 90;
				break;
			default:
				defaultValidRotation = 0;
				otherValidRotation = 180;
				break;
		}

		if (startingSideways)
		{
			defaultValidRotation += 90;
			otherValidRotation += 90;

			while (defaultValidRotation >= 360)
				defaultValidRotation -= 360;
			while (otherValidRotation >= 360)
				otherValidRotation -= 360;
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.PlayerEnteredClimbingState += CheckToDisableParentCollider;
		GameManager.Instance.PlayerExitedClimbingState += EnableParentCollider;
	}

	private void OnDisable()
	{
		GameManager.Instance.PlayerEnteredClimbingState -= CheckToDisableParentCollider;
		GameManager.Instance.PlayerExitedClimbingState -= EnableParentCollider;
	}

	private void Update()
	{
		float parentRotation = transform.parent.transform.localEulerAngles.z;

		if (parentRotation == defaultValidRotation)
		{
			ladderTop.transform.localPosition = new Vector3(ladderTop.transform.localPosition.x, ladderTopHeight, ladderTop.transform.localPosition.z);
			ladderBottom.transform.localPosition = new Vector3(ladderBottom.transform.localPosition.x, ladderBottomHeight, ladderBottom.transform.localPosition.z);
		}
		else if (parentRotation == otherValidRotation)
		{
			ladderTop.transform.localPosition = new Vector3(ladderTop.transform.localPosition.x, ladderBottomHeight, ladderTop.transform.localPosition.z);
			ladderBottom.transform.localPosition = new Vector3(ladderBottom.transform.localPosition.x, ladderTopHeight, ladderBottom.transform.localPosition.z);
		}
		else
		{
			boxCollider.enabled = false;
			return;
		}

		boxCollider.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
			collidingWithPlayer = true;
	}

	private void OnTriggerExit2D(Collider2D pCollision)
	{
		if (pCollision.gameObject.CompareTag(Constants.TagPlayer))
			collidingWithPlayer = false;
	}

	private void CheckToDisableParentCollider()
	{
		if (collidingWithPlayer)
			parentCollider.isTrigger = true;
	}

	private void EnableParentCollider()
	{
		parentCollider.isTrigger = false;
	}
}
