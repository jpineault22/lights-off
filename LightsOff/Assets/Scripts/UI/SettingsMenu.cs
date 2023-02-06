using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : Singleton<SettingsMenu>
{
	[SerializeField] private TMP_Dropdown resolutionDropDown = default;
	[SerializeField] private Slider mainVolumeSlider = default;
	[SerializeField] private Slider musicVolumeSlider = default;
	[SerializeField] private Slider sfxVolumeSlider = default;
	[SerializeField] private Slider uiVolumeSlider = default;

	private Resolution[] resolutions;

	private void Start()
	{
		// Initialize resolutions dropdown
		resolutions = Screen.resolutions;
		resolutionDropDown.ClearOptions();

		List<string> resolutionOptions = new List<string>();
		int currentResolutionIndex = 0;

		for (int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + " x " + resolutions[i].height;
			resolutionOptions.Add(option);

			if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}

		resolutionDropDown.AddOptions(resolutionOptions);
		resolutionDropDown.value = currentResolutionIndex;
		resolutionDropDown.RefreshShownValue();

		mainVolumeSlider.value = AudioManager.Instance.MasterVolume;
		musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
		sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
		uiVolumeSlider.value = AudioManager.Instance.UIVolume;
	}

	public void SetResolution(int pResolutionIndex)
	{
		Resolution resolution = resolutions[pResolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetFullScreen(bool pIsFullScreen)
	{
		Screen.fullScreen = pIsFullScreen;
	}

	public void SetQuality(int pQualityIndex)
	{
		QualitySettings.SetQualityLevel(pQualityIndex);
	}

	public void SetMasterVolume(float pVolume)
	{
		AudioManager.Instance.UpdateMasterVolume(pVolume);
	}

	public void SetMusicVolume(float pVolume)
	{
		AudioManager.Instance.UpdateMusicVolume(pVolume);
	}

	public void SetSFXVolume(float pVolume)
	{
		AudioManager.Instance.UpdateSFXVolume(pVolume);
	}

	public void SetUIVolume(float pVolume)
	{
		AudioManager.Instance.UpdateUIVolume(pVolume);
	}
}
