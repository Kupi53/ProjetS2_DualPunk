using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;


public class EnergyBladeScript : MeleeWeaponScript
{
    [SerializeField] private Vector3 _defenceWeaponOffset;
    [SerializeField] private float _defenceWeaponDistance;
    [SerializeField] private float _swipeRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _finalAttackDistance;
    [SerializeField] private float _finalAttackAngle;

    private SpriteRenderer _spriteRenderer;
    private float _vel;
    private float _angle;
    private float _targetAngle;
    private float _currentWeaponDistance;


    private new void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        base.Start();
    }

    private new void Update()
    {
        base.Update();

        _angle = Mathf.SmoothDamp(_angle, _targetAngle, ref _vel, _attackSpeed);
    }


    public override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        base.MovePosition(position, direction, targetPoint);

        if (_attack == 0 && Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            _targetAngle = -_targetAngle;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 0);
        }

        if (_defenceTimer > 0 && !_defenceCooldown)
        {
            transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        }
        else
        {
            transform.position = position + WeaponOffset + Quaternion.Euler(0, 0, _angle) * direction * _currentWeaponDistance;
        }

        _attackPoint.transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(Quaternion.Euler(0, 0, _angle) * direction));
    }

    protected override void ResetPosition()
    {
        _currentWeaponDistance = _weaponDistance;
        _targetAngle = Math.Sign(transform.localScale.y) * _swipeRange;
        WeaponOffset = _weaponOffset;
        _spriteRenderer.flipY = false;
    }


    protected override void Defend(Vector3 direction)
    {
        if (_currentWeaponDistance != _defenceWeaponDistance) // si oui alors on a pas encore set la bonne pos de l'arme
        {
            _targetAngle = Math.Sign(transform.localScale.y) * 90;
            _currentWeaponDistance = _defenceWeaponDistance;
            WeaponOffset = _defenceWeaponOffset;
        }

        base.Defend(direction);

        BaseDefend();
    }


    protected override void Attack(Vector3 direction, bool damagePlayer)
    {
        _attack++;
        _attackTimer = 0;
        _resetCooldownTimer = 0;

        BaseAttack(direction, damagePlayer);

        _currentWeaponDistance = _attackDistance;

        switch (_attack)
        {
            case 1:
                _spriteRenderer.flipY = false;
                _targetAngle = -Math.Sign(transform.localScale.y) * _swipeRange;
                return;
            case 2:
                _spriteRenderer.flipY = true;
                _targetAngle = Math.Sign(transform.localScale.y) * _swipeRange;
                return;
            case 3:
                _spriteRenderer.flipY = false;
                _targetAngle = _finalAttackAngle;
                _currentWeaponDistance = _finalAttackDistance;
                return;
        }
    }
}
