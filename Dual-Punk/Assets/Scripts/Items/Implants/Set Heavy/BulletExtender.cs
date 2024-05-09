using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class BulletExtender : ImplantScript
{
    [SerializeField] private float _ammoMultiplier;

    private GameObject _oldModifiedWeapon;
    private int _oldMagSize;

    void Awake()
    {
        Type = ImplantType.Arm;
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
                        oldFireArmScript.MagSize = _oldMagSize;
                        oldFireArmScript.ReloadAmout = _oldMagSize;

                        _oldModifiedWeapon = fireArmScript.gameObject;
                    }

                    _oldMagSize = fireArmScript.MagSize;
                    int newMagSize = (int)(fireArmScript.MagSize * _ammoMultiplier);

                    fireArmScript.MagSize = newMagSize;
                    fireArmScript.ReloadAmout = newMagSize;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                FireArmScript oldFireArmScript = _oldModifiedWeapon.GetComponent<FireArmScript>();
                oldFireArmScript.MagSize = _oldMagSize;
                oldFireArmScript.ReloadAmout = _oldMagSize;

                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            FireArmScript oldFireArmScript = _oldModifiedWeapon.GetComponent<FireArmScript>();
            oldFireArmScript.MagSize = _oldMagSize;
            oldFireArmScript.ReloadAmout = _oldMagSize;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
