using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class FastReload : ImplantScript
{
    [SerializeField] protected float _speedReloadTime;

    private GameObject _holdModifiedWeapon;
    private float _holdReloadTime;

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
                        _holdReloadTime = fireArmScript.ReloadTime;
                    }
                    else
                    {
                        _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime = _holdReloadTime;

                        _holdModifiedWeapon = fireArmScript.gameObject;
                        _holdReloadTime = fireArmScript.ReloadTime;
                    }

                    fireArmScript.ReloadTime /= _speedReloadTime;
                }
            }
            else if (_holdModifiedWeapon != null)
            {
                _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime = _holdReloadTime;

                _holdModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_holdModifiedWeapon != null)
        {
            _holdModifiedWeapon.GetComponent<FireArmScript>().ReloadTime = _holdReloadTime;
        }

        _holdModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
