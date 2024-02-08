using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    // set in spawnui
    public GameObject LOCALPLAYER;
    public GameObject Camera;
    public PlayerState playerState;

    #nullable enable
    public WeaponScript? weaponScript;
    public KnifeScript? knifeScript;
    
    #nullable disable

    private void Start()
    {
        playerState = LOCALPLAYER.GetComponentInChildren<PlayerState>();
    }

    private void Update()
    {
        if (playerState.HoldingWeapon)
        {
            weaponScript = playerState.Weapon.GetComponent<WeaponScript>();
        }
        else if (playerState.HoldingKnife)
        {
            knifeScript = playerState.Weapon.GetComponent<KnifeScript>();
        }
    }
}