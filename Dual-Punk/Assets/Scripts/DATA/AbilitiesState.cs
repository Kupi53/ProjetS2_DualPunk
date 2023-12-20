using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;


/* track l'état et les attributs de toutes les abilités 
Deviendra peut etre une classe ability base avec des classes enfant de chaque abilité*/

public class AbilitiesState : MonoBehaviour
{
    public static AbilitiesState Instance;
    public bool dashing = false;
    public float dashCooldown = 0.0f;
    public float dashCooldownMax = 4.0f;
    public float dashTimer = 0.0f;

    void Awake(){
        /* pattern "Singleton" càd une seule instance de la classe qui est utilisée pour communiquer
        entre script et garder des infos */
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } 
        else { 
            Instance = this; 
        } 
    }
}
