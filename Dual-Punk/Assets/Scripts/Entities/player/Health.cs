using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// classe base pour les joueurs
public class Player : MonoBehaviour {

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int damage = 10;
    public int[] Stats;
    [SerializeField]
    public float Health;
    public bool HoldingWeapon {get;set;}

    void Start(){
        Health = maxHealth;
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            Damage(damage);
        }
        if(Input.GetKeyDown(KeyCode.V)){
            Heal(10);
        }
    }
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        Health = maxHealth;
        HoldingWeapon = false;
    }

    void Die(){
        Debug.Log("you dead man");
        Destroy(gameObject);
    }

    public void Damage(int amount){
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
        if (Health + amount > maxHealth){
            this.Health = this.maxHealth;
        }
        else
            this.Health += amount;
    }
}