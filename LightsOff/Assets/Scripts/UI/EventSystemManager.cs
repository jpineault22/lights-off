using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemManager : Singleton<EventSystemManager>
{
    private InputSystemUIInputModule module;
	private EventSystem currentEventSystem;
	private GameObject currentlySelectedObject;

	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);

		module = GetComponent<InputSystemUIInputModule>();
		currentEventSystem = EventSystem.current;
		currentlySelectedObject = currentEventSystem.currentSelectedGameObject;
	}

	private void Update()
	{
		if (GameManager.Instance.CurrentGameState == GameState.Menu || GameManager.Instance.CurrentGameState == GameState.Paused)
		{
			if (currentEventSystem.currentSelectedGameObject != null && currentlySelectedObject != currentEventSystem.currentSelectedGameObject)
			{
				currentlySelectedObject = currentEventSystem.currentSelectedGameObject;
			}

			if (currentEventSystem.currentSelectedGameObject == null)
			{
				if (currentlySelectedObject != null)
				{
					currentlySelectedObject.GetComponent<Selectable>().Select();
				}
			}
		}
	}

	public void ActivateModule()
	{
		module.enabled = true;
	}

	public void DeactivateModule()
	{
		module.enabled = false;
	}
}
