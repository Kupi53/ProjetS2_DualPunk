using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    // set in spawnui
    public GameObject LOCALPLAYER;
    public PlayerState playerState;
    public WeaponScript? weaponScript;
    public KnifeScript? knifeScript;


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