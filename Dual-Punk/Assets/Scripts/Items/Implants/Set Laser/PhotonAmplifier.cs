using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;


public class PhotonAmplifier : ImplantScript
{
    [SerializeField] protected float _multiplicatorLaserTime;

    private GameObject _oldModifiedWeapon;


    void Awake()
    {
        Type = ImplantType.Arm;
        SetName = "Laser";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                LaserGunScript laserGunScript = PlayerState.WeaponScript as LaserGunScript;

                if (laserGunScript != null && _oldModifiedWeapon != laserGunScript.gameObject)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = laserGunScript.gameObject;
                    }
                    else
                    {
                        _oldModifiedWeapon.GetComponent<LaserGunScript>().FireTime /= _multiplicatorLaserTime;

                        _oldModifiedWeapon = laserGunScript.gameObject;
                    }

                    laserGunScript.FireTime *= _multiplicatorLaserTime;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.GetComponent<LaserGunScript>().FireTime /= _multiplicatorLaserTime;

                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.GetComponent<LaserGunScript>().FireTime /= _multiplicatorLaserTime;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
