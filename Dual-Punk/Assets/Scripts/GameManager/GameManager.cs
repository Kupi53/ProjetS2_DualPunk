using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool DebugMode;
    public bool Solo;
    public bool InGame;
    public bool InTutorial;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject LocalPlayer {
        get
        {
            // REALLY degueulasse car ca a rien a voir avec la camera mais en gros ca return le joueur du client qui appelle la fonction car la camera est que en local
            return GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>().PlayerState.gameObject;
        } 
    }

    void Start(){
        Player1 = null;
        Player2 = null;
        if (Instance == null)
        {
            Instance = this;
        }
        if (!DebugMode)
        {
            InTutorial = true;
        }
    }

    public void FadeIn()
    {
        GameObject.Find($"{LocalPlayer.name} UI").GetComponentInChildren<Animator>().Play("FadeToBlack_second");
    }
}