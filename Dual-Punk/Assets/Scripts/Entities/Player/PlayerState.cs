using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerState : MonoBehaviour
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public bool Walking { get; set; }
    public bool Dashing { get; set; }
    public bool Attacking { get; set; }
    public bool HoldingWeapon { get; set; }

    public float DashCooldown { get; set; }
    public float DashCooldownMax { get; set; }

    #nullable enable
    public GameObject? Pointer { get; set; }
    public GameObject? Weapon { get; set; }
    #nullable disable


    private void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        Walking = false;
        Dashing = false;
        Attacking = false;
        HoldingWeapon = false;
        DashCooldown = 0.0f;
        DashCooldownMax = 1.0f;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            Damage(10);
        }
        if(Input.GetKeyDown(KeyCode.V)){
            Heal(10);
        }
    }

    void Die(){
        Debug.Log("you dead man");
        Destroy(gameObject);
    }

    public void Damage(float amount){
        if (amount < 0){
            throw new ArgumentOutOfRangeException("Cannot have Negative damage.");
        }
        Health -= (int)amount;

        if (Health <= 0){
            Die();
        }
    }

    public void Heal(int amount){
        if (amount < 0){
            throw new ArgumentOutOfRangeException("Cannot have Negative Healing.");
        }
        if (Health + amount > MaxHealth){
            Health = MaxHealth;
        }
        else
            Health += amount;
    }
}