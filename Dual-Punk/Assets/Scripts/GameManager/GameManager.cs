using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public bool DebugMode;
    public bool InGame;
    public GameObject Player1;
    public GameObject Player2;

    void Start(){
        Player1 = null;
        Player2 = null;
        Instance = this;
    }

    public void FadeIn()
    {
        GameObject.FindWithTag("UI").GetComponentInChildren<Animator>().Play("FadeToBlack_second");
    }

}