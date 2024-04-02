using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        /*Load la prochaine scene en recuperant la 
        scene actuelle et en generant la prochaine*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    public void QuitGame()
    {
        Application.Quit();     /*Quitte le jeu*/
    }
}
