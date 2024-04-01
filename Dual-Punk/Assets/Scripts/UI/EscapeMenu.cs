using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenu;
    public GameObject settingsMenu;
    
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !settingsMenu.activeSelf && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
        {
            escapeMenu.SetActive(true);
        }
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
