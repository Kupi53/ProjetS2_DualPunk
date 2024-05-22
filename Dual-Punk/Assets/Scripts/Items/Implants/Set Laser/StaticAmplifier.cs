using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


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
                        _oldColor = laserGunScript.Particles[0].main.startColor.color;
                    }
                    else
                    {
                        _oldModifiedWeapon.Damage = _oldDamage;
                        ChangeColorParticle(_oldModifiedWeapon.Particles, _oldColor);
                        ChangeColorLaser(_oldModifiedWeapon.LineRenderer, _oldColor);
                    

                        _oldModifiedWeapon = laserGunScript;
                        _oldDamage = laserGunScript.Damage;
                        _oldColor = laserGunScript.Particles[0].main.startColor.color;
                    }

                    float newDamage = laserGunScript.Damage * _damageMultiplier;
                    ChangeColorParticle(laserGunScript.Particles, Color.red);
                    ChangeColorLaser(laserGunScript.LineRenderer, Color.red);
                    laserGunScript.Damage = (int)newDamage;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.Damage = _oldDamage;
                ChangeColorParticle(_oldModifiedWeapon.Particles, _oldColor);
                ChangeColorLaser(_oldModifiedWeapon.LineRenderer, _oldColor);

                _oldModifiedWeapon = null;
            }
        }
    }

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.Damage = _oldDamage;
            ChangeColorParticle(_oldModifiedWeapon.Particles, _oldColor);
            ChangeColorLaser(_oldModifiedWeapon.LineRenderer, _oldColor);
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    private void ChangeColorParticle(List<ParticleSystem> particles, Color color)
    {
        foreach (ParticleSystem particle in particles)
        {
            Material material = particle.gameObject.GetComponent<Renderer>().material;
            material.SetColor("_Color", color);
        }
    }

    private void ChangeColorLaser(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
