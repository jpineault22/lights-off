using System;
using UnityEngine;

public class Device : RotatableObject
{
	[SerializeField] protected bool isConnected = true;
	[SerializeField] protected bool isOn;
    [SerializeField] protected Sprite spriteOn = default;
    [SerializeField] protected Sprite spriteOff = default;
	[SerializeField] protected Sprite spriteInactive = default;
	[SerializeField] protected float enemyDistanceToBlock = 6f;

    protected SpriteRenderer spriteRenderer;
	protected SpriteRenderer outlineSpriteRenderer;
	protected GameObject spawnedEnemy;									// This works only with the current setups, that is only one enemy per level allowed
	protected Vector2 spawnedEnemyBoundsExtents;
	protected bool deviceBlocked;

	public event Action<GameObject, bool> DeviceBlocked;

	protected override void Awake()
	{
		base.Awake();
		
		spriteRenderer = GetComponent<SpriteRenderer>();

		AssignAudioEmitterToPlayerListener();

		foreach (GameObject obj in GameObjectUtils.GetChildren(gameObject))
		{
			if (obj.CompareTag(Constants.TagOutline))
			{
				outlineSpriteRenderer = obj.GetComponent<SpriteRenderer>();
				outlineSpriteRenderer.color = UIManager.Instance.deviceOutlineColor;
				ShowOutline(false, false);
			}
		}

		if (spriteInactive == null)
			spriteInactive = spriteOff;

		ApplyOnOffBehavior();
	}

	protected virtual void OnEnable()
	{
		GameManager.Instance.PlayerSpawned += AssignAudioEmitterToPlayerListener;
		Spawner.Instance.EnemySpawned += GetEnemyReference;
	}

	protected virtual void OnDisable()
	{
		GameManager.Instance.PlayerSpawned -= AssignAudioEmitterToPlayerListener;
		Spawner.Instance.EnemySpawned -= GetEnemyReference;
	}

	public void SwitchOnOff()
	{
		isOn = !isOn;

		ApplyOnOffBehavior();
	}

	public void DisconnectReconnect()
	{
		isConnected = !isConnected;

		ApplyOnOffBehavior();
	}

	public virtual void ApplyOnOffBehavior()
	{
		if (isConnected)
		{
			if (isOn)
				spriteRenderer.sprite = spriteOn;
			else
				spriteRenderer.sprite = spriteOff;
		}
		else
		{
			spriteRenderer.sprite = spriteInactive;
		}
	}

	public virtual void ShowOutline(bool pShow, bool pFromBreaker)
	{
		ChangeOutlineColor(pFromBreaker, false);
		
		outlineSpriteRenderer.enabled = pShow;
	}

	public virtual void ChangeOutlineColor(bool pFromBreaker, bool pSwitchBlocked)
	{
		if (pFromBreaker)
		{
			outlineSpriteRenderer.color = UIManager.Instance.breakerOutlineColor;
		}
		else
		{
			if (isConnected && !pSwitchBlocked)
				outlineSpriteRenderer.color = UIManager.Instance.deviceOutlineColor;
			else
				outlineSpriteRenderer.color = UIManager.Instance.inactiveOutlineColor;
		}
	}

	public bool IsOnAndConnected()
	{
		return isOn && isConnected;
	}

	public bool IsConnected()
	{
		return isConnected;
	}

	private void AssignAudioEmitterToPlayerListener()
	{
		AudioManager.Instance.AssignEmitterToPlayerListener(gameObject);
	}

	protected void GetEnemyReference(GameObject pSpawnedEnemy)
	{
		spawnedEnemy = pSpawnedEnemy;
		SpriteRenderer spawnedEnemySpriteRenderer = pSpawnedEnemy.GetComponent<SpriteRenderer>();
		spawnedEnemyBoundsExtents = new Vector2(spawnedEnemySpriteRenderer.bounds.extents.x, spawnedEnemySpriteRenderer.bounds.extents.y);
	}

	protected void CheckIfDeviceBlocked(float pDeviceLeftEdge, float pDeviceRightEdge, float pEnemyDistanceBuffer)
	{
		if (transform.eulerAngles.z != 0 || Mathf.Abs(spawnedEnemy.transform.position.y - transform.position.y) >= enemyDistanceToBlock)
			return;
		
		float enemyRightEdge = spawnedEnemy.transform.position.x + spawnedEnemyBoundsExtents.x;
		float enemyLeftEdge = spawnedEnemy.transform.position.x - spawnedEnemyBoundsExtents.x;

		if (!deviceBlocked && enemyRightEdge - pEnemyDistanceBuffer > pDeviceLeftEdge && enemyLeftEdge + pEnemyDistanceBuffer < pDeviceRightEdge)
		{
			deviceBlocked = true;
			InvokeDeviceBlocked(true);
		}
		else if (deviceBlocked && (enemyRightEdge - pEnemyDistanceBuffer <= pDeviceLeftEdge || enemyLeftEdge + pEnemyDistanceBuffer >= pDeviceRightEdge))
		{
			deviceBlocked = false;
			InvokeDeviceBlocked(false);
		}
	}

	protected void InvokeDeviceBlocked(bool pBlocked)
	{
		DeviceBlocked?.Invoke(gameObject, pBlocked);
	}
}
