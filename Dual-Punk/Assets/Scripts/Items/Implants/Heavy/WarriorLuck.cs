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

    private GameObject _holdModifiedWeapon;

    void Awake()
    {
        Type = ImplantType.Neuralink;
        SetNumber = 2;
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                FireArmScript fireArmScript = PlayerState.WeaponScript as FireArmScript;

                if (fireArmScript != null && _holdModifiedWeapon != fireArmScript.gameObject)
                {
                    if (_holdModifiedWeapon == null)
                    {
                        _holdModifiedWeapon = fireArmScript.gameObject;
                    }
                    else
                    {
                        _holdModifiedWeapon.GetComponent<FireArmScript>().DropPercentage = 1;
                        _holdModifiedWeapon.GetComponent<FireArmScript>().DamageMultiplier = 1;
                        _holdModifiedWeapon.GetComponent<FireArmScript>().WarriorLuck = false;

                        _holdModifiedWeapon = fireArmScript.gameObject;
                    }

                    fireArmScript.DropPercentage = _dropPercentage;
                    fireArmScript.DamageMultiplier = _damageMultiplier;
                    fireArmScript.WarriorLuck = true;
                }
            }
            else if (_holdModifiedWeapon != null)
            {
                _holdModifiedWeapon.GetComponent<FireArmScript>().DropPercentage = 1;
                _holdModifiedWeapon.GetComponent<FireArmScript>().DamageMultiplier = 1;
                _holdModifiedWeapon.GetComponent<FireArmScript>().WarriorLuck = false;

                _holdModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_holdModifiedWeapon != null)
        {
            _holdModifiedWeapon.GetComponent<FireArmScript>().DropPercentage = 1;
            _holdModifiedWeapon.GetComponent<FireArmScript>().DamageMultiplier = 1;
            _holdModifiedWeapon.GetComponent<FireArmScript>().WarriorLuck = false;
        }

        _holdModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    
}
