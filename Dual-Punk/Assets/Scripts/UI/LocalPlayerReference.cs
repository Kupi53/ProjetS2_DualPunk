using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalPlayerReference : MonoBehaviour
{
    // set in spawnui
    public GameObject LOCALPLAYER { get;  set; }
    public GameObject Camera { get; set; }
    public PlayerState PlayerState { get; set; }

    #nullable enable
    public FireArmScript? FireArmScript { get; private set; }
    public KnifeScript? KnifeScript { get; private set; }
    #nullable disable


    private void Start()
    {
        PlayerState = LOCALPLAYER.GetComponentInChildren<PlayerState>();
    }

    private void Update()
    {
        if (PlayerState.HoldingWeapon)
        {
            FireArmScript = PlayerState.Weapon.GetComponent<FireArmScript>();
            KnifeScript = PlayerState.Weapon.GetComponent<KnifeScript>();
        }
    }
}