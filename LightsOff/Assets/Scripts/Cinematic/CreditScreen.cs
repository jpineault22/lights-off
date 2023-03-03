using TMPro;
using UnityEngine;

[System.Serializable]
public class CreditScreen
{
    public RectTransform[] creditElement;
    public float screenDuration;

    public void EnableScreenText(bool pEnable)
	{
        foreach (RectTransform element in creditElement)
		{
			element.gameObject.SetActive(pEnable);
		}
	}
}
