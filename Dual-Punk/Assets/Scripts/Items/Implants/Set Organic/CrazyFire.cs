using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class CrazyFire : ImplantScript
{
    [SerializeField] protected float _speedReloadTime;

    private GameObject _oldModifiedWeapon;

    void Awake()
    {
        Type = ImplantType.Arm;
        SetName = "Organic";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                PowerWeaponScript fireArmScript = PlayerState.WeaponScript as PowerWeaponScript;

                if (fireArmScript != null && _oldModifiedWeapon != fireArmScript.gameObject)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = fireArmScript.gameObject;
                    }
                    else
                    {
                        _oldModifiedWeapon.GetComponent<PowerWeaponScript>().ReloadTime *= _speedReloadTime;

                        _oldModifiedWeapon = fireArmScript.gameObject;
                    }

                    fireArmScript.ReloadTime /= _speedReloadTime;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.GetComponent<PowerWeaponScript>().ReloadTime *= _speedReloadTime;

                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.GetComponent<PowerWeaponScript>().ReloadTime *= _speedReloadTime;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
