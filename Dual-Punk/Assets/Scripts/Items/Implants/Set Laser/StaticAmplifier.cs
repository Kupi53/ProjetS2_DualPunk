using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class StaticAmplifier : ImplantScript
{
    [SerializeField] protected float _damageMultiplier;

    private LaserGunScript _oldModifiedWeapon;


    void Awake()
    {
        Type = ImplantType.Boots;
        SetName = "Laser";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null && !PlayerState.Moving)
            {
                LaserGunScript laserGunScript = PlayerState.WeaponScript as LaserGunScript;

                if (laserGunScript != null && _oldModifiedWeapon != laserGunScript)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = laserGunScript;
                    }
                    else
                    {
                        _oldModifiedWeapon.ChangeLaser(false, 1);
                        _oldModifiedWeapon = laserGunScript;
                    }

                    laserGunScript.ChangeLaser(true, _damageMultiplier);
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.ChangeLaser(false, 1);
                _oldModifiedWeapon = null;
            }
        }
    }

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.ChangeLaser(false, 1);
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    /*private void ChangeColorParticle(List<ParticleSystem> particles, Color color)
    {
        foreach (ParticleSystem particle in particles)
        {
            Material material = particle.gameObject.GetComponent<Renderer>().material;
            material.SetColor("_Color", color);
        }
    }*/
}
