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
    protected float _stunDuration;

    // Pour effet de set scavenger
    private int _criticalPercentage;
    private bool _setIsActive;
    private bool _critical;


    public int Damage { get => _damage; set => _damage = value; }
    public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
    public float ResetCooldown { get => _resetCooldown; set => _resetCooldown = value; }
    public int CriticalDamage { get => _criticalDamage; set => _criticalDamage = value; }
    public float StunDuration { get => _stunDuration; set => _stunDuration = value; }
    public AudioClip CriticalSound { get => _criticalSound; set => _criticalSound = value; }
    public int CriticalPercentage { get => _criticalPercentage; set => _criticalPercentage = value; }
    public bool SetIsActive { get => _setIsActive; set => _setIsActive = value; }

    public override bool DisplayInfo { get => _defenceTimer > 0 && _attack == 0 || _resetCooldownTimer > 0 && _attack > 0; }
    public override float InfoMaxTime { get => _attack > 0 ? _resetCooldown : _defenceTime; }
    public override float InfoTimer { get => _attack > 0 ? _resetCooldownTimer : _defenceTime - _defenceTimer; }


    protected void Start()
    {
        ResetWeapon();
        _setIsActive = false;
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
        MovePosition(position, direction, targetPoint);

        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;
        if (_attack > 0 && _attackTimer < _attackCooldown || _disableDefence)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;

        if ((Input.GetButtonUp("SecondaryUse") && _attack == 0 || PlayerState.Stop) && !_defenceCooldown)
        {
            ResetDefence();
            _disableDefence = false;
        }

        if (PlayerState.Stop) return;

        if (Input.GetButton("SecondaryUse") && !_disableDefence && _resetCooldownTimer >= _resetCooldown)
        {
            Defend(direction);
        }
        else if (Input.GetButtonDown("Use") && _attack < 3 && _attackTimer >= _attackCooldown)
        {
            Attack(direction, false);
        }
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (EnemyState.CanAttack && _attack < 3 && _attackTimer >= _attackCooldown)
        {
            Attack(direction, true);
        }
        else if (!EnemyState.CanAttack && !_disableDefence && _resetCooldownTimer >= _resetCooldown && (int)EnemyState.DefenceType > (int)DefenceType.NotDefending)
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


    protected virtual void Defend(Vector3 direction)
    {
        if (_ownerType == "Player")
            PlayerState.Walking = true;
        else
            EnemyState.DefenceType = DefenceType.Defending;

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


    protected virtual void Attack(Vector3 direction, bool damagePlayer)
    {
        _attack++;
        _attackTimer = 0;
        _resetCooldownTimer = 0;

        int damage;
        if (SetIsActive && UnityEngine.Random.Range(0, 100) < _criticalPercentage)
        {
            damage = _criticalDamage;
            _critical = true;
        }
        else
        {
            damage = _damage;
            _critical = false;
        }

        if (_critical)
            AudioManager.Instance.PlayClipAt(_criticalSound, gameObject.transform.position, _ownerType);
        else
        {
            System.Random randomSound = new System.Random();
            AudioManager.Instance.PlayClipAt(_attackSound[randomSound.Next(_attackSound.Count)], gameObject.transform.position, _ownerType);
        }

        float impactForce = _impactForce;
        if (_attack == 3)
        {
            if (_critical)
                damage = (int)(_criticalDamage * _finalAttackPower);
            else
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
                if (_critical)
                    hitObject.GetComponent<IDamageable>().Damage(damage, 0, true, _stunDuration);
                else
                    hitObject.GetComponent<IDamageable>().Damage(damage, 0, false, _stunDuration);
                hitObject.GetComponent<IImpact>().Impact(direction, impactForce);
            }
            if (hitObject.CompareTag("Projectile"))
            {
                hitObject.GetComponent<IDestroyable>().DestroyObject();
            }

            damagedObjects.Add(hitObject);
        }
    }
}