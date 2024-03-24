using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    // Set in spawnui
    public PlayerState PlayerState { get; set; }

    #nullable enable
    public FireArmScript? FireArmScript { get; private set; }
    public MeleeWeaponScript? MeleeWeaponScript { get; private set; }
    #nullable disable


    private void Update()
    {
        if (PlayerState.HoldingWeapon)
        {
            if (PlayerState.WeaponScript is FireArmScript)
            {
                FireArmScript = (FireArmScript)PlayerState.WeaponScript;
                MeleeWeaponScript = null;
            }
            else
            {
                FireArmScript = null;
                MeleeWeaponScript = (MeleeWeaponScript)PlayerState.WeaponScript;
            }
        }
    }
}