using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool DebugMode;
    public bool InGame;
    public GameObject Player1;
    public GameObject Player2;

    void Start(){
        Instance = this;
    }
}