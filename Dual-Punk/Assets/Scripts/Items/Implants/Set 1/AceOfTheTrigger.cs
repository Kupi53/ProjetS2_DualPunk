using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class AceOfTheTrigger : ImplantScript
{
    [SerializeField] protected float _speedFireRate;

    private GameObject _holdModifiedWeapon;
    private float _holfFireRate;

    void Awake()
    {
        Type = ImplantType.Neuralink;
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
                        _holfFireRate = fireArmScript.FireRate;
                    }
                    else
                    {
                        _holdModifiedWeapon.GetComponent<FireArmScript>().FireRate = _holfFireRate;

                        _holdModifiedWeapon = fireArmScript.gameObject;
                        _holfFireRate = fireArmScript.FireRate;
                    }

                    fireArmScript.FireRate /= _speedFireRate;
                }
            }
            else if (_holdModifiedWeapon != null)
            {
                _holdModifiedWeapon.GetComponent<FireArmScript>().FireRate = _holfFireRate;

                _holdModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_holdModifiedWeapon != null)
        {
            _holdModifiedWeapon.GetComponent<FireArmScript>().FireRate = _holfFireRate;
        }

        _holdModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
