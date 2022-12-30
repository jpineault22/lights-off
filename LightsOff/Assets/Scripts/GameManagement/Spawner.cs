using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
	#region Prefabs

	[Header("Prefabs")]
	[SerializeField] private GameObject playerPrefab = default;
    [SerializeField] private GameObject enemyPassedOutPrefab = default;

	#endregion

	private List<GameObject> objectsToDestroyOnReload;

	protected override void Awake()
	{
		base.Awake();

		objectsToDestroyOnReload = new List<GameObject>();
	}

	public GameObject InstantiatePlayer()
	{
		return Instantiate(playerPrefab);
	}

    public void InstantiateEnemyPassedOut(Vector3 pPosition, Quaternion pRotation)
	{
        GameObject enemy = Instantiate(enemyPassedOutPrefab, pPosition, pRotation);
		objectsToDestroyOnReload.Add(enemy);
	}

    public GameObject FindStartDoor(GameObject pGameObjects)
	{
        foreach (GameObject obj in GameObjectUtils.GetChildren(pGameObjects))
		{
            if (obj.CompareTag(Constants.TagStartDoor))
			{
                return obj;
			}
		}

        return null;
	}

	public void DestroyObjectsForReload()
	{
		foreach(GameObject objectToDestroy in objectsToDestroyOnReload)
		{
			Destroy(objectToDestroy);
		}

		objectsToDestroyOnReload.Clear();
	}
}
