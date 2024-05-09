using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class WarriorLuck : ImplantScript
{
    [SerializeField] protected int _damageMultiplier;
    [SerializeField] protected int _dropPercentage;

    private GameObject _oldModifiedWeapon;

    void Awake()
    {
        Type = ImplantType.Neuralink;
        SetName = "Heavy";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                FireArmScript fireArmScript = PlayerState.WeaponScript as FireArmScript;

                if (fireArmScript != null && _oldModifiedWeapon != fireArmScript.gameObject)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = fireArmScript.gameObject;
                    }
                    else
                    {
                        FireArmScript oldFireArmScript = _oldModifiedWeapon.GetComponent<FireArmScript>();
                        oldFireArmScript.DropPercentage = 1;
                        oldFireArmScript.DamageMultiplier = 1;
                        oldFireArmScript.WarriorLuck = false;

                        _oldModifiedWeapon = fireArmScript.gameObject;
                    }

                    fireArmScript.DropPercentage = _dropPercentage;
                    fireArmScript.DamageMultiplier = _damageMultiplier;
                    fireArmScript.WarriorLuck = true;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                FireArmScript oldFireArmScript = _oldModifiedWeapon.GetComponent<FireArmScript>();
                oldFireArmScript.DropPercentage = 1;
                oldFireArmScript.DamageMultiplier = 1;
                oldFireArmScript.WarriorLuck = false;

                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            FireArmScript oldFireArmScript = _oldModifiedWeapon.GetComponent<FireArmScript>();
            oldFireArmScript.DropPercentage = 1;
            oldFireArmScript.DamageMultiplier = 1;
            oldFireArmScript.WarriorLuck = false;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    
}
