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
    public FireArmScript? fireArmScript;
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
            fireArmScript = playerState.Weapon.GetComponent<FireArmScript>();
            knifeScript = playerState.Weapon.GetComponent<KnifeScript>();
        }
    }
}