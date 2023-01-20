using Cinemachine;
using UnityEngine;

public class CinemachineManagerV2 : Singleton<CinemachineManagerV2>
{
	public float defaultCameraSize = 10f;
	public float zoomDistance = 1f;

	private float targetSize;
	
	private CinemachineVirtualCamera virtualCamera;

	protected override void Awake()
	{
		base.Awake();

		virtualCamera = GetComponent<CinemachineVirtualCamera>();

		targetSize = defaultCameraSize;
	}

	public void ChangeCameraPositionAndSize(Vector2? pPosition, float? pSize)
	{
		if (!pPosition.HasValue || !pSize.HasValue)
			return;
		
		transform.position = new Vector3(pPosition.Value.x, pPosition.Value.y, -10);
		virtualCamera.m_Lens.OrthographicSize = pSize.Value;
	}

	public void ZoomOutInstantly()
	{
		targetSize = virtualCamera.m_Lens.OrthographicSize;
		virtualCamera.m_Lens.OrthographicSize += zoomDistance;
	}

	public void ZoomIn(float pDifferenceProportion)
	{
		virtualCamera.m_Lens.OrthographicSize -= pDifferenceProportion * zoomDistance;

		if (virtualCamera.m_Lens.OrthographicSize < targetSize)
			virtualCamera.m_Lens.OrthographicSize = targetSize;
	}
}
