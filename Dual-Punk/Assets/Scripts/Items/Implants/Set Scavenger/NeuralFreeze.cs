using UnityEngine;
using FishNet.Object;

public class NeuralFreeze : ImplantScript
{
    [SerializeField] protected int _stunDuration;

    private GameObject _oldModifiedWeapon;

    void Awake()
    {
        Type = ImplantType.Neuralink;
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
                        _oldModifiedWeapon = meleeWeaponScript.gameObject;
                    }

                    meleeWeaponScript.StunDuration = _stunDuration;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.gameObject.GetComponent<MeleeWeaponScript>().StunDuration = 0f;
                _oldModifiedWeapon = null;
            }
        }
    }

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.gameObject.GetComponent<MeleeWeaponScript>().StunDuration = 0f;
            _oldModifiedWeapon = null;
        }

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}