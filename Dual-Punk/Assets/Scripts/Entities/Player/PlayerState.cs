using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerState : MonoBehaviour
{
    public int Health;
    public int MaxHealth = 100;

    public bool Walking = false;
    public bool Dashing = false;
    public bool HoldingWeapon = false;
    public bool HoldingKnife = false;

    public float DashCooldown = 0.0f;
    public float DashCooldownMax = 1.0f;

    public GameObject? Pointer;
    public GameObject? Weapon;


    private void Awake()
    {
        Health = MaxHealth;
    }
}