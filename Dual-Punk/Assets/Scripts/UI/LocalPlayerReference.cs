using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    public GameObject LOCALPLAYER;
    public PlayerState playerState;

    #nullable enable
    public WeaponScript? weaponScript;
    public KnifeScript? knifeScript;
    
    #nullable disable

    private void Start()
    {
        playerState = LOCALPLAYER.gameObject.GetComponent<PlayerState>();
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