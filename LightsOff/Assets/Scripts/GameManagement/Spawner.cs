using UnityEngine;

public class Spawner : Singleton<Spawner>
{
	#region Prefabs

	[Header("Prefabs")]
	[SerializeField] private GameObject playerPrefab = default;
    [SerializeField] private GameObject enemyPassedOutPrefab = default;

	#endregion

	public GameObject InstantiatePlayer()
	{
		return Instantiate(playerPrefab);
	}

	public void DestroyPlayer(GameObject pPlayer)
	{
		Destroy(pPlayer);
	}

	public void DestroyUIManager(GameObject pUIManager)
	{
		Destroy(pUIManager);
	}

	public void DestroyMainMenu(GameObject pMainMenu)
	{
		Destroy(pMainMenu);
	}

    public void InstantiateEnemyPassedOut(Vector3 pPosition, Quaternion pRotation)
	{
        Instantiate(enemyPassedOutPrefab, pPosition, pRotation);
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
}
