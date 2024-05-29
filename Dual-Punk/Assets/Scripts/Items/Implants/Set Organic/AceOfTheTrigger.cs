using UnityEngine;
using FishNet.Object;

public class AceOfTheTrigger : ImplantScript
{
    [SerializeField] protected float _speedFireRate;

    private GameObject _oldModifiedWeapon;
    private float _olfFireRate;

    void Awake()
    {
        Type = ImplantType.Neuralink;
        SetName = "Organic";
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
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = fireArmScript.gameObject;
                        _olfFireRate = fireArmScript.FireRate;
                    }
                    else
                    {
                        _oldModifiedWeapon.GetComponent<PowerWeaponScript>().FireRate = _olfFireRate;

                        _oldModifiedWeapon = fireArmScript.gameObject;
                        _olfFireRate = fireArmScript.FireRate;
                    }

                    fireArmScript.FireRate /= _speedFireRate;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.GetComponent<PowerWeaponScript>().FireRate = _olfFireRate;

                _oldModifiedWeapon = null;
            }
        }
    }


    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.GetComponent<PowerWeaponScript>().FireRate = _olfFireRate;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
