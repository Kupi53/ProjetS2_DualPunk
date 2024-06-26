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
    private int _oldReloadAmount;


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
                PowerWeaponScript fireArmScript = PlayerState.WeaponScript as PowerWeaponScript;

                if (fireArmScript != null && _oldModifiedWeapon != fireArmScript.gameObject)
                {
                    if (_oldModifiedWeapon != null)
                    {
                        ResetOldWeapon();
                    }

                    _oldModifiedWeapon = fireArmScript.gameObject;
                    _oldMagSize = fireArmScript.MagSize;
                    _oldReloadAmount = fireArmScript.ReloadAmout;

                    fireArmScript.MagSize = (int)(fireArmScript.MagSize * _ammoMultiplier);
                    fireArmScript.ReloadAmout = (int)(fireArmScript.ReloadAmout * _ammoMultiplier) + 1;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                ResetOldWeapon();
                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            ResetOldWeapon();
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }


    private void ResetOldWeapon()
    {
        PowerWeaponScript oldFireArmScript = _oldModifiedWeapon.GetComponent<PowerWeaponScript>();
        oldFireArmScript.MagSize = _oldMagSize;
        oldFireArmScript.ReloadAmout = _oldReloadAmount;

        if (oldFireArmScript.AmmoLeft > oldFireArmScript.MagSize)
        {
            oldFireArmScript.AmmoLeft = oldFireArmScript.MagSize;
        }
    }
}
