﻿using UnityEngine;

public class Device : RotatableObject
{
	[SerializeField] protected bool isConnected = true;
	[SerializeField] protected bool isOn;
    [SerializeField] protected Sprite spriteOn = default;
    [SerializeField] protected Sprite spriteOff = default;
	[SerializeField] protected Sprite spriteInactive = default;

    protected SpriteRenderer spriteRenderer;
	protected SpriteRenderer outlineSpriteRenderer;

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
				ShowOutline(false);
			}
		}

		if (spriteInactive == null)
			spriteInactive = spriteOff;

		ApplyOnOffBehavior();
	}

	protected virtual void OnEnable()
	{
		GameManager.Instance.PlayerSpawned += AssignAudioEmitterToPlayerListener;
	}

	protected virtual void OnDisable()
	{
		GameManager.Instance.PlayerSpawned -= AssignAudioEmitterToPlayerListener;
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

	public virtual void ShowOutline(bool pShow)
	{
		outlineSpriteRenderer.enabled = pShow;
	}

	public virtual void ChangeOutlineColor(Color pColor)
	{
		outlineSpriteRenderer.color = pColor;
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
}
