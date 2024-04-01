using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clicMenu;

    public void ClicMenu()
    {
        audioSource.PlayOneShot(clicMenu);
    }
}
