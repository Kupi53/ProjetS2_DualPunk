using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImplantController : MonoBehaviour
{
    #nullable enable
    public ImplantScript? NeuralinkImplant {get;set;}
    public ImplantScript? ExoSqueletonImplant {get;set;}
    public ImplantScript? ArmImplant {get;set;}
    public ImplantScript? BootsImplant {get;set;}
    private ImplantScript?[] _implants 
    {
        get
        {
            ImplantScript?[] implants = new ImplantScript?[4];
            implants[0] = NeuralinkImplant;
            implants[1] = ExoSqueletonImplant;
            implants[2] = ArmImplant;
            implants[3] = BootsImplant;
            return implants;
        }
    }
#nullable disable


    void Update()
    {
        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                implant.Run();
            }
        }

        (bool, string) setIsEquipped = SetIsEquipped();

        if (setIsEquipped.Item1)
        {
            switch (setIsEquipped.Item2)
            {
                case "Organic":
                    SetOrganic();
                    break;
                case "Heavy":
                    SetHeavy();
                    break;
                case "Laser":
                    SetLaser();
                    break;
                case "Scavenger":
                    SetScavenger();
                    break;
            }
        }
        else if (_setActive != "")
        {
            switch (_setActive)
            {
                case "Organic":
                    ResetSetOrganic();
                    break;
                case "Heavy":
                    ResetSetHeavy();
                    break;
                case "Laser":
                    ResetSetLaser();
                    break;
                case "Scavenger":
                    ResetSetScavenger();
                    break;
            }
        }
    }

    public (bool, string) SetIsEquipped()
    {
        (bool, string) result = (false, "");

        int implantNumber = 0;

        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                if (result.Item2 == "")
                {
                    result.Item1 = true;
                    result.Item2 = implant.SetName;
                }
                else if (result.Item2 != implant.SetName)
                {
                    return (false, "");
                }

                implantNumber++;
            }
        }

        if (implantNumber != 4)
        {
            return (false, "");
        }

        return result;
    }

    private string _setActive = "";

    [SerializeField] private int _lifestealPercentage;
    public float LifestealMultiplier { get => (float)_lifestealPercentage / 100; }

    private void SetOrganic(){}
    private void ResetSetOrganic(){}


    [SerializeField] private float _lessDamageMultipler;
    [SerializeField] private float _walkSpeedMultiplier;

    public float LessDamageMultiplier { get => (float)_lessDamageMultipler; }

    private void SetHeavy()
    {
        if (_setActive == "")
        {
            MouvementsController mouvementController = gameObject.GetComponent<MouvementsController>();
            mouvementController.WalkSpeed *= _walkSpeedMultiplier;
            mouvementController.SprintSpeed *=_walkSpeedMultiplier;
            _setActive = "Heavy";
        }
    }
    private void ResetSetHeavy()
    {
        MouvementsController mouvementController = gameObject.GetComponent<MouvementsController>();
        mouvementController.WalkSpeed /= _walkSpeedMultiplier;
        mouvementController.SprintSpeed /=_walkSpeedMultiplier;
        _setActive = "";
    }

    [SerializeField] private float _rangeDamage;
    public float RangeDamage { get => _rangeDamage; }
    private GameObject _oldModifiedLaser;
    private void SetLaser()
    {
        PlayerState playerState = gameObject.GetComponent<PlayerState>();

        if (playerState.HoldingWeapon && playerState.WeaponScript != null)
        {
            LaserGunScript laserGunScript = playerState.WeaponScript as LaserGunScript;

            if (laserGunScript != null && _oldModifiedLaser != laserGunScript)
            {
                if (_oldModifiedLaser != null)
                {
                    _oldModifiedLaser.GetComponent<LaserGunScript>().SetIsActive = false;
                }

                _oldModifiedLaser = laserGunScript.gameObject;
                laserGunScript.GetComponent<LaserGunScript>().SetIsActive = true;
            }
        }
        else if (_oldModifiedLaser != null)
        {
            _oldModifiedLaser.GetComponent<LaserGunScript>().SetIsActive = false;
            _oldModifiedLaser = null;
        }

        _setActive = "Laser";
    }
    private void ResetSetLaser()
    {
        if (_oldModifiedLaser != null)
        {
            _oldModifiedLaser.GetComponent<LaserGunScript>().SetIsActive = false;
            _oldModifiedLaser = null;
        }

        _setActive = "";
    }


    [SerializeField] private int _criticalPercentage;
    private GameObject _oldModifiedMelee;
    private void SetScavenger()
    {
        PlayerState playerState = gameObject.GetComponent<PlayerState>();

        if (playerState.HoldingWeapon && playerState.WeaponScript != null)
        {
            MeleeWeaponScript meleeWeaponScript = playerState.WeaponScript as MeleeWeaponScript;

            if (meleeWeaponScript != null && _oldModifiedMelee != meleeWeaponScript)
            {
                if (_oldModifiedMelee != null)
                {
                    _oldModifiedMelee.GetComponent<MeleeWeaponScript>().SetIsActive = false;
                }

                _oldModifiedMelee = meleeWeaponScript.gameObject;
                
                MeleeWeaponScript weapon = meleeWeaponScript.GetComponent<MeleeWeaponScript>();
                weapon.CriticalPercentage = _criticalPercentage;
                weapon.SetIsActive = true;
            }
        }
        else if (_oldModifiedMelee != null)
        {
            _oldModifiedMelee.GetComponent<MeleeWeaponScript>().SetIsActive = false;
            _oldModifiedMelee = null;
        }

        _setActive = "Scavenger";
    }
    private void ResetSetScavenger()
    {
        if (_oldModifiedMelee != null)
        {
            _oldModifiedMelee.GetComponent<MeleeWeaponScript>().SetIsActive = false;
            _oldModifiedMelee = null;
        }

        _setActive = "";
    }
}
