using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using FishNet.Object;

public class Bulletstorm : ImplantScript
{   
    [SerializeField] protected int _dodgePercentage;
    [SerializeField] protected int _dodgePercentageWithMeleeWeapon;

    private HealthManager HealthManager { get => PlayerState.gameObject.GetComponent<HealthManager>(); }

    void Awake()
    {
        Type = ImplantType.ExoSqueleton;
        SetName = "Scavenger";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.WeaponScript as MeleeWeaponScript)
            {
                HealthManager.DodgePercentage = _dodgePercentageWithMeleeWeapon;
                HealthManager.DodgeActive = true;
            }
            else
            {
                HealthManager.DodgePercentage = _dodgePercentage;
                HealthManager.DodgeActive = true;
            }
        }
        else
        {
            HealthManager.DodgeActive = false;
        }
    }

    public override void ResetImplant()
    {
        HealthManager.DodgeActive = false;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}