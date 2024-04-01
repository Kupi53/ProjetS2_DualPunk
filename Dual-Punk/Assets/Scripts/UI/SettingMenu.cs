using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;
    public Slider musicSlider;
    public Slider soundSlider;

    public void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Insert(0, option);
        }

        resolutionDropdown.AddOptions(options);

        Screen.fullScreen = true;
    }

    void Update()
    {
        if (musicSlider.value == -30)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        
        if (soundSlider.value == -30)
        {
            audioMixer.SetFloat("Sound", -80f);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutions.Length - resolutionIndex - 1];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }
    
    public void SetSound(float volume)
    {
        audioMixer.SetFloat("Sound", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
