using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EnergyBladeScript : MeleeWeaponScript
{
    [SerializeField] private float _attackSpeed;

    private float _vel;
    private float _angle;
    private float _targetAngle;
    private float _currentWeaponDistance;


    private new void Update()
    {
        base.Update();

        _angle = Mathf.SmoothDamp(_angle, _targetAngle, ref _vel, _attackSpeed);
    }


    protected override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(WeaponOffset.x))
        {
            WeaponOffset = new Vector3(-WeaponOffset.x, WeaponOffset.y, 0);
        }
        if (_attack == 0 && Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            _targetAngle = -_targetAngle;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 0);
        }

        if (_defenceTimer > 0 && !_defenceCooldown)
            transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        else
            transform.position = position + WeaponOffset + Quaternion.Euler(0, 0, _angle) * direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(Quaternion.Euler(0, 0, _angle) * direction));
    }

    protected override void ResetDefence()
    {
        _defenceCooldown = true;
        _currentWeaponDistance = _weaponDistance;
        _targetAngle = Math.Sign(transform.localScale.y) * _swipeRange;
        WeaponOffset = _weaponOffset;
    }


    protected override void Defend(Vector3 direction)
    {
        base.Defend(direction);

        if (_currentWeaponDistance != _defenceWeaponDistance) // si oui alors on a pas encore set la bonne pos de l'arme
        {
            _targetAngle = Math.Sign(transform.localScale.y) * 90;
            _currentWeaponDistance = _defenceWeaponDistance;
            WeaponOffset = _defenceWeaponOffset;
        }
    }


    protected override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _currentWeaponDistance = _range;
        
        switch (_attack)
        {
            case 0:
                return;
            case 1:
                _spriteRenderer.flipY = false;
                _targetAngle = -Math.Sign(transform.localScale.y) * _swipeRange;
                UserRecoil.Impact(-direction, _recoilForce);
                return;
            case 2:
                _spriteRenderer.flipY = true;
                _targetAngle = Math.Sign(transform.localScale.y) * _swipeRange;
                UserRecoil.Impact(-direction, _recoilForce);
                return;
            case 3:
                _targetAngle = 0;
                _spriteRenderer.flipY = false;
                _currentWeaponDistance *= _finalAttackPower;
                UserRecoil.Impact(-direction, _recoilForce * _finalAttackPower);
                return;
        }
    }
}
