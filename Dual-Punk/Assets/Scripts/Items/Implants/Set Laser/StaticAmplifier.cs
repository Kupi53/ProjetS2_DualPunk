using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using GameKit.Utilities;


public class StaticAmplifier : ImplantScript
{
    [SerializeField] protected float _damageMultiplier;

    private LaserGunScript _oldModifiedWeapon;
    private int _oldDamage;
    private Color _oldColor;


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
                        _oldDamage = laserGunScript.Damage;
                        _oldColor = laserGunScript.Particles[0].gameObject.GetComponent<Renderer>().material.GetColor("_BaseColor");
                    }
                    else
                    {
                        _oldModifiedWeapon.Damage = _oldDamage;
                        ChangeColor(_oldModifiedWeapon.LineRenderer, _oldModifiedWeapon.Particles, _oldColor);
                        
                        _oldModifiedWeapon = laserGunScript;
                        _oldDamage = laserGunScript.Damage;
                        _oldColor = laserGunScript.Particles[0].gameObject.GetComponent<Renderer>().material.GetColor("_BaseColor");
                    }
                    
                    ChangeColor(laserGunScript.LineRenderer, laserGunScript.Particles, Color.red);
                    
                    float newDamage = laserGunScript.Damage * _damageMultiplier;
                    laserGunScript.Damage = (int)newDamage;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.Damage = _oldDamage;
                ChangeColor(_oldModifiedWeapon.LineRenderer, _oldModifiedWeapon.Particles, _oldColor);

                _oldModifiedWeapon = null;
            }
        }
    }

    private void ChangeColor(LineRenderer lineRenderer, List<ParticleSystem> particleSystems, Color color)
    {
        foreach (var particle in particleSystems)
        {
            particle.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
        }

        lineRenderer.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
    

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.Damage = _oldDamage;
            ChangeColor(_oldModifiedWeapon.LineRenderer, _oldModifiedWeapon.Particles, _oldColor);
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
