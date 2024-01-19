using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Classe base pour les joueurs
public class PlayerState : MonoBehaviour {

    [SerializeField]
    private int MaxHealth = 100;
    public int[] Stats;
    public float Health;

    public static bool HoldingWeapon = false;
    public static bool Aiming = false;
    public static bool Dashing = false;
    public static float DashTimer = 0.0f;
    public static float DashCooldown = 0.0f;
    public static float DashCooldownMax = 1.0f;


    void Awake()
    {
        Health = MaxHealth;
        HoldingWeapon = false;
    }
}