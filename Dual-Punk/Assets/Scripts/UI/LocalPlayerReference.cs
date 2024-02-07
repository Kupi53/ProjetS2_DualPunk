using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    public GameObject LOCALPLAYER;
    public GameObject Camera;
    public PlayerState playerState;
    public WeaponScript? weaponScript;
    public KnifeScript? knifeScript;


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