using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class MeleeWeaponScript : WeaponScript
{
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _resetCooldown;
    [SerializeField] private List<AudioClip> _attackSound;

    private float _angle;
    private float _attack;
    private float _rangeTop;
    private float _rangeMiddle;
    private float _rangeBottom;
    private float _resetCooldownTimer;
    private float _currentWeaponDistance;

    public float Attack { get => _attack; set => _attack = value; }

    public override bool DisplayInfo { get => Attack != 0; }
    public override float InfoMaxTime { get => _resetCooldown; }
    public override float InfoTimer { get => _resetCooldownTimer; }


    private new void Start()
    {
        base.Start();

        _attack = 0;
        _resetCooldownTimer = 0;
        _currentWeaponDistance = _weaponDistance;
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (!Owner.IsLocalClient) return;

        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;

        if (Input.GetButtonDown("Use") && !PlayerState.Attacking && _attack < 3)
        {
            _angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            _attack += 1;
            _resetCooldownTimer = 0;
            PlayerState.Attacking = true;

            _rangeMiddle = _angle;
            _rangeTop = _angle + _attackRange;
            _rangeBottom = _angle - _attackRange;

            if (_attack == 2)
            {
                _angle = _rangeBottom;
                _spriteRenderer.flipY = true;
            }
            else
            {
                _angle = _rangeTop;
                _spriteRenderer.flipY = false;
            }
            
            Random randomSound = new Random();
            AudioManager.Instance.PlayClipAt(_attackSound[randomSound.Next(_attackSound.Count)], gameObject.transform.position);
        }

        if (PlayerState.Attacking)
        {
            switch (_attack)
            {
                case 1:
                    _angle -= _attackSpeed * Time.deltaTime;
                    if (_angle <= _rangeBottom)
                        PlayerState.Attacking = false;
                    break;

                case 2:
                    _angle += _attackSpeed * Time.deltaTime;
                    if (_angle >= _rangeTop)
                        PlayerState.Attacking = false;
                    break;

                case 3:
                    if (_angle < _rangeMiddle)
                        _angle -= _attackSpeed * Time.deltaTime;
                    else if (_currentWeaponDistance < _attackDistance)
                        _currentWeaponDistance += Time.deltaTime * 10;
                    else
                    {
                        PlayerState.Attacking = false;
                        _currentWeaponDistance = _attackDistance;
                    }
                    break;
            }
        }

        else
        {
            _angle = Methods.GetAngle(direction);

            switch (_attack)
            {
                case 1:
                    _angle -= _attackRange;
                    break;
                case 2:
                    _angle += _attackRange;
                    break;
                case 0:
                    if (_angle > 90 || _angle < -90)
                        _spriteRenderer.flipY = true;
                    else
                        _spriteRenderer.flipY = false;
                    break;
            }
        }


        if (_attack != 0)
        {
            _resetCooldownTimer += Time.deltaTime;
            if (_resetCooldownTimer > _resetCooldown)
            {
                ResetWeapon();
            }

            direction = new Vector3((float)Math.Cos(_angle * Math.PI / 180), (float)Math.Sin(_angle * Math.PI / 180)).normalized;
        }

        transform.position = position + _weaponOffset + direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, _angle);
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        
    }


    public override void ResetWeapon()
    {
        _attack = 0;
        _resetCooldownTimer = 0;
        _currentWeaponDistance = _weaponDistance;
    }
}