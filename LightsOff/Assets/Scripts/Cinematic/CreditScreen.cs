using TMPro;

[System.Serializable]
public class CreditScreen
{
    public TMP_Text[] creditText;
    public float screenDuration;

    public void EnableScreenText(bool pEnable)
	{
        foreach (TMP_Text text in creditText)
		{
			text.enabled = pEnable;
		}
	}
}
