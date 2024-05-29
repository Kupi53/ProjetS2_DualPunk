using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class MeleeWeaponScript : WeaponScript
{
    [SerializeField] protected List<AudioClip> _attackSound;
    [SerializeField] protected AudioClip _criticalSound;
    [SerializeField] protected GameObject _attackPoint;
    [SerializeField] protected int _criticalDamage;
    [SerializeField] protected float _swipeRange;
    [SerializeField] protected float _finalAttackPower;
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _resetCooldown;
    [SerializeField] protected float _defenceTime;
    [SerializeField] protected float _defenceCooldownSpeed;
    [SerializeField] protected float _defenceWeaponDistance;
    [SerializeField] protected Vector3 _defenceWeaponOffset;
    [SerializeField] protected LayerMask _layerMask;

    protected int _attack;
    protected bool _defenceCooldown;
    protected bool _disableDefence;
    protected float _attackTimer;
    protected float _defenceTimer;
    protected float _resetCooldownTimer;


    public int Damage { get => _damage; set => _damage = value; }
    public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
    public float ResetCooldown { get => _resetCooldown; set => _resetCooldown = value; }
    public int CriticalDamage { get => _criticalDamage; set => _criticalDamage = value; }
    public AudioClip CriticalSound { get => _criticalSound; set => _criticalSound = value; }

    public override bool DisplayInfo { get => _defenceTimer > 0 && _attack == 0 || _resetCooldownTimer > 0 && _attack > 0; }
    public override float InfoMaxTime { get => _attack > 0 ? _resetCooldown : _defenceTime; }
    public override float InfoTimer { get => _attack > 0 ? _resetCooldownTimer : _defenceTime - _defenceTimer; }



    protected void Start()
    {
        ResetWeapon();
    }

    protected new void Update()
    {
        base.Update();

        if (_attackTimer < _attackCooldown)
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
        if (_defenceCooldown)
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

        if (_attack > 0 && _attackTimer < _attackCooldown || _disableDefence)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;

        if (Input.GetButton("SecondaryUse") && !_disableDefence && _resetCooldownTimer >= _resetCooldown)
        {
            Defend(direction);
        }
        else if (Input.GetButtonDown("Use") && _attack < 3 && _attackTimer >= _attackCooldown)
        {
            Attack(direction, false);
        }

        if (Input.GetButtonUp("SecondaryUse") && _attack == 0)
        {
            ResetDefence();
            _disableDefence = false;
        }

        MovePosition(position, direction, targetPoint);
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (EnemyState.CanAttack && _attack < 3 && _attackTimer >= _attackCooldown)
        {
            Attack(direction, true);
        }
        else if (!EnemyState.CanAttack && !_disableDefence && _resetCooldownTimer >= _resetCooldown)
        {
            Defend(direction);
        }

        if (EnemyState.CanAttack && _defenceTimer > 0 && !_defenceCooldown)
        {
            ResetDefence();
            _disableDefence = true;
        }

        MovePosition(position, direction, targetPoint);
    }


    public override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(WeaponOffset.x))
        {
            WeaponOffset = new Vector3(-WeaponOffset.x, WeaponOffset.y, 0);
        }
    }

    public override void ResetWeapon()
    {
        _attack = 0;
        _disableDefence = false;
        _spriteRenderer.flipY = false;
        _attackTimer = _attackCooldown;
        _resetCooldownTimer = _resetCooldown;

        if (_defenceTimer > 0 && !_defenceCooldown)
            ResetDefence();
        else
            ResetPosition();
    }
    protected abstract void ResetDefence();
    protected abstract void ResetPosition();


    protected virtual void Attack(Vector3 direction, bool damagePlayer)
    {
        _attack++;
        _attackTimer = 0;
        _resetCooldownTimer = 0;
        System.Random randomSound = new System.Random();
        AudioManager.Instance.PlayClipAt(_attackSound[randomSound.Next(_attackSound.Count)], gameObject.transform.position, _ownerType);

        int damage = _damage;
        float impactForce = _impactForce;
        if (_attack == 3)
        {
            damage = (int)(_damage * _finalAttackPower);
            impactForce *= _finalAttackPower;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_attackPoint.transform.position, _range / 2, _layerMask);
        List<GameObject> damagedObjects = new List<GameObject>();

        foreach (Collider2D hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;

            if (damagedObjects.Contains(hitObject)) continue;

            if (hitObject.CompareTag("Ennemy") && !damagePlayer || hitObject.CompareTag("Player") && damagePlayer)
            {
                hitObject.GetComponent<IDamageable>().Damage(damage, 0, false);
                hitObject.GetComponent<IImpact>().Impact(direction, impactForce);
            }
            if (hitObject.CompareTag("Projectile"))
            {
                hitObject.GetComponent<IDestroyable>().DestroyObject();
            }

            damagedObjects.Add(hitObject);
        }
    }


    protected virtual void Defend(Vector3 direction)
    {
        if (_ownerType == "Player")
            PlayerState.Walking = true;
        else
            EnemyState.Defending = true;

        _defenceCooldown = false;
        _defenceTimer += Time.deltaTime;

        if (_defenceTimer > _defenceTime)
        {
            ResetDefence();
            _disableDefence = true;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(_attackPoint.transform.position, _range / 2, _layerMask);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Projectile"))
            {
                hitCollider.GetComponent<IDestroyable>().DestroyObject();
            }
        }
    }
}