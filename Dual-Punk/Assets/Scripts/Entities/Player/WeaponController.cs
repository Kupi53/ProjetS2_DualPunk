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
    private float angle;
    private Vector3 direction;
    private Vector3 mousePos;


    private void Start()
    {
        index = 0;
        weapons = new List<GameObject>();
        playerState = gameObject.GetComponent<PlayerState>();
    }


    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (weapons.Count > 0 && !playerState.HoldingWeapon && !playerState.HoldingKnife)
        {
            if (Input.GetButtonDown("Switch"))
                index = (index + 1) % weapons.Count;

            if (Input.GetButtonDown("Pickup"))
            {
                Weapon = weapons[index];
                playerState.Weapon = Weapon;
                index = 0;

                if (Weapon.CompareTag("Weapon"))
                {
                    playerState.HoldingWeapon = true;
                    weaponScript = Weapon.GetComponent<WeaponScript>();
                }
                else if (Weapon.CompareTag("Knife"))
                {
                    playerState.HoldingKnife = true;
                    knifeScript = Weapon.GetComponent<KnifeScript>();
                }
            }
        }

        //Gere les scripts des armes
        //Quand le joueur tient une arme a feu
        if (playerState.HoldingWeapon)
        {
            direction = (mousePos - transform.position - weaponScript.weaponOffset).normalized;
            angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            weaponScript.Run(transform.position, direction, angle, playerState.Walking);

            if (Input.GetButtonDown("Drop"))
            {
                playerState.HoldingWeapon = false;
                weaponScript.ResetReload();
            }
        }

        //Quand le joueur tient une arme blanche
        else if (playerState.HoldingKnife)
        {
            if (!knifeScript.attacking)
            {
                direction = (mousePos - transform.position - knifeScript.weaponOffset).normalized;

                if (Input.GetButtonDown("Drop"))
                {
                    playerState.HoldingKnife = false;
                    knifeScript.ResetAttack();
                }
            }

            if (playerState.HoldingKnife)
                knifeScript.Run(transform.position, direction);
        }

        if (weapons.Count > 0 && !playerState.HoldingWeapon && !playerState.HoldingKnife)
            weapons[index].GetComponent<HighlightWeapon>().Highlight();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife"))
        {
            weapons.Add(collision.gameObject);
            index %= weapons.Count;
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