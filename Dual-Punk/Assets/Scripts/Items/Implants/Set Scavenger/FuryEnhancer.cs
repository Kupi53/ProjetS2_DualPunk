﻿using UnityEngine;
using FishNet.Object;

public class FuryEnhancer : ImplantScript
{
    [SerializeField] protected float _speedAttackRate;
    [SerializeField] protected float _multiplierDamage;
    private GameObject _oldModifiedWeapon;
    private int _oldDamage;
    
    void Awake()
    {
        Type = ImplantType.Arm;
        SetName = "Scavenger";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            if (PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                MeleeWeaponScript meleeWeaponScript = PlayerState.WeaponScript as MeleeWeaponScript;

                if (meleeWeaponScript != null && _oldModifiedWeapon != meleeWeaponScript.gameObject)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = meleeWeaponScript.gameObject;
                        _oldDamage = meleeWeaponScript.Damage;
                    }
                    else
                    {
                        MeleeWeaponScript oldMeleeWeaponScript = _oldModifiedWeapon.GetComponent<MeleeWeaponScript>();
                        oldMeleeWeaponScript.Damage = _oldDamage;
                        oldMeleeWeaponScript.AttackSpeed /= _speedAttackRate;
                        oldMeleeWeaponScript.ResetColdown *= _speedAttackRate;

                        _oldModifiedWeapon = meleeWeaponScript.gameObject;
                        _oldDamage = meleeWeaponScript.Damage;
                    }

                    float newDamage = meleeWeaponScript.Damage * _multiplierDamage;
                    
                    meleeWeaponScript.Damage = (int)newDamage;
                    meleeWeaponScript.AttackSpeed *= _speedAttackRate;
                    meleeWeaponScript.ResetColdown /= _speedAttackRate;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                MeleeWeaponScript oldMeleeWeaponScript = _oldModifiedWeapon.GetComponent<MeleeWeaponScript>();
                oldMeleeWeaponScript.Damage = _oldDamage;
                oldMeleeWeaponScript.AttackSpeed /= _speedAttackRate;
                oldMeleeWeaponScript.ResetColdown *= _speedAttackRate;

                _oldModifiedWeapon = null;
            }
        }
    }

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            MeleeWeaponScript oldMeleeWeaponScript = _oldModifiedWeapon.GetComponent<MeleeWeaponScript>();
            oldMeleeWeaponScript.Damage = _oldDamage;
            oldMeleeWeaponScript.AttackSpeed /= _speedAttackRate;
            oldMeleeWeaponScript.ResetColdown *= _speedAttackRate;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}