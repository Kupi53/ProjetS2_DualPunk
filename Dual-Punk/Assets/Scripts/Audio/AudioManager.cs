using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	
	public AudioSource musicSource;
    public AudioClip musicMenu;
    public AudioClip clicOnMenu;
    
    public AudioClip[] playlist;
    private int indexPlaylist = 0;
    private bool isInGame = false;

    public AudioMixerGroup soundEffectMixer;
    
    void Start()
    {
	    if (Instance == null)
	    {
		    Instance = this;
	    }

	    if (this == Instance)
	    {
		    musicSource.clip = musicMenu;
		    musicSource.Play();
	    }
    }
    
    void Update()
    {
	    if (this == Instance)
	    {
		    if ((!musicSource.isPlaying || musicSource.clip.name != "Menu") && (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1))
		    {
			    isInGame = false;

			    musicSource.Stop();

			    musicSource.clip = musicMenu;
			    musicSource.Play();
		    }
		    else if (musicSource.clip.name == "Menu" && SceneManager.GetActiveScene().buildIndex != 0 &&
		             SceneManager.GetActiveScene().buildIndex != 1)
		    {
			    musicSource.Stop();

			    musicSource.clip = playlist[indexPlaylist];
			    musicSource.Play();

			    isInGame = true;
		    }

		    if (isInGame && !musicSource.isPlaying)
		    {
			    indexPlaylist = (indexPlaylist + 1) % playlist.Length;
			    musicSource.clip = playlist[indexPlaylist];
			    musicSource.Play();
		    }
	    }
    }
    
    public void ClicOnMenu()
    {
	    AudioManager.Instance.PlayClipAt(clicOnMenu, transform.position);
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
	    GameObject soundEffectTemp = new GameObject("TempAudio");
	    soundEffectTemp.tag = "TempAudio";
	    soundEffectTemp.transform.position = pos;
	    AudioSource audioSource = soundEffectTemp.AddComponent<AudioSource>();
	    audioSource.clip = clip;
	    audioSource.outputAudioMixerGroup = soundEffectMixer;
	    audioSource.Play();  
	    Destroy(soundEffectTemp, clip.length);
	    return audioSource;
    }
}
