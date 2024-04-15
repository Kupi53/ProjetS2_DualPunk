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
    public static SettingsManager Instance;

    private const int FULLSCREENDEFAULT = 0;
    private const int RESOLUTIONDEFAULT = 0;
    private const float SFXVOLUMEMAX = 0f;
    private const float MUSICVOLUMEMAX = 0f;
    private const float SFXVOLUMEDEFAULT = SFXVOLUMEMAX;
    private const float MUSICVOLUMEDEFAULT = MUSICVOLUMEMAX;
    private const FullScreenMode FULLSCREENMODE = FullScreenMode.FullScreenWindow;
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

        Screen.fullScreenMode = FULLSCREENMODE;
        DefaultSettings();
        LoadSettings();
    }

    private void DefaultSettings(){
        if (!PlayerPrefs.HasKey("Fullscreen"))
            PlayerPrefs.SetInt("Fullscreen", FULLSCREENDEFAULT);
        if (!PlayerPrefs.HasKey("Resolution"))
            PlayerPrefs.SetInt("Resolution", RESOLUTIONDEFAULT);
        if (!PlayerPrefs.HasKey("SFX Volume"))
            PlayerPrefs.SetFloat("SFX Volume", SFXVOLUMEDEFAULT);
        if (!PlayerPrefs.HasKey("Music Volume"))
            PlayerPrefs.SetFloat("Music Volume", MUSICVOLUMEDEFAULT);
        PlayerPrefs.Save();
    }
    public void LoadSettings(){
    //
        int fullscreen = PlayerPrefs.GetInt("Fullscreen");
        int resIndex = PlayerPrefs.GetInt("Resolution");
        if (resIndex < RESOLUTIONS.Length){
            if (fullscreen == 1){
                Screen.SetResolution(RESOLUTIONS[resIndex].Item1, RESOLUTIONS[resIndex].Item2, true);
            }
            else{
                Screen.SetResolution(RESOLUTIONS[resIndex].Item1, RESOLUTIONS[resIndex].Item2, false);
            }
        }
        else {
            PlayerPrefs.SetInt("Resolution", RESOLUTIONDEFAULT);
        }
    //
        float sfxVol = PlayerPrefs.GetFloat("SFX Volume");
        if (sfxVol <= SFXVOLUMEMAX){
            audioMixer.SetFloat("Sound", sfxVol);
        }
        else {
            PlayerPrefs.SetFloat("SFX Volume", SFXVOLUMEDEFAULT);
        }
    //
        float musicVol = PlayerPrefs.GetFloat("Music Volume");
        if (musicVol <= MUSICVOLUMEMAX){
            audioMixer.SetFloat("Music", musicVol);
        }
        else{
            PlayerPrefs.SetFloat("Music Volume", MUSICVOLUMEDEFAULT);
        }
    }

    public void SaveSettings(int fullscreen, int resolution, float mvolume, float svolume){
        Debug.Log(svolume);
        Debug.Log(resolution);
        PlayerPrefs.SetInt("Fullscreen", fullscreen);
        PlayerPrefs.SetInt("Resolution", resolution);
        PlayerPrefs.SetFloat("Music Volume", mvolume);
        PlayerPrefs.SetFloat("SFX Volume", svolume);
        PlayerPrefs.Save();
    }
}
