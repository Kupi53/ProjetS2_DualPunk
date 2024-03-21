using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerState : MonoBehaviour
{
    private Vector3 _mousePosition;

    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public bool Walking { get; set; }
    public bool Dashing { get; set; }
    public bool Attacking { get; set; }
    public bool HoldingWeapon { get; set; }

    public float DashCooldown { get; set; }
    public float DashCooldownMax { get; set; }

    public Vector3 MousePosition { get => _mousePosition; }

    public Camera Camera { get; set; }

    #nullable enable
    public WeaponScript? WeaponScript { get; set; }
    public PointerScript? PointerScript { get; set; }
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
        _mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0;

        if (Input.GetKeyDown(KeyCode.V)){
            Damage(10);
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