using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponContoller : MonoBehaviour
{
    SpriteRenderer sr;
    public bool onGround = true;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnPickup(){
    
    }

// lorsqu'on joueur collide avec le weapon
    void OnTriggerStay2D(Collider2D collisionInfo){
        // si sur le sol
        if (onGround){
            if (Input.GetButtonDown("Pickup")){
                GameObject Player = collisionInfo.gameObject;
                Player.GetComponent<Player>().HoldingWeapon = true;
                sr.transform.parent = Player.transform; 
                onGround = false;
            }
        }
        // si dans les mains d'un joueur
        else {
            if (Input.GetButtonDown("Pickup")){
                GameObject Player = collisionInfo.gameObject;
                Player.GetComponent<Player>().HoldingWeapon = false;
                sr.transform.parent = null;
                onGround = true;
            }
        }
    }
}
