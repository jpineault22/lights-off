using UnityEngine;

public static class GameObjectUtils
{
	public static GameObject[] GetChildren(GameObject pGameObject)
	{
		GameObject[] gameObjects = new GameObject[pGameObject.transform.childCount];

		for (int i = 0; i < gameObjects.Length; i++)
		{
			gameObjects[i] = pGameObject.transform.GetChild(i).gameObject;
		}

		return gameObjects;
	}

	public static bool AnimatorHasParameter(Animator pAnimator, string pParameterName)
	{
		foreach (AnimatorControllerParameter parameter in pAnimator.parameters)
		{
			if (parameter.name == pParameterName)
				return true;
		}

		return false;
	}
}
