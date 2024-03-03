using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//Ce script va gerer les attaques, des implants ou des armes

public class AttacksController : MonoBehaviour
{
    private Vector3 direction;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (playerState.HoldingWeapon)
        {
            direction = (playerState.Pointer.transform.position - transform.position - weaponScript.WeaponOffset).normalized;

            weaponScript.Run(transform.position, direction);

            if (Input.GetButtonDown("Drop"))
            {
                weaponScript.Reset();
                weaponScript.InHand = false;
                playerState.HoldingWeapon = false;
                weaponScript.PointerScript.locked = false;
                weaponScript.PointerScript.spriteNumber = 0;
            }
        }

        //Quand le joueur tient une arme blanche
        else if (playerState.HoldingKnife)
            knifeScript.Run(transform.position, direction);


        if (!knifeScript.Attacking)
        {
            direction = (playerState.Pointer.transform.position - transform.position - knifeScript.weaponOffset).normalized;

            if (Input.GetButtonDown("Drop"))
            {
                knifeScript.ResetAttack();
                knifeScript.inHand = false;
                playerState.HoldingKnife = false;
                knifeScript.pointerScript.spriteNumber = 0;
            }
        }
        knifeScript.Run(transform.position, direction);
    }
}