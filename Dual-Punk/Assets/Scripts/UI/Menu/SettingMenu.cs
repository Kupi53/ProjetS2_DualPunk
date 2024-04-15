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
    List<string> resolutions = new List<string>{
        "1920x1080",
        "1280x1024",
        "800x600",
        "400x300"
    };
    public Slider musicSlider;
    public Slider soundSlider;


    private int _fullscreen;
    private int _resolution;
    private float _mvolume;
    private float _svolume;

    public void Start()
    {
        Initialise();
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

    private void Initialise()
    {
        _fullscreen = PlayerPrefs.GetInt("Fullscreen");
        _resolution = PlayerPrefs.GetInt("Resolution");
        _mvolume = PlayerPrefs.GetFloat("Music Volume");
        _svolume = PlayerPrefs.GetFloat("SFX Volume");
        ///
		audioMixer.GetFloat("Music", out float musicValueForSlider);
		if (musicValueForSlider <= -30)
		{
			musicSlider.value = -30;
		}
		else
		{
			musicSlider.value = musicValueForSlider;
		}
		audioMixer.GetFloat("Sound", out float soundValueForSlider);
		if (soundValueForSlider <= -30)
		{
			soundSlider.value = -30;
		}
		else
		{
			soundSlider.value = soundValueForSlider;
		}
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions);
    }


    public void SetResolution(int resolutionIndex)
    {
        //   
    }
    
    public void SetMusic(float volume)
    {
        //
        audioMixer.SetFloat("Music", volume);
    }
    
    public void SetSound(float volume)
    {
        //
        audioMixer.SetFloat("Sound", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        //
        Screen.fullScreen = isFullScreen;
    }

    private void QuitMenu(){
        //
    }
}
