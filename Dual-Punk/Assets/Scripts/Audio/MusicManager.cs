using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	
	public AudioSource musicSource;
	public AudioSource soundSource;
    public AudioClip musicMenu;
    public AudioClip clicOnMenu;
    
    public AudioClip[] playlist;
    private int indexPlaylist = 0;
    private bool isInGame = false;
    
    void Start()
    {
	    if (Instance == null)
	    {
		    Instance = this;
		    DontDestroyOnLoad(gameObject);
	    }
	    else
	    {
		    Destroy(gameObject);
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
	    soundSource.PlayOneShot(clicOnMenu);
    }
}
