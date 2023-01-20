using System.Collections;
using UnityEngine;

public class CreditsManager : Singleton<CreditsManager>
{
	[SerializeField] private float timeBeforeCredits = 1f;
	[SerializeField] private float timeBetweenScreens = 1f;
	[SerializeField] private CreditScreen[] screens = default;

    public IEnumerator StartCredits()
	{
		yield return new WaitForSeconds(timeBeforeCredits);

		for (int i = 0; i < screens.Length; i++)
		{
			screens[i].EnableScreenText(true);
			yield return new WaitForSeconds(screens[i].screenDuration);

			if (i == screens.Length - 1)
			{
				InputManager.Instance.EnablePlayerInputCreditsEnd();
				break;
			}

			screens[i].EnableScreenText(false);
			yield return new WaitForSeconds(timeBetweenScreens);
		}
	}

	public void DisableLastScreen()
	{
		screens[screens.Length - 1].EnableScreenText(false);
	}
}
