using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.VisualScripting;


public class WeaponController : NetworkBehaviour
{
    private PlayerState playerState;
    private List<GameObject> weapons;
    private WeaponScript? weaponScript;
    private KnifeScript? knifeScript;
    private GameObject? Weapon;

    private int index;
    private Vector3 direction;


    private void Start()
    {
        index = 0;
        weapons = new List<GameObject>();
        playerState = gameObject.GetComponent<PlayerState>();
    }


    private void Update()
    {
        if (weapons.Count > 0 && !playerState.HoldingWeapon && !playerState.HoldingKnife)
        {
            if (Input.GetButtonDown("Switch"))
                index = (index + 1) % weapons.Count;

            Weapon = weapons[index];
            Weapon.GetComponent<HighlightWeapon>().Highlight();

            if (Input.GetButtonDown("Pickup"))
            {
                index = 0;

                if (Weapon.CompareTag("Weapon"))
                {
                    playerState.Weapon = Weapon;
                    playerState.HoldingWeapon = true;
                    weaponScript = Weapon.GetComponent<WeaponScript>();
                    weaponScript.inHand = true;
                    weaponScript.playerState = playerState;
                }
                else if (Weapon.CompareTag("Knife"))
                {
                    playerState.Weapon = Weapon;
                    playerState.HoldingKnife = true;
                    knifeScript = Weapon.GetComponent<KnifeScript>();
                }
            }
        }

        //Gere les scripts des armes
        //Quand le joueur tient une arme a feu
        if (playerState.HoldingWeapon)
        {
            direction = (playerState.Pointer.transform.position - transform.position - weaponScript.weaponOffset).normalized;

            weaponScript.Run(transform.position, direction);

            if (Input.GetButtonDown("Drop"))
            {
                weaponScript.ResetReload();
                weaponScript.inHand = false;
                playerState.HoldingWeapon = false;
                weaponScript.pointerScript.spriteNumber = 0;
            }
        }

        //Quand le joueur tient une arme blanche
        else if (playerState.HoldingKnife)
        {
            if (!knifeScript.attacking)
            {
                direction = (playerState.Pointer.transform.position - transform.position - knifeScript.weaponOffset).normalized;

                if (Input.GetButtonDown("Drop"))
                {
                    weaponScript.pointerScript.spriteNumber = 0;
                    playerState.HoldingKnife = false;
                    knifeScript.ResetAttack();
                }
            }

            if (playerState.HoldingKnife)
                knifeScript.Run(transform.position, direction);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife"))
        {
            weapons.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife"))
        {
            index = 0;
            weapons.Remove(collision.gameObject);
        }
    }
}