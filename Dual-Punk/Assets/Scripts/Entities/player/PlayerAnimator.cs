using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour{
    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";
    public Animator animator;
    private string currentState;

      void Start()
    {
        animator = GetComponent<Animator>();
        // de base le joueur face en bas
        currentState = PLAYER_S;
    }
}