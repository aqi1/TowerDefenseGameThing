using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public AudioMixer audioMixer;
	public Dropdown resolutionDropdown, colorblindDropdown;
	public Toggle fullscreenToggle, vsyncToggle;
	public Slider masterSlider, fxSlider, voiceSlider, musicSlider;
	public List<string> options = new List<string>();

	Resolution[] resolutions;

	public Colorblind colorblindScript;

	void Start()
	{
		// answers.unity.com/questions/1463609/screenresolutions-returning-duplicates.html

		// resolutions = Screen.resolutions;
		resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
		resolutionDropdown.ClearOptions();
		int currentResolutionIndex = 0;

		for (int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + " x " + resolutions[i].height; // + " @ " + resolutions[i].refreshRate;
			options.Add(option);

			if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.RefreshShownValue();

		// should never happen
		if(resolutions.Length == 0)
		{
			Screen.SetResolution(640, 360, false);
		}

		// settings persistence
		resolutionDropdown.value = PlayerPrefs.GetInt("resIndex", currentResolutionIndex);
		fullscreenToggle.isOn = Screen.fullScreen; // don't need to use PlayerPrefs for this
		vsyncToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("vsync", 1));
		masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 0f);
		fxSlider.value = PlayerPrefs.GetFloat("fxVolume", 0f);
		voiceSlider.value = PlayerPrefs.GetFloat("voiceVolume", 0f);
		musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0f);
		colorblindDropdown.value = PlayerPrefs.GetInt("colorblindMode", 0);
	}

	public void SetResolution(int resolutionIndex)
	{
		Debug.Log("SetResolution() entered");
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

		PlayerPrefs.SetInt("resIndex", resolutionIndex);
	}

	public void SetFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;

		/*
		if (isFullscreen)
		{
			PlayerPrefs.SetInt("fullscreen", 1);
		}
		else
		{
			PlayerPrefs.SetInt("fullscreen", 0);
		}
		*/
	}

	public void SetVsync(bool isVsync)
	{
		if (isVsync)
		{
			QualitySettings.vSyncCount = 1;
			PlayerPrefs.SetInt("vsync", 1);
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			PlayerPrefs.SetInt("vsync", 0);
		}
	}

	public void SetVolume(float Volume)
	{
		if (Volume < -39f)
			audioMixer.SetFloat("MainVolume", -80f);
		else
			audioMixer.SetFloat("MainVolume", Volume);

		PlayerPrefs.SetFloat("masterVolume", Volume);
	}

	public void SetFXVolume(float Volume)
	{
		if (Volume < -39f)
			audioMixer.SetFloat("FXVolume", -80f);
		else
			audioMixer.SetFloat("FXVolume", Volume);

		PlayerPrefs.SetFloat("fxVolume", Volume);
	}
	public void SetVoiceVolume(float Volume)
	{
		if (Volume < -39f)
			audioMixer.SetFloat("VoiceVolume", -80f);
		else
			audioMixer.SetFloat("VoiceVolume", Volume);

		PlayerPrefs.SetFloat("voiceVolume", Volume);
	}

	public void SetMusicVolume(float Volume)
	{
		if (Volume < -39f)
			audioMixer.SetFloat("MusicVolume", -80f);
		else
			audioMixer.SetFloat("MusicVolume", Volume);

		PlayerPrefs.SetFloat("musicVolume", Volume);
	}

	public void ColorblindMode(int mode)
    {
		PlayerPrefs.SetInt("colorblindMode", mode);
		
		if(colorblindScript)
			colorblindScript.ChangeColors(mode);
	}
}
