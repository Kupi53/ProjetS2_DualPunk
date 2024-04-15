using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UIElements;

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
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider soundSlider;
    [SerializeField] UnityEngine.UI.Toggle fullscreenToggle;


    private int _fullscreen;
    private int _resolution;
    private float _mvolume;
    private float _svolume;

    public void Start()
    {
        Debug.Log("test");
        Initialise();
    }

    private void Initialise()
    {
        _fullscreen = PlayerPrefs.GetInt("Fullscreen");
        _resolution = PlayerPrefs.GetInt("Resolution");
        _mvolume = PlayerPrefs.GetFloat("Music Volume");
        _svolume = PlayerPrefs.GetFloat("SFX Volume");
        ///
		audioMixer.GetFloat("Music", out float musicValueForSlider);
		musicSlider.value = musicValueForSlider;
		audioMixer.GetFloat("Sound", out float soundValueForSlider);
		soundSlider.value = soundValueForSlider;
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions);
        resolutionDropdown.value = _resolution;
        if (_fullscreen == 1){
            fullscreenToggle.isOn = true;
        }
        else fullscreenToggle.isOn = false;
    }


    public void SetResolution(int resolutionIndex)
    {
        _resolution = resolutionIndex;
    }
    
    public void SetMusic(float volume)
    {
        //
        audioMixer.SetFloat("Music", volume);
        _mvolume = volume;
    }
    
    public void SetSound(float volume)
    {
        //
        audioMixer.SetFloat("Sound", volume);
        _svolume = volume;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        //
        if (isFullScreen) _fullscreen = 1;
        else _fullscreen = 0;
    }

    public void ExitSettingsMenu(){
        SettingsManager.Instance.LoadSettings();
        Initialise();
    }

    public void ApplySettings(){
        SettingsManager.Instance.SaveSettings(_fullscreen, _resolution, _mvolume, _svolume);
        SettingsManager.Instance.LoadSettings();
    }
}
