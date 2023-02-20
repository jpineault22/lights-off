using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDropDownScroller : MonoBehaviour, ISelectHandler
{
	private ScrollRect scrollRect;
	private float scrollPosition = 1;

	private void Start()
    {
		scrollRect = GetComponentInParent<ScrollRect>(true);

		scrollRect.scrollSensitivity = UIManager.Instance.ScrollSensitivity;

		// We remove one item from the child count because the first one comes from the template when Unity generates the Dropdown List
		int childCount = scrollRect.content.transform.childCount - 1;
		int itemIndex = transform.GetSiblingIndex();

		if (itemIndex < 2)
		{
			scrollPosition = 1;
		}
		else if (itemIndex > childCount - 2)
		{
			scrollPosition = 0;
		}
		else
		{
			int scrollOffset = childCount - 2;
			int reversedOffsetIndex = childCount - itemIndex;
			scrollPosition = (float)(reversedOffsetIndex - 1) / scrollOffset;
		}

		UpdateScrollbarValue();
    }

	public void OnSelect(BaseEventData pEventData)
	{
		UpdateScrollbarValue();
	}

	private void UpdateScrollbarValue()
	{
		if (scrollRect && InputManager.Instance.CurrentControlScheme == Constants.InputControlSchemeGamepad)
			scrollRect.verticalScrollbar.value = scrollPosition;
	}
}
