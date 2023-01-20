using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public static List<Scene> GetLoadedScenesNoBoot()
	{
		int sceneCount = SceneManager.sceneCount;

		List<Scene> loadedScenes = new List<Scene>();

		for (int i = 0; i < sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);

			if (scene.name != Constants.NameSceneBoot)
				loadedScenes.Add(scene);
		}

		return loadedScenes;
	}
}
