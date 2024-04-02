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

    private LocalPlayerReference _references;
    private Image _image;

#nullable enable
    private FireArmScript? _fireArmScript;
#nullable disable


    void Start()
    {
        _image = _meleeWeaponIcon.GetComponent<Image>();
        _references = transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }


    void Update()
    {
        if (_references.PlayerState.HoldingWeapon && _references.PlayerState.WeaponScript is FireArmScript)
        {
            _fireArmScript = (FireArmScript)_references.PlayerState.WeaponScript;

            if (_fireArmScript.AmmoLeft < 10)
                _text.text = "0" + _fireArmScript.AmmoLeft.ToString();
            else
                _text.text = _fireArmScript.AmmoLeft.ToString();

            _text.enabled = true;
            _image.enabled = false;
        }
        else
        {
            _text.enabled = false;
            _image.enabled = true;
        }
    }
}