using UnityEngine;
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
                    if (_oldModifiedWeapon != null)
                    {
                        ResetEffect();
                    }

                    _oldModifiedWeapon = meleeWeaponScript.gameObject;
                    _oldDamage = meleeWeaponScript.Damage;

                    float newDamage = meleeWeaponScript.Damage * _multiplierDamage;
                    
                    meleeWeaponScript.Damage = (int)newDamage;
                    meleeWeaponScript.AttackCooldown *= _speedAttackRate;
                    meleeWeaponScript.ResetCooldown /= _speedAttackRate;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                ResetEffect();
            }
        }
    }


    private void ResetEffect()
    {
        MeleeWeaponScript oldMeleeWeaponScript = _oldModifiedWeapon.GetComponent<MeleeWeaponScript>();
        oldMeleeWeaponScript.Damage = _oldDamage;
        oldMeleeWeaponScript.AttackCooldown /= _speedAttackRate;
        oldMeleeWeaponScript.ResetCooldown *= _speedAttackRate;
        _oldModifiedWeapon = null;
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            ResetEffect();
        }
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}