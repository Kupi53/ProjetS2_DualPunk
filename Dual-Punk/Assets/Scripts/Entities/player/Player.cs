using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// classe base pour les joueurs
public class Player : MonoBehaviour {

    [SerializeField]
    private int maxHealth = 100;
    public int[] Stats;
    public float Health;
    public bool HoldingWeapon {get;set;}

    void Awake(){
        Health = maxHealth;
        HoldingWeapon = false;
    }
}