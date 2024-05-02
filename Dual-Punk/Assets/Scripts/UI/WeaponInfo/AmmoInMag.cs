using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;


public class AmmoInMag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _meleeWeaponIcon;
    [SerializeField] private GameObject _laserGunIcon;

    private LocalPlayerReference _references;
    private Image _meleeWeaponImage;
    private Image _laserGunImage;

#nullable enable
    private FireArmScript? _fireArmScript;
#nullable disable


    void Start()
    {
        _meleeWeaponImage = _meleeWeaponIcon.GetComponent<Image>();
        _laserGunImage = _laserGunIcon.GetComponent<Image>();
        _references = transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }


    void Update()
    {
        _text.enabled = false;
        _meleeWeaponImage.enabled = false;
        _laserGunImage.enabled = false;

        if (!_references.PlayerState.HoldingWeapon) return;

        if (_references.PlayerState.WeaponScript is FireArmScript)
        {
            _text.enabled = true;

            _fireArmScript = (FireArmScript)_references.PlayerState.WeaponScript;

            if (_fireArmScript.AmmoLeft < 10)
                _text.text = "0" + _fireArmScript.AmmoLeft.ToString();
            else
                _text.text = _fireArmScript.AmmoLeft.ToString();
        }
        else if (_references.PlayerState.WeaponScript is LaserGunScript)
        {
            _laserGunImage.enabled = true;
        }
        else
        {
            _meleeWeaponImage.enabled = true;
        }
    }
}