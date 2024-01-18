using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Classe base pour les joueurs
public class PlayerState : MonoBehaviour {

    [SerializeField]
    private int maxHealth = 100;
    public int[] Stats;
    public float Health;
    public bool HoldingWeapon {get;set; }

    public static bool dashing = false;
    public static float dashTimer = 0.0f;
    public static float dashCooldown = 0.0f;
    public static float dashCooldownMax = 1.0f;

    void Awake(){
        Health = maxHealth;
        HoldingWeapon = false;
    }
}