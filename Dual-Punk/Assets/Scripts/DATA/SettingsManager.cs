using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Demo.AdditiveScenes;
using GameKit.Utilities.Types.CanvasContainers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    public SettingsManager Instance;

    private const int FULLSCREENDEFAULT = 0;
    private const int RESOLUTIONDEFAULT = 0;
    private const float SFXVOLUMEMAX = 100f;
    private const float MUSICVOLUMEMAX = 100f;
    private const float SFXVOLUMEDEFAULT = SFXVOLUMEMAX;
    private const float MUSICVOLUMEDEFAULT = MUSICVOLUMEMAX;
    private (int, int)[] RESOLUTIONS = {
        (1920, 1080),
        (1280, 1024),
        (800, 600),
        (400, 300)
    };

    [SerializeField] private AudioMixer audioMixer;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;

        DefaultSettings();
        LoadSettings();
    }

    private void DefaultSettings(){
        if (!PlayerPrefs.HasKey("Fullscreen"))
            PlayerPrefs.SetInt("Fullscreen", FULLSCREENDEFAULT);
        if (!PlayerPrefs.HasKey("Resolution"))
            PlayerPrefs.SetInt("Resolution", RESOLUTIONDEFAULT);
        if (!PlayerPrefs.HasKey("SFXVol"))
            PlayerPrefs.SetFloat("SFX Volume", SFXVOLUMEDEFAULT);
        if (!PlayerPrefs.HasKey("Music Volume"))
            PlayerPrefs.SetFloat("Music Volume", MUSICVOLUMEDEFAULT);
        PlayerPrefs.Save();
    }
    public void LoadSettings(){
        if (PlayerPrefs.GetInt("Fullscreen") == 1)
            Screen.fullScreen = true;
        else Screen.fullScreen = false;
    //
        int resIndex = PlayerPrefs.GetInt("Resolution");
        if (resIndex < RESOLUTIONS.Length){
            Screen.SetResolution(RESOLUTIONS[resIndex].Item1, RESOLUTIONS[resIndex].Item2, Screen.fullScreen);
        }
        else {
            PlayerPrefs.SetInt("Resolution", RESOLUTIONDEFAULT);
        }
    //
        float sfxVol = PlayerPrefs.GetFloat("SFX Volume") - 100f;
        if (sfxVol <= SFXVOLUMEMAX){
            audioMixer.SetFloat("Sound", sfxVol);
        }
        else {
            PlayerPrefs.SetFloat("SFX Volume", SFXVOLUMEDEFAULT);
        }
    //
        float musicVol = PlayerPrefs.GetFloat("Music Volume") - 100f;
        if (musicVol <= MUSICVOLUMEMAX){
            audioMixer.SetFloat("Music", musicVol);
        }
        else{
            PlayerPrefs.SetFloat("Music Volume", MUSICVOLUMEDEFAULT);
        }
    }

    public void SaveSettings(int fullscreen, int resolution, float mvolume, float svolume){
        PlayerPrefs.SetInt("Fullscreen", fullscreen);
        PlayerPrefs.SetInt("Resolution", resolution);
        PlayerPrefs.SetFloat("Music Volume", mvolume);
        PlayerPrefs.SetFloat("SFX Volume", svolume);
        PlayerPrefs.Save();
    }
}
