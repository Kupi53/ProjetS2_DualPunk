using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
            }
            else if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
        }
    }
    
    public void MainMenu()
    {
        DontDestroyOnLoadScene.Instance.RemoveFromDontDestroyOnLoad();
        SceneManager.LoadScene(0);
    }
}
