using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerState : MonoBehaviour
{
    public float Health;
    public float MaxHealth = 100;
    public bool Aiming = false;
    public bool Walking = false;
    public bool Dashing = false;
    public bool HoldingWeapon = false;
    public float DashTimer = 0.0f;
    public float DashCooldown = 0.0f;
    public float DashCooldownMax = 1.0f;

    private void Awake()
    {
        Health = MaxHealth;
    }
}