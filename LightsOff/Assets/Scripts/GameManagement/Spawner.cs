using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
	#region Prefabs

	[Header("Prefabs")]
	[SerializeField] private GameObject[] systemPrefabs = default;          // Prefabs to instantiate when launching the game
	[SerializeField] private GameObject playerPrefab = default;
	[SerializeField] private GameObject enemyDrunkPrefab = default;
    [SerializeField] private GameObject enemyPassedOutPrefab = default;

	#endregion

	[Header("Parameters")]
	[SerializeField] private float playerDoorHeightDifference = 0.5f;       // The player's position at the start of a level will be lower than the start door's position by this amount (initial 0.56f)

	private List<GameObject> instantiatedSystemPrefabs;
	private List<GameObject> objectsToDestroyOnReload;
	private GameObject[] levelRootGameObjects;
	private GameObject currentFunctionalLevel;
	private GameObject[] spawnPoints;
	private Vector2 startDoorPosition;

	protected override void Awake()
	{
		base.Awake();

		objectsToDestroyOnReload = new List<GameObject>();
		instantiatedSystemPrefabs = new List<GameObject>();
		InstantiateSystemPrefabs();
	}

	protected override void OnDestroy()
	{
		for (int i = 0; i < instantiatedSystemPrefabs.Count; i++)
		{
			Destroy(instantiatedSystemPrefabs[i]);
		}

		instantiatedSystemPrefabs.Clear();

		base.OnDestroy();
	}

	// Instantiate and destroy methods

	private void InstantiateSystemPrefabs()
	{
		for (int i = 0; i < systemPrefabs.Length; i++)
		{
			GameObject prefabInstance = Instantiate(systemPrefabs[i]);
			instantiatedSystemPrefabs.Add(prefabInstance);
		}
	}

	public GameObject SpawnPlayer()
	{
		return Instantiate(playerPrefab);
	}

	public void SpawnEnemies()
	{
		FindSpawnPoints();
		
		if (spawnPoints != null && spawnPoints.Length > 0)
		{
			foreach (GameObject spawnPoint in spawnPoints)
			{
				if (spawnPoint.CompareTag(Constants.TagSpawnPointEnemyDrunk))
				{
					Instantiate(enemyDrunkPrefab, spawnPoint.transform);
				}
				else if (spawnPoint.CompareTag(Constants.TagSpawnPointEnemyPassedOut))
				{
					Instantiate(enemyPassedOutPrefab, spawnPoint.transform);
				}
			}
		}
	}

    public void SpawnNewEnemyPassedOut(Vector3 pPosition, Quaternion pRotation)
	{
        GameObject enemy = Instantiate(enemyPassedOutPrefab, pPosition, pRotation);
		objectsToDestroyOnReload.Add(enemy);
	}

	public void DestroyObjectsForReload()
	{
		foreach (GameObject objectToDestroy in objectsToDestroyOnReload)
		{
			Destroy(objectToDestroy);
		}

		objectsToDestroyOnReload.Clear();
	}

	// Find objects in scene methods

	public GameObject FindStartDoor()
	{
		return FindObjectOfTag(currentFunctionalLevel, Constants.TagStartDoor);
	}

	public GameObject FindWindow()
	{
		return FindObjectOfTag(currentFunctionalLevel, Constants.TagWindow);
	}

	public void FindFunctionalLevel()
	{
		foreach (GameObject obj in levelRootGameObjects)
		{
			if (obj.CompareTag(Constants.TagFunctionalLevel))
			{
				currentFunctionalLevel = obj;
				return;
			}
		}

		currentFunctionalLevel = null;
	}

	private void FindSpawnPoints()
	{
		foreach (GameObject obj in levelRootGameObjects)
		{
			if (obj.CompareTag(Constants.TagSpawnPoints))
			{
				spawnPoints = GameObjectUtils.GetChildren(obj);
				return;
			}
		}

		spawnPoints = null;
	}

	private GameObject FindObjectOfTag(GameObject pGameObjects, string pTag)
	{
		foreach (GameObject obj in GameObjectUtils.GetChildren(pGameObjects))
		{
			if (obj.CompareTag(pTag))
			{
				return obj;
			}
		}

		return null;
	}

	// Set methods

	public void SetRootGameObjects(GameObject[] pGameObjects)
	{
		levelRootGameObjects = pGameObjects;
	}

	public void SetStartDoorPosition(Vector2 pPosition)
	{
		startDoorPosition = pPosition;
	}

	public Vector2 GetPositionForPlayerSpawn()
	{
		return new Vector2(startDoorPosition.x, startDoorPosition.y - playerDoorHeightDifference);
	}

	public Vector2? GetLevelCameraPosition()
	{
		if (currentFunctionalLevel != null)
			return currentFunctionalLevel.GetComponent<LevelData>().cameraPosition;
		else
			return null;
	}

	public float? GetLevelCameraSize()
	{
		if (currentFunctionalLevel != null)
			return currentFunctionalLevel.GetComponent<LevelData>().cameraSize;
		else
			return null;
	}
}
