using UnityEngine;

public class Spawner : Singleton<Spawner>
{
	#region Prefabs

	[Header("Prefabs")]
	[SerializeField] private GameObject playerPrefab = default;
    [SerializeField] private GameObject enemyPassedOutPrefab = default;

	#endregion

	/*public GameObject[] SpawnPoints { get; private set; }
	public GameObject FirstPlayerSpawnPoint { get; private set; }
    public Vector2? PlayerSavedPosition { get; set; }*/

	public GameObject InstantiatePlayer()
	{
		return Instantiate(playerPrefab);
	}

	public void DestroyPlayer(GameObject pPlayer)
	{
		Destroy(pPlayer);
	}

    public void InstantiateEnemyPassedOut(Vector3 pPosition, Quaternion pRotation)
	{
        Instantiate(enemyPassedOutPrefab, pPosition, pRotation);
	}

    /*public void FindSpawnPoints(GameObject[] pGameObjects)
    {
        foreach (GameObject obj in pGameObjects)
        {
            if (obj.CompareTag(Constants.TagSpawnPoints))
            {
                SpawnPoints = GameObjectUtils.GetChildren(obj);
            }
        }

        if (SpawnPoints.Length > 0)
        {
            InstantiatePowerupsAndEnemies();
        }
    }*/

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

    /*private void InstantiatePowerupsAndEnemies()
    {
        foreach (GameObject spawnPoint in SpawnPoints)
        {
            // Retrieve initial player spawn point if in first map
            if (LevelLoader.Instance.CurrentLevelName == Constants.NamePrefixSceneLevel + Constants.StartingLevelNumber
            && spawnPoint.CompareTag(Constants.TagFirstPlayerSpawnPoint))
            {
                FirstPlayerSpawnPoint = spawnPoint;
            }
            // Spawn powerups/hearts
            else if (spawnPoint.CompareTag(Constants.TagDashPowerupSpawnPoint))
            {
                if (!PlayerController.Instance.HasDash)
                {
                    Instantiate(dashPowerupPrefab, spawnPoint.transform);
                }
            }
            else if (spawnPoint.CompareTag(Constants.TagWallJumpPowerupSpawnPoint))
            {
                if (!PlayerController.Instance.HasWallJump)
                {
                    Instantiate(wallJumpPowerupPrefab, spawnPoint.transform);
                }
            }
            else if (spawnPoint.CompareTag(Constants.TagBombPowerupSpawnPoint))
            {
                if (!PlayerController.Instance.HasBomb)
                {
                    Instantiate(bombPowerupPrefab, spawnPoint.transform);
                }
            }
            else if (spawnPoint.CompareTag(Constants.TagHeartSpawnPoint))
            {
                Instantiate(heartPrefab, spawnPoint.transform);
            }
            // Spawn enemies
            else if (spawnPoint.CompareTag(Constants.TagEnemyPlaceholderSpawnPoint))
            {
                Instantiate(enemyPlaceholderPrefab, spawnPoint.transform);
            }
        }
    }*/
}
