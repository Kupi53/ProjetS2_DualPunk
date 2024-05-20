using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ReloadCooldown : MonoBehaviour
{
    [SerializeField] private RawImage _image;

    private LocalPlayerReference _references;
    private RectTransform _rectTransform;
    private float _maxTop;
    private float _minTop;


    void Start()
    {
        _rectTransform = _image.rectTransform;
        _maxTop = -_rectTransform.offsetMax.y;
        _minTop = _rectTransform.offsetMin.y;
        _references = transform.root.gameObject.GetComponent<LocalPlayerReference>();
    }

    private void Update()
    {
        if (_references.PlayerState.HoldingWeapon && _references.PlayerState.WeaponScript.DisplayInfo)
        {
            _image.enabled = true;
            float multiplier = (_maxTop - _minTop) / _references.PlayerState.WeaponScript.InfoMaxTime;
            _rectTransform.offsetMax = new Vector2(_rectTransform.offsetMax.x, -_minTop - multiplier * _references.PlayerState.WeaponScript.InfoTimer);
        }
        else
        {
            _image.enabled = false;
        }
    }
}




/*if (_references.FireArmScript != null && _references.FireArmScript.Reloading)
{
    _image.enabled = true;
    float multiplier = (_maxTop - _minTop) / _references.FireArmScript.ReloadTime;
    _rectTransform.offsetMax = new Vector2(_rectTransform.offsetMax.x, -_minTop - multiplier * _references.FireArmScript.ReloadTimer);
}
else if (_references.MeleeWeaponScript != null && _references.MeleeWeaponScript.Attack != 0)
{
    _image.enabled = true;
    float multiplier = (_maxTop - _minTop) / _references.MeleeWeaponScript.ResetCooldown;
    _rectTransform.offsetMax = new Vector2(_rectTransform.offsetMax.x, -_minTop - multiplier * _references.MeleeWeaponScript.ResetCooldownTimer);
}*/
