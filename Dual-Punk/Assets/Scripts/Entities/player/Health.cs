using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// classe base pour les joueurs
public class Health : MonoBehaviour {

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int damage = 10;
    public int[] Stats;
    [SerializeField]
    public float health;

    void Start(){
        health = maxHealth;
    }
    void Update(){
        
    }
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        health = maxHealth;
    }

    void Die(){
        Debug.Log("you dead man");
        Destroy(gameObject);
    }

    public void Damage(int amount){
        if (amount < 0){
            throw new ArgumentOutOfRangeException("Cannot have Negative damage.");
        }
        this.health -= amount;

        if (health <= 0){
            Die();
        }
    }

    public void Heal(int amount){
        if (amount < 0){
            throw new ArgumentOutOfRangeException("Cannot have Negative Healing.");
        }
        if (health + amount > maxHealth){
            this.health = this.maxHealth;
        }
        else
            this.health += amount;
    }

}