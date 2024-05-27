using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public PlayerState PlayerState { get; set; }
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    private void Start()
    {
        PlayerState = gameObject.transform.parent.parent.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (pauseMenu.activeSelf)
            {
                if (PlayerState.WeaponScript != null)
                    PlayerState.WeaponScript.enabled = true;
                pauseMenu.SetActive(false);
            }
            else if (settingsMenu.activeSelf)
            {
                settingsMenu.GetComponent<SettingMenu>().ExitSettingsMenu();
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else
            {
                if (PlayerState.WeaponScript != null)
                    PlayerState.WeaponScript.enabled = false;
                pauseMenu.SetActive(true);
            }
        }
    }

    public void Resume()
    {
        if (PlayerState.WeaponScript != null)
            PlayerState.WeaponScript.enabled = true;
        pauseMenu.SetActive(false);
    }
}
