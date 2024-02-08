using System;
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

    #nullable enable
    public GameObject? Pointer;
    public GameObject? Weapon;
    
    #nullable disable


    private void Awake()
    {
        Health = MaxHealth;
    }

        void Update(){
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
        this.Health -= amount;

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