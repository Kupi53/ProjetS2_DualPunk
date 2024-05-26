using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;


public class MeleeWeaponScript : WeaponScript
{
    [SerializeField] private int _criticalDamage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _swipeRange;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _resetCooldown;
    [SerializeField] private float _defenceTime;
    [SerializeField] private float _defenceCooldownSpeed;
    [SerializeField] private float _defenceWeaponDistance;
    [SerializeField] private Vector3 _defenceWeaponOffset;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _attackPoint;
    [SerializeField] private List<AudioClip> _attackSound;
    [SerializeField] private AudioClip _criticalSound;

    private int _attack;
    private float _angle;
    private float _attackTimer;
    private float _defenceTimer;
    private float _resetCooldownTimer;
    private bool _defenceCooldown;
    private bool _disableDefence;

    public int Damage { get => _damage; set => _damage = value; }
    public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public float ResetCooldown { get => _resetCooldown; set => _resetCooldown = value; }
    public int CriticalDamage { get => _criticalDamage; set => _criticalDamage = value; }
    public AudioClip CriticalSound { get => _criticalSound; set => _criticalSound = value; }

    public override bool DisplayInfo { get => _defenceTimer > 0 || _attackTimer > 0 && _attackTimer < _attackSpeed || _resetCooldownTimer > 0 && _attack == 3; }
    
    public override float InfoMaxTime {
        get {
            if (_attack == 3) return _resetCooldown;
            else if (_attack > 0) return _attackSpeed;
            else return _defenceTime;
        }
    }
    public override float InfoTimer { 
        get {
            if (_attack == 3) return _resetCooldownTimer;
            else if (_attack > 0) return _attackTimer;
            else return _defenceTime - _defenceTimer;
        }
    }


    private void Start()
    {
        ResetWeapon();
    }

    private new void Update()
    {
        base.Update();

        if (_attackTimer < _attackSpeed)
        {
            _attackTimer += Time.deltaTime;
        }
        if (_resetCooldownTimer < _resetCooldown)
        {
            _resetCooldownTimer += Time.deltaTime;
            if (_resetCooldownTimer >= _resetCooldown)
            {
                ResetWeapon();
            }
        }
        if (_defenceTimer > 0 && _defenceCooldown)
        {
            _defenceTimer -= Time.deltaTime * _defenceCooldownSpeed;
            if (_defenceTimer <= 0)
            {
                _defenceCooldown = false;
                _disableDefence = false;
            }
        }
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;

        if (_attack > 0 && DisplayInfo || _disableDefence)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;

        // contrairement a l'ennemi on peut cancel la defense quand on veut (sans avoir besoin d'attaquer)
        if (Input.GetButtonUp("SecondaryUse") && _attack == 0)
        {
            ResetOffset();
            _defenceCooldown = true;
            _disableDefence = false;
        }

        if (Input.GetButton("SecondaryUse") && !_disableDefence && _resetCooldownTimer >= _resetCooldown)
        {
            Defend();
        }
        else if (Input.GetButtonDown("Use") && _attack < 3 && _attackTimer >= _attackSpeed)
        {
            Attack();
        }

        MovePosition(position, direction, targetPoint);
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (EnemyState.CanAttack && _attack < 3 && _attackTimer >= _attackSpeed)
        {
            if (_defenceTimer > 0 && !_defenceCooldown)
            {
                ResetOffset();
                _defenceCooldown = true;
                _disableDefence = false;
            }

            Attack();
        }
        else if (!EnemyState.CanAttack && !_disableDefence && _resetCooldownTimer >= _resetCooldown)
        {
            Defend();
        }

        MovePosition(position, direction, targetPoint);
    }



    protected override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(WeaponOffset.x))
        {
            WeaponOffset = new Vector3(-WeaponOffset.x, WeaponOffset.y, 0);
        }
        if (_attack == 0 && Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            _angle = -_angle;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 0);
        }

        _attackPoint.transform.position = position + WeaponOffset + direction * (_currentWeaponDistance + _range / 2);

        if (_defenceTimer > 0 && !_defenceCooldown)
            transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        else
            transform.position = position + WeaponOffset + Quaternion.Euler(0, 0, _angle) * direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(Quaternion.Euler(0, 0, _angle) * direction));
    }

    public override void ResetWeapon()
    {
        _attack = 0;
        _defenceCooldown = true;
        _disableDefence = false;
        _spriteRenderer.flipY = false;
        _attackTimer = _attackSpeed;
        _resetCooldownTimer = _resetCooldown;

        ResetOffset();
    }

    // reset offset distance etc quand on desactive la defense
    private void ResetOffset()
    {
        _currentWeaponDistance = _weaponDistance;
        _angle = Math.Sign(transform.localScale.y) * _swipeRange;
        WeaponOffset = _weaponOffset;
    }


    private void Defend()
    {
        // gere le temps de defense et la bonne position de l'arme -------

        if (_currentWeaponDistance != _defenceWeaponDistance) // si oui alors on a pas encore set la bonne pos de l'arme
        {
            _angle = Math.Sign(transform.localScale.y) * 90;
            _currentWeaponDistance = _defenceWeaponDistance;
            WeaponOffset = _defenceWeaponOffset;
        }

        _defenceCooldown = false;
        _defenceTimer += Time.deltaTime;

        if (_defenceTimer > _defenceTime)
        {
            ResetOffset();
            _defenceCooldown = true;
            _disableDefence = true;
            return;
        }
        // ---------------------------------------------------------------
    }


    private void Attack()
    {
        _attack++;
        _attackTimer = 0;
        _resetCooldownTimer = 0;
        _currentWeaponDistance = _attackDistance;

        Random randomSound = new Random();
        AudioManager.Instance.PlayClipAt(_attackSound[randomSound.Next(_attackSound.Count)], gameObject.transform.position);

        switch (_attack)
        {
            case 0:
                return;
            case 1:
                _angle = -Math.Sign(transform.localScale.y) * _swipeRange;
                _spriteRenderer.flipY = false;
                return;
            case 2:
                _angle = Math.Sign(transform.localScale.y) * _swipeRange;
                _spriteRenderer.flipY = true;
                return;
            case 3:
                _angle = 0;
                _spriteRenderer.flipY = false;
                _currentWeaponDistance *= 1.2f;
                return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(_attackPoint.transform.position, _range / 2, _layerMask);
    }
}