using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;


public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private GameObject _ammoInMagObject;
    [SerializeField] private GameObject _weaponNameObject;
    [SerializeField] private GameObject _meleeWeaponIcon;
    [SerializeField] private GameObject _laserGunIcon;
    [SerializeField] private GameObject _weaponIcon;

    private LocalPlayerReference _references;
    private TextMeshProUGUI _ammoInMag;
    private TextMeshProUGUI _weaponName;
    private Image _meleeWeaponImage;
    private Image _laserGunImage;
    private Image _weaponImage;

#nullable enable
    private Sprite? _currentWeaponSprite;
#nullable disable


    void Start()
    {
        _ammoInMag = _ammoInMagObject.GetComponent<TextMeshProUGUI>();
        _weaponName = _weaponNameObject.GetComponent<TextMeshProUGUI>();
        _weaponImage = _weaponIcon.GetComponent<Image>();
        _meleeWeaponImage = _meleeWeaponIcon.GetComponent<Image>();
        _laserGunImage = _laserGunIcon.GetComponent<Image>();
        _references = transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }


    void Update()
    {
        _ammoInMag.enabled = false;
        _meleeWeaponImage.enabled = false;
        _laserGunImage.enabled = false;

        if (!_references.PlayerState.HoldingWeapon)
        {
            _weaponImage.enabled = false;
            _weaponName.text = "NO WEAPON";
            return;
        }

        _weaponName.enabled = true;
        _weaponImage.enabled = true;

        _weaponName.text = _references.PlayerState.WeaponScript.gameObject.name;

        Sprite _newWeaponSprite = _references.PlayerState.WeaponScript.gameObject.GetComponent<SpriteRenderer>().sprite;
        if (_newWeaponSprite != _currentWeaponSprite)
        {
            _currentWeaponSprite = _newWeaponSprite;
            float scale = 190 / _currentWeaponSprite.rect.width * 1.25f;
            _weaponImage.rectTransform.localScale = new Vector2(- scale, scale);
            _weaponImage.rectTransform.sizeDelta = new Vector2(_currentWeaponSprite.rect.width, _currentWeaponSprite.rect.height);
            _weaponImage.sprite = _currentWeaponSprite;
        }


        if (_references.PlayerState.WeaponScript is FireArmScript)
        {
            _ammoInMag.enabled = true;

            FireArmScript fireArmScript = (FireArmScript)_references.PlayerState.WeaponScript;

            if (fireArmScript.AmmoLeft < 10)
                _ammoInMag.text = "0" + fireArmScript.AmmoLeft.ToString();
            else
                _ammoInMag.text = fireArmScript.AmmoLeft.ToString();
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