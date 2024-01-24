using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerReference : MonoBehaviour
{
    public GameObject LOCALPLAYER;
    public PlayerState playerState;
    private WeaponScript? weaponScript;

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
    }

}
