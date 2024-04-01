using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
	public AudioSource audioSource;
    public AudioClip musicMenu;
    
    public AudioClip[] playlist;
    private int indexPlaylist = 0;
    private bool isInGame = false;
    
    void Update()
    {
		if ((!audioSource.isPlaying || audioSource.clip.name != "Menu") && (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1))
		{
			isInGame = false;

			audioSource.Stop();
			
			audioSource.clip = musicMenu;
			audioSource.Play();
		}
		else if (audioSource.clip.name == "Menu" && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
		{
			audioSource.Stop();
			
			audioSource.clip = playlist[indexPlaylist];
			audioSource.Play();

			isInGame = true;
		}

		if (isInGame && !audioSource.isPlaying)
		{
			indexPlaylist = (indexPlaylist + 1) % playlist.Length;
			audioSource.clip = playlist[indexPlaylist];
			audioSource.Play();
		}
    }
}
