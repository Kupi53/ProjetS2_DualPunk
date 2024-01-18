using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Classe base pour les joueurs
public class PlayerState : MonoBehaviour {

    [SerializeField]
    public int[] Stats;
    public bool HoldingWeapon { get; set; }
    public static bool aiming = false;
    public static bool dashing = false;
    public static float dashTimer = 0.0f;
    public static float dashCooldown = 0.0f;
    public static float dashCooldownMax = 1.0f;
    

    void Awake(){
        HoldingWeapon = false;
    }
}