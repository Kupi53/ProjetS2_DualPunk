using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;


public class ThermicExchange : ImplantScript
{
    [SerializeField] protected float _damageConversionPercentage;

    private GameObject _oldModifiedWeapon;

    private HealthManager HealthManager { get => PlayerState.gameObject.GetComponent<HealthManager>(); }


    void Awake()
    {
        Type = ImplantType.ExoSqueleton;
        SetName = "Laser";
    }


    public override void Run()
    {
        if (IsEquipped)
        {
            HealthManager.DamageMultiplier = 1 - _damageConversionPercentage / 100;

            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                LaserGunScript laserGunScript = PlayerState.WeaponScript as LaserGunScript;

                if (laserGunScript != null && _oldModifiedWeapon != laserGunScript.gameObject)
                {
                    if (_oldModifiedWeapon != null)
                    {
                        HealthManager.DamageMultiplier = 1;
                        HealthManager.LaserGunScript = null;
                    }

                    _oldModifiedWeapon = laserGunScript.gameObject;
                    HealthManager.LaserGunScript = laserGunScript;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                HealthManager.DamageMultiplier = 1;
                HealthManager.LaserGunScript = null;

                _oldModifiedWeapon = null;
            }
        }
    }

    public override void ResetImplant()
    {
        HealthManager.DamageMultiplier = 1;
        HealthManager.LaserGunScript = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
