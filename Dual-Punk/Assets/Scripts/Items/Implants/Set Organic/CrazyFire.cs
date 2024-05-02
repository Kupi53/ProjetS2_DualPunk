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

    private GameObject _holdModifiedWeapon;

    void Awake()
    {
        Type = ImplantType.Arm;
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
                        _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime *= _speedReloadTime;

                        _holdModifiedWeapon = fireArmScript.gameObject;
                    }

                    fireArmScript.ReloadTime /= _speedReloadTime;
                }
            }
            else if (_holdModifiedWeapon != null)
            {
                _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime *= _speedReloadTime;

                _holdModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_holdModifiedWeapon != null)
        {
            _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime *= _speedReloadTime;
        }

        _holdModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
